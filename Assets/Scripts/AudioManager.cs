using UnityEngine;
using System.Collections;

/// <summary>
/// Background music + SFX. Ducks volume while Luma TTS is speaking.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips (optional)")]
    public AudioClip calmClip;
    public AudioClip dangerClip;
    public AudioClip collectClip;
    public AudioClip winClip;
    public AudioClip failedClip;

    [Header("Voice Ducking (while Luma speaks)")]
    [Tooltip("Music volume multiplier when Luma is talking (0 = silent, 1 = full).")]
    [Range(0f, 1f)]
    public float duckedMusicVolume = 0.15f;

    [Tooltip("SFX volume multiplier when Luma is talking.")]
    [Range(0f, 1f)]
    public float duckedSfxVolume = 0.1f;

    [Tooltip("Seconds to fade between normal and ducked volume.")]
    public float duckFadeSeconds = 0.35f;

    private float normalMusicVolume = 1f;
    private float normalSfxVolume = 1f;
    private int voiceDuckCount;
    private Coroutine duckFadeRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
        }

        normalMusicVolume = musicSource.volume;
        normalSfxVolume = sfxSource.volume;
    }

    void Start()
    {
        PlayCalm();
    }

    /// <summary>Lower background audio so Luma's voice is clearer.</summary>
    public void BeginVoiceDuck()
    {
        voiceDuckCount++;
        if (voiceDuckCount == 1)
            StartDuckFade(ducked: true);
    }

    /// <summary>Restore background audio after Luma finishes speaking.</summary>
    public void EndVoiceDuck()
    {
        voiceDuckCount = Mathf.Max(0, voiceDuckCount - 1);
        if (voiceDuckCount == 0)
            StartDuckFade(ducked: false);
    }

    void StartDuckFade(bool ducked)
    {
        if (duckFadeRoutine != null)
            StopCoroutine(duckFadeRoutine);

        duckFadeRoutine = StartCoroutine(FadeVolumes(ducked));
    }

    IEnumerator FadeVolumes(bool ducked)
    {
        float musicStart = musicSource != null ? musicSource.volume : 1f;
        float sfxStart = sfxSource != null ? sfxSource.volume : 1f;
        float musicEnd = ducked ? normalMusicVolume * duckedMusicVolume : normalMusicVolume;
        float sfxEnd = ducked ? normalSfxVolume * duckedSfxVolume : normalSfxVolume;

        float elapsed = 0f;
        float duration = Mathf.Max(duckFadeSeconds, 0.01f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            if (musicSource != null)
                musicSource.volume = Mathf.Lerp(musicStart, musicEnd, t);

            if (sfxSource != null)
                sfxSource.volume = Mathf.Lerp(sfxStart, sfxEnd, t);

            yield return null;
        }

        if (musicSource != null)
            musicSource.volume = musicEnd;
        if (sfxSource != null)
            sfxSource.volume = sfxEnd;

        duckFadeRoutine = null;
    }

    public void PlayCalm()
    {
        PlayMusic(calmClip);
    }

    public void PlayDanger()
    {
        PlayMusic(dangerClip);
    }

    public void PlayCollect()
    {
        PlaySfx(collectClip);
    }

    public void PlayWin()
    {
        PlaySfx(winClip);
    }

    public void PlayFailed()
    {
        PlaySfx(failedClip);
    }

    /// <summary>Plays win SFX and returns clip length in seconds (for sequencing Luma voice after).</summary>
    public float PlayWinAndGetDuration()
    {
        if (winClip == null || sfxSource == null)
            return 2f;

        sfxSource.PlayOneShot(winClip);
        return winClip.length;
    }

    void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();

        // Keep ducked level if Luma is still speaking.
        if (voiceDuckCount > 0)
            musicSource.volume = normalMusicVolume * duckedMusicVolume;
    }

    void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip);
    }
}
