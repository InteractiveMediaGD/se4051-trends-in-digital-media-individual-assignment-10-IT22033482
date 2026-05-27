using UnityEngine;
using TMPro;
using System.Collections;
using System.Reflection;

/// <summary>
/// Central game brain: seeds, stress, forest mood, UI, and win state.
/// Place on an empty GameManager object in the scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public TMP_Text objectiveText;
    public TMP_Text seedCountText;
    public TMP_Text stressText;
    public TMP_Text statusText;

    [Header("World References")]
    public Light directionalLight;
    public ForestHeart forestHeart;
    public LumaGuide lumaGuide;
    public Transform playerTransform;

    [Header("Fog Settings")]
    public float calmFogDensity = 0.02f;
    public float corruptedFogDensity = 0.04f;
    public float restoredFogDensity = 0.005f;

    [Header("Stress Tuning")]
    [Tooltip("Stress removed when collecting one Light Seed (does not force 0 unless stress was already low).")]
    public int stressReductionOnSeedCollect = 20;
    [Tooltip("Instant stress removed when entering a Safe Zone.")]
    public int stressReductionOnEnterSafeZone = 5;
    [Tooltip("Stress gained per second while inside a Shadow Zone.")]
    public float shadowZoneStressPerSecond = 4f;
    [Tooltip("Stress lost per second while inside a Safe Zone.")]
    public float safeZoneRecoveryPerSecond = 8f;
    [Tooltip("If stress goes above this, the player loses.")]
    public int stressGameOverThreshold = 80;

    [Header("Fail Conditions")]
    [Tooltip("If player Y position goes below this, the player loses.")]
    public float fallGameOverY = -8f;

    [Header("State (read-only in Play mode)")]
    public int seedCount;
    public int stressLevel;
    public bool isInShadowZone;
    public bool isInSafeZone;
    public bool reducedIntensity;

    public enum ForestMood { Calm, Curious, Alert, Corrupted, Supportive, Restored }
    public ForestMood currentMood = ForestMood.Calm;

    private Color calmLightColor = Color.white;
    private Color corruptedLightColor = new Color(0.6f, 0.2f, 0.8f);
    private Color restoredLightColor = new Color(1f, 0.9f, 0.4f);

    private float timeSinceLastSeed;
    private float lostTimer;
    private float shadowStressAccumulator;
    private float safeRecoveryAccumulator;
    private bool hasGameEnded;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
        }

        RenderSettings.fog = true;
        ApplyCalmEnvironment();

        if (objectiveText != null)
            objectiveText.text = "Collect 5 Light Seeds to restore the Forest Heart.";

        SetStatus("Welcome to Flowbound Grove.");
    }

    void Update()
    {
        if (hasGameEnded)
            return;

        UpdateStressOverTime();
        CheckFailConditions();
        UpdateMood();
        UpdateEnvironment();
        UpdateUI();
    }

    public void CollectSeed()
    {
        if (seedCount >= 5)
            return;

        seedCount++;
        timeSinceLastSeed = 0f;
        lostTimer = 0f;
        ChangeStress(-stressReductionOnSeedCollect);
        SetStatus($"A Light Seed was collected! Stress eased by {stressReductionOnSeedCollect}.");

        RenderSettings.fogDensity = Mathf.Max(restoredFogDensity, RenderSettings.fogDensity - 0.008f);

        if (lumaGuide != null)
            lumaGuide.Celebrate();

        AudioManager.Instance?.PlayCollect();

        if (seedCount >= 5)
        {
            SetStatus("All seeds collected! The Forest Heart is unlocked.");
            if (forestHeart != null)
                forestHeart.UnlockHeart();
        }
    }

    public void EnterShadowZone()
    {
        isInShadowZone = true;
        ChangeStress(8);
        SetStatus("Warning: You entered a Shadow Zone!");
        AudioManager.Instance?.PlayDanger();
    }

    public void ExitShadowZone()
    {
        isInShadowZone = false;
        SetStatus("You left the Shadow Zone.");
        AudioManager.Instance?.PlayCalm();
    }

    public void EnterSafeZone()
    {
        isInSafeZone = true;
        ChangeStress(-stressReductionOnEnterSafeZone);
        SetStatus("Safe Zone — stress is easing.");
    }

    public void ExitSafeZone()
    {
        isInSafeZone = false;
    }

    public void ChangeStress(int amount)
    {
        stressLevel = Mathf.Clamp(stressLevel + amount, 0, 100);

        if (!hasGameEnded && stressLevel > stressGameOverThreshold)
            LoseGame($"Stress exceeded {stressGameOverThreshold}. You were consumed by the forest's corruption.");
    }

    public void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    public void WinGame()
    {
        if (hasGameEnded)
            return;

        hasGameEnded = true;
        currentMood = ForestMood.Restored;
        SetStatus("Forest restored! The grove is alive again.");
        ApplyRestoredEnvironment();
        StartCoroutine(WinSequence());
    }

    public void LoseGame(string reason)
    {
        if (hasGameEnded)
            return;

        hasGameEnded = true;
        SetStatus(reason);
        AudioManager.Instance?.PlayFailed();
        TryShowEndScreen();
    }

    IEnumerator WinSequence()
    {
        // 1) Win music first.
        float winDuration = 2f;
        if (AudioManager.Instance != null)
            winDuration = AudioManager.Instance.PlayWinAndGetDuration();

        // Luma moves to restore position (no voice yet).
        if (lumaGuide != null)
            lumaGuide.EnterRestoreState();

        yield return new WaitForSeconds(winDuration);

        // 2) Luma's final message after win music.
        if (lumaGuide != null)
            lumaGuide.PlayRestoreVoiceLine();

        // 3) End screen after Luma finishes speaking.
        // Use reflection to avoid a compile-time dependency if GameFlowManager fails to compile.
        TryShowEndScreen();
    }

    void TryShowEndScreen()
    {
        var asm = typeof(GameManager).Assembly;
        var gfType = asm.GetType("GameFlowManager");
        if (gfType == null)
            return;

        var instanceProp = gfType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
        object gfInstance = instanceProp != null ? instanceProp.GetValue(null) : null;
        if (gfInstance == null)
            return;

        var showMethod = gfType.GetMethod("ShowEndScreen", BindingFlags.Public | BindingFlags.Instance);
        showMethod?.Invoke(gfInstance, null);
    }

    public float GetWispSpeedMultiplier()
    {
        if (reducedIntensity)
            return 0.5f;
        if (stressLevel >= 81)
            return 0.6f;
        return 1f;
    }

    void UpdateStressOverTime()
    {
        timeSinceLastSeed += Time.deltaTime;

        // Safe Zone wins if zones overlap — do not raise stress while recovering.
        if (isInShadowZone && !isInSafeZone)
            ApplyStressOverTime(ref shadowStressAccumulator, shadowZoneStressPerSecond, increase: true);

        if (isInSafeZone)
            ApplyStressOverTime(ref safeRecoveryAccumulator, safeZoneRecoveryPerSecond, increase: false);

        if (seedCount > 0 && seedCount < 5)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer > 25f)
            {
                ChangeStress(1);
                lostTimer = 0f;
                SetStatus("Luma senses you may be lost...");
            }
        }
    }

    void CheckFailConditions()
    {
        if (playerTransform != null && playerTransform.position.y < fallGameOverY)
            LoseGame("You fell from the floating islands. Game Over.");
    }

    void UpdateMood()
    {
        if (forestHeart != null && forestHeart.IsRestored)
        {
            currentMood = ForestMood.Restored;
            return;
        }

        if (stressLevel >= 61)
            currentMood = ForestMood.Supportive;
        else if (isInShadowZone)
            currentMood = ForestMood.Corrupted;
        else if (stressLevel >= 31)
            currentMood = ForestMood.Alert;
        else
            currentMood = ForestMood.Calm;
    }

    void UpdateEnvironment()
    {
        if (currentMood == ForestMood.Restored)
        {
            ApplyRestoredEnvironment();
            return;
        }

        if (isInShadowZone && !reducedIntensity)
        {
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, corruptedFogDensity, Time.deltaTime * 2f);
            if (directionalLight != null)
                directionalLight.color = Color.Lerp(directionalLight.color, corruptedLightColor, Time.deltaTime * 2f);
        }
        else if (!isInShadowZone)
        {
            float targetFog = reducedIntensity ? calmFogDensity * 0.5f : calmFogDensity;
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, targetFog, Time.deltaTime * 1.5f);
            if (directionalLight != null)
                directionalLight.color = Color.Lerp(directionalLight.color, calmLightColor, Time.deltaTime * 1.5f);
        }
    }

    void UpdateUI()
    {
        if (seedCountText != null)
            seedCountText.text = "Seeds: " + seedCount + " / 5";

        if (stressText != null)
            stressText.text = "Stress: " + stressLevel;
    }

    /// <summary>
    /// Applies fractional stress change per second (avoids RoundToInt(deltaTime) always being 0).
    /// </summary>
    void ApplyStressOverTime(ref float accumulator, float ratePerSecond, bool increase)
    {
        accumulator += Time.deltaTime * ratePerSecond;

        while (accumulator >= 1f)
        {
            ChangeStress(increase ? 1 : -1);
            accumulator -= 1f;
        }
    }

    void ApplyCalmEnvironment()
    {
        RenderSettings.fogDensity = reducedIntensity ? calmFogDensity * 0.5f : calmFogDensity;
        if (directionalLight != null)
            directionalLight.color = calmLightColor;
    }

    void ApplyRestoredEnvironment()
    {
        RenderSettings.fogDensity = restoredFogDensity;
        if (directionalLight != null)
            directionalLight.color = restoredLightColor;
    }
}
