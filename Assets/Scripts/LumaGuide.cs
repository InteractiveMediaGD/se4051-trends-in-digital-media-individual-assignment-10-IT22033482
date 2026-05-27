using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// Intelligent guide NPC with state-based behavior:
/// Follow, Warn, Guide, Celebrate, Restore.
/// </summary>
public class LumaGuide : MonoBehaviour
{
    public enum LumaState { Follow, Warn, Guide, Celebrate, Restore }

    [Header("References")]
    public Transform player;
    public Transform guideTarget;

    [Header("Movement")]
    public float followSpeed = 3f;
    public float followDistance = 2.5f;
    public float floatHeight = 1.2f;

    [Header("Colors")]
    public Color followColor = new Color(0.4f, 1f, 0.85f);
    public Color warnColor = new Color(1f, 0.45f, 0.2f);
    public Color guideColor = new Color(0.3f, 0.7f, 1f);
    public Color celebrateColor = Color.yellow;

    public LumaState CurrentState { get; private set; } = LumaState.Follow;

    private Renderer rend;
    private Light lumaLight;
    private Coroutine celebrateRoutine;

    // Track previous state so voice lines only fire on state change, not every frame.
    private LumaState previousState = LumaState.Follow;
    private bool hasSpokenWelcome;

    void Start()
    {
        rend = GetComponent<Renderer>();
        lumaLight = GetComponentInChildren<Light>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        StartCoroutine(WaitForGameplayAndSpeakWelcome());
    }

    IEnumerator WaitForGameplayAndSpeakWelcome()
    {
        while (!IsGameplayStarted())
            yield return null;

        yield return new WaitForSeconds(1.5f);
        SpeakWelcome();
    }

    bool IsGameplayStarted()
    {
        if (GameFlowManager.Instance == null)
            return true;

        return GameFlowManager.Instance.IsGameplayActive;
    }

    void SpeakWelcome()
    {
        if (!hasSpokenWelcome)
        {
            hasSpokenWelcome = true;
            LumaTTS.Instance?.Speak("I am Luma. I will guide you through the forest. Find the five Light Seeds.");
        }
    }

    void Update()
    {
        if (player == null || GameManager.Instance == null)
            return;

        if (CurrentState != LumaState.Celebrate)
            UpdateState();

        Move();
        ApplyColorForState();
    }

    void UpdateState()
    {
        var gm = GameManager.Instance;

        if (gm.currentMood == GameManager.ForestMood.Restored)
        {
            CurrentState = LumaState.Restore;
        }
        else if (InputReaderUtil.IsPressed(Key.H) || gm.stressLevel >= 61)
        {
            CurrentState = LumaState.Guide;
            UpdateGuideTarget();
        }
        else if (gm.isInShadowZone || gm.stressLevel >= 31)
        {
            CurrentState = LumaState.Warn;
            if (gm.isInShadowZone)
                gm.SetStatus("Luma warns: Leave the Shadow Zone!");
        }
        else
        {
            CurrentState = LumaState.Follow;
        }

        // Speak only when the state changes.
        if (CurrentState != previousState)
        {
            OnStateChanged(previousState, CurrentState);
            previousState = CurrentState;
        }
    }

    void UpdateGuideTarget()
    {
        if (guideTarget != null)
            return;

        LightSeed[] seeds = FindObjectsByType<LightSeed>(FindObjectsSortMode.None);
        if (seeds.Length > 0)
        {
            guideTarget = seeds[0].transform;
            return;
        }

        ForestHeart heart = FindFirstObjectByType<ForestHeart>();
        if (heart != null)
            guideTarget = heart.transform;
    }

    void Move()
    {
        Vector3 targetPos;

        switch (CurrentState)
        {
            case LumaState.Guide when guideTarget != null:
                targetPos = guideTarget.position + Vector3.up * floatHeight;
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed * 1.5f);
                break;

            case LumaState.Restore:
                ForestHeart heart = FindFirstObjectByType<ForestHeart>();
                if (heart != null)
                {
                    targetPos = heart.transform.position + Vector3.right * 2f + Vector3.up * floatHeight;
                    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
                }
                break;

            default:
                targetPos = player.position + player.forward * -followDistance + Vector3.up * floatHeight;
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
                break;
        }

        if (CurrentState == LumaState.Warn)
            transform.position = Vector3.Lerp(transform.position, player.position + Vector3.up * floatHeight, Time.deltaTime * followSpeed * 1.2f);
    }

    void ApplyColorForState()
    {
        if (rend == null)
            return;

        Color target = followColor;
        switch (CurrentState)
        {
            case LumaState.Warn: target = warnColor; break;
            case LumaState.Guide: target = guideColor; break;
            case LumaState.Celebrate: target = celebrateColor; break;
            case LumaState.Restore: target = celebrateColor; break;
        }

        rend.material.color = Color.Lerp(rend.material.color, target, Time.deltaTime * 4f);

        if (lumaLight != null)
            lumaLight.color = target;
    }

    public void Celebrate()
    {
        if (celebrateRoutine != null)
            StopCoroutine(celebrateRoutine);

        celebrateRoutine = StartCoroutine(CelebrateRoutine());
    }

    IEnumerator CelebrateRoutine()
    {
        CurrentState = LumaState.Celebrate;
        int remaining = 5 - GameManager.Instance.seedCount;
        if (remaining > 0)
            LumaTTS.Instance?.Speak($"Well done! {remaining} seed{(remaining == 1 ? "" : "s")} remaining.");
        else
            LumaTTS.Instance?.Speak("All seeds collected! Take them to the Forest Heart!");
        GameManager.Instance?.SetStatus("Luma celebrates your progress!");
        yield return new WaitForSeconds(1.5f);
        CurrentState = LumaState.Follow;
        previousState = LumaState.Follow;
        celebrateRoutine = null;
    }

    public void EnterRestoreState()
    {
        CurrentState = LumaState.Restore;
        previousState = LumaState.Restore;
    }

    /// <summary>Called after win music finishes — only the end-game voice line.</summary>
    public void PlayRestoreVoiceLine()
    {
        LumaTTS.Instance?.Speak("The forest is restored. You have done it. Thank you.");
    }

    void OnStateChanged(LumaState from, LumaState to)
    {
        switch (to)
        {
            case LumaState.Warn:
                LumaTTS.Instance?.Speak("Be careful! Danger is near. Stay close to me.");
                break;

            case LumaState.Guide:
                LumaTTS.Instance?.Speak("Follow me. I will show you the way.");
                break;

            case LumaState.Follow when from == LumaState.Warn:
                LumaTTS.Instance?.Speak("Good. You are safe now.");
                break;
        }
    }
}
