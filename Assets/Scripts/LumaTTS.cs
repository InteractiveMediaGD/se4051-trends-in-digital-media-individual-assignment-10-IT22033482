using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

/// <summary>
/// Luma voice — Windows built-in TTS (free, offline).
/// Uses PowerShell + System.Speech (main-thread safe paths only).
/// </summary>
public class LumaTTS : MonoBehaviour
{
    public static LumaTTS Instance { get; private set; }

    [Header("Voice Settings")]
    [Range(-5, 10)]
    public int speakRate = 2;

    [Range(0, 100)]
    public int volume = 80;

    [Tooltip("Optional e.g. Microsoft Zira Desktop. Leave blank for default.")]
    public string preferredVoiceName = "";

    [Header("3D Audio")]
    public AudioSource audioSource;
    public float voiceMaxDistance = 20f;

    private bool ttsAvailable;
    private bool usePowerShellBackend = true;
    private bool isSpeaking;

    public bool IsSpeaking => isSpeaking;
    private readonly Queue<string> speechQueue = new Queue<string>();

    // Cached on main thread — never call Application.* from background threads.
    private string cacheFolder;

    // Optional STA / reflection backend.
    private object synthesizer;
    private Type synthType;
    private MethodInfo speakMethod;
    private MethodInfo setOutputToWaveStreamMethod;
    private MethodInfo setOutputToNullMethod;
    private PropertyInfo rateProperty;
    private PropertyInfo volumeProperty;
    private readonly object staLock = new object();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        cacheFolder = Application.temporaryCachePath;
        SetupAudioSource();
        InitialiseTTS();
    }

    void SetupAudioSource()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = voiceMaxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.playOnAwake = false;
        audioSource.volume = 1f;
    }

    void InitialiseTTS()
    {
        if (!IsWindowsPlatform())
        {
            UnityEngine.Debug.Log("[LumaTTS] Windows only. Voice disabled.");
            return;
        }

        // PowerShell is most reliable inside Unity Editor (no COM/STA issues).
        if (TestPowerShellTTS())
        {
            ttsAvailable = true;
            usePowerShellBackend = true;
            UnityEngine.Debug.Log("[LumaTTS] Ready (PowerShell).");
            return;
        }

        if (TryInitReflectionSTA())
        {
            ttsAvailable = true;
            usePowerShellBackend = false;
            UnityEngine.Debug.Log("[LumaTTS] Ready (System.Speech / STA).");
            return;
        }

        UnityEngine.Debug.LogWarning("[LumaTTS] TTS failed. Install a voice: Settings → Time & language → Speech.");
    }

    bool TestPowerShellTTS()
    {
        try
        {
            string testWav = Path.Combine(cacheFolder, "luma_tts_test.wav");
            byte[] wav = SynthesizeViaPowerShell("test", testWav);
            if (wav != null && wav.Length > 44)
            {
                try { File.Delete(testWav); } catch { }
                return true;
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning("[LumaTTS] PowerShell test: " + e.Message);
        }

        return false;
    }

    bool TryInitReflectionSTA()
    {
        Exception initError = null;
        bool ready = false;

        Thread staThread = new Thread(() =>
        {
            try
            {
                Assembly speechAssembly = LoadSystemSpeechAssembly();
                if (speechAssembly == null)
                    throw new Exception("System.Speech.dll not found.");

                synthType = speechAssembly.GetType("System.Speech.Synthesis.SpeechSynthesizer");
                if (synthType == null)
                    throw new Exception("SpeechSynthesizer type missing.");

                synthesizer = Activator.CreateInstance(synthType);
                if (synthesizer == null)
                    throw new Exception("Could not create SpeechSynthesizer.");

                rateProperty = synthType.GetProperty("Rate");
                volumeProperty = synthType.GetProperty("Volume");
                speakMethod = synthType.GetMethod("Speak", new[] { typeof(string) });
                setOutputToWaveStreamMethod = synthType.GetMethod("SetOutputToWaveStream", new[] { typeof(Stream) });
                setOutputToNullMethod = synthType.GetMethod("SetOutputToNull", Type.EmptyTypes);

                if (speakMethod == null || setOutputToWaveStreamMethod == null)
                    throw new Exception("SpeechSynthesizer methods not found.");

                rateProperty?.SetValue(synthesizer, speakRate);
                volumeProperty?.SetValue(synthesizer, volume);

                if (!string.IsNullOrWhiteSpace(preferredVoiceName))
                    TrySelectVoiceOnSTA(preferredVoiceName);

                ready = true;
            }
            catch (Exception e)
            {
                initError = e;
            }
        });

        staThread.IsBackground = true;
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start();
        staThread.Join(8000);

        if (initError != null)
            UnityEngine.Debug.LogWarning("[LumaTTS] STA init: " + FormatException(initError));

        return ready;
    }

    public void Speak(string text)
    {
        if (!ttsAvailable || string.IsNullOrWhiteSpace(text))
            return;

        speechQueue.Enqueue(text);
        if (!isSpeaking)
            StartCoroutine(ProcessQueue());
    }

    IEnumerator ProcessQueue()
    {
        isSpeaking = true;
        AudioManager.Instance?.BeginVoiceDuck();

        while (speechQueue.Count > 0)
            yield return StartCoroutine(SpeakOnePhrase(speechQueue.Dequeue()));

        AudioManager.Instance?.EndVoiceDuck();
        isSpeaking = false;
    }

    IEnumerator SpeakOnePhrase(string text)
    {
        // Build path on MAIN thread only.
        string wavPath = Path.Combine(cacheFolder, "luma_" + Guid.NewGuid().ToString("N") + ".wav");

        byte[] wavBytes = null;
        string error = null;
        bool done = false;

        Thread worker = new Thread(() =>
        {
            try
            {
                if (usePowerShellBackend)
                    wavBytes = SynthesizeViaPowerShell(text, wavPath);
                else
                    wavBytes = SynthesizeOnSTAThread(text);
            }
            catch (Exception e)
            {
                error = FormatException(e);
            }
            finally
            {
                try { if (File.Exists(wavPath)) File.Delete(wavPath); } catch { }
                done = true;
            }
        });

        worker.IsBackground = true;
        if (!usePowerShellBackend)
            worker.SetApartmentState(ApartmentState.STA);
        worker.Start();

        float waited = 0f;
        while (!done && waited < 15f)
        {
            waited += Time.unscaledDeltaTime;
            yield return null;
        }

        if (error != null)
        {
            UnityEngine.Debug.LogWarning("[LumaTTS] Speak failed: " + error);
            yield break;
        }

        if (wavBytes == null || wavBytes.Length < 44)
        {
            UnityEngine.Debug.LogWarning("[LumaTTS] No audio for: " + text);
            yield break;
        }

        AudioClip clip = WavBytesToAudioClip(wavBytes);
        if (clip == null)
            yield break;

        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSeconds(clip.length);
    }

    byte[] SynthesizeOnSTAThread(string text)
    {
        byte[] result = null;
        Exception error = null;

        Thread t = new Thread(() =>
        {
            lock (staLock)
            {
                try
                {
                    rateProperty?.SetValue(synthesizer, speakRate);
                    volumeProperty?.SetValue(synthesizer, volume);

                    using MemoryStream stream = new MemoryStream();
                    setOutputToWaveStreamMethod.Invoke(synthesizer, new object[] { stream });
                    speakMethod.Invoke(synthesizer, new object[] { text });
                    if (setOutputToNullMethod != null)
                        setOutputToNullMethod.Invoke(synthesizer, Array.Empty<object>());

                    result = stream.ToArray();
                }
                catch (Exception e)
                {
                    error = e;
                }
            }
        });

        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join(12000);

        if (error != null)
            throw error;

        return result;
    }

    byte[] SynthesizeViaPowerShell(string text, string wavPath)
    {
        string safeText = text.Replace("'", "''");
        string safePath = wavPath.Replace("'", "''");
        string voiceLine = "";

        if (!string.IsNullOrWhiteSpace(preferredVoiceName))
        {
            string safeVoice = preferredVoiceName.Replace("'", "''");
            voiceLine = $"try {{ $s.SelectVoice('{safeVoice}') }} catch {{ }} ";
        }

        string script =
            "Add-Type -AssemblyName System.Speech; " +
            "$s = New-Object System.Speech.Synthesis.SpeechSynthesizer; " +
            $"$s.Rate = {speakRate}; $s.Volume = {volume}; " +
            voiceLine +
            $"$s.SetOutputToWaveFile('{safePath}'); " +
            $"$s.Speak('{safeText}'); " +
            "$s.Dispose()";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = "-NoProfile -ExecutionPolicy Bypass -Command \"" + script + "\"",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };

        using Process process = Process.Start(psi);
        if (process == null)
            return null;

        string stderr = process.StandardError.ReadToEnd();
        process.WaitForExit(15000);

        if (process.ExitCode != 0 && !string.IsNullOrEmpty(stderr))
            UnityEngine.Debug.LogWarning("[LumaTTS] PowerShell: " + stderr.Trim());

        if (!File.Exists(wavPath))
            return null;

        return File.ReadAllBytes(wavPath);
    }

    void TrySelectVoiceOnSTA(string voiceName)
    {
        try
        {
            MethodInfo selectVoice = synthType.GetMethod("SelectVoice", new[] { typeof(string) });
            selectVoice?.Invoke(synthesizer, new object[] { voiceName });
        }
        catch
        {
            UnityEngine.Debug.LogWarning("[LumaTTS] Voice not found: " + voiceName + ". Using default.");
        }
    }

    static string FormatException(Exception e)
    {
        if (e is TargetInvocationException tie && tie.InnerException != null)
            return tie.InnerException.Message;
        return e.Message;
    }

    static bool IsWindowsPlatform()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        return true;
#else
        return Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsEditor;
#endif
    }

    static Assembly LoadSystemSpeechAssembly()
    {
        try
        {
            return Assembly.Load("System.Speech, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        }
        catch { }

        string[] paths =
        {
            @"C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Speech\v4.0_4.0.0.0__31bf3856ad364e35\System.Speech.dll",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                @"Microsoft.NET\Framework64\v4.0.30319\System.Speech.dll"),
        };

        foreach (string path in paths)
        {
            if (!File.Exists(path)) continue;
            try { return Assembly.LoadFrom(path); }
            catch { }
        }

        return null;
    }

    static AudioClip WavBytesToAudioClip(byte[] wav)
    {
        if (wav == null || wav.Length < 44) return null;

        int channels = wav[22] | (wav[23] << 8);
        int sampleRate = wav[24] | (wav[25] << 8) | (wav[26] << 16) | (wav[27] << 24);
        int bitDepth = wav[34] | (wav[35] << 8);
        int dataStart = 44;
        int bytesPerSample = Mathf.Max(bitDepth / 8, 1);
        int sampleCount = (wav.Length - dataStart) / bytesPerSample / Mathf.Max(channels, 1);

        float[] samples = new float[sampleCount * channels];
        if (bitDepth == 16)
        {
            for (int i = 0; i < samples.Length; i++)
            {
                int offset = dataStart + i * 2;
                if (offset + 1 >= wav.Length) break;
                short raw = (short)(wav[offset] | (wav[offset + 1] << 8));
                samples[i] = raw / 32768f;
            }
        }

        AudioClip clip = AudioClip.Create("LumaVoice", sampleCount, channels, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    void OnDestroy()
    {
        if (synthesizer is IDisposable d)
            d.Dispose();
    }
}
