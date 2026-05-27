using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Manages Start (loading/logo) screen and End (win) screen.
/// Wire UI panels and buttons in the Inspector.
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("UI Panels")]
    [Tooltip("Full-screen panel: logo + loading + Start button.")]
    public GameObject startScreenPanel;
    [Tooltip("In-game HUD (objective, stress, status) — hidden on start/end.")]
    public GameObject gameplayPanel;
    [Tooltip("Full-screen panel: win icon + Restart + Exit.")]
    public GameObject endScreenPanel;

    [Header("Start Screen")]
    public Button startButton;
    [Tooltip("Optional loading bar on start screen (0 to 1).")]
    public Slider loadingBar;
    public float minLoadingSeconds = 2f;
    [Tooltip("If true, player must click Start after loading. If false, game begins automatically.")]
    public bool requireStartButton = true;

    [Header("Gameplay")]
    public GameObject player;
    public PlayerMover playerMover;

    private bool gameStarted;
    private bool gameEnded;
    public bool IsGameplayActive => gameStarted && !gameEnded;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p;
        }

        if (playerMover == null && player != null)
            playerMover = player.GetComponent<PlayerMover>();
    }

    void Start()
    {
        ShowStartScreen();
        StartCoroutine(StartLoadingRoutine());
    }

    void ShowStartScreen()
    {
        gameStarted = false;
        gameEnded = false;
        Time.timeScale = 1f;

        SetPanelActive(startScreenPanel, true);
        SetPanelActive(gameplayPanel, false);
        SetPanelActive(endScreenPanel, false);

        SetGameplayEnabled(false);

        if (loadingBar != null)
            loadingBar.value = 0f;

        if (startButton != null)
            startButton.interactable = false;
    }

    IEnumerator StartLoadingRoutine()
    {
        float elapsed = 0f;

        while (elapsed < minLoadingSeconds)
        {
            elapsed += Time.unscaledDeltaTime;

            if (loadingBar != null)
                loadingBar.value = Mathf.Clamp01(elapsed / minLoadingSeconds);

            yield return null;
        }

        if (loadingBar != null)
            loadingBar.value = 1f;

        if (requireStartButton)
        {
            if (startButton != null)
                startButton.interactable = true;
        }
        else
        {
            BeginGame();
        }
    }

    /// <summary>Hook to Start button OnClick().</summary>
    public void OnStartButtonPressed()
    {
        if (gameStarted || gameEnded)
            return;

        BeginGame();
    }

    public void BeginGame()
    {
        gameStarted = true;

        SetPanelActive(startScreenPanel, false);
        SetPanelActive(gameplayPanel, true);
        SetPanelActive(endScreenPanel, false);

        SetGameplayEnabled(true);
    }

    /// <summary>Called from GameManager after win music + Luma's final line.</summary>
    public void ShowEndScreen()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        StartCoroutine(ShowEndScreenRoutine());
    }

    IEnumerator ShowEndScreenRoutine()
    {
        // Wait for Luma's final voice line to finish.
        float waited = 0f;
        while (LumaTTS.Instance != null && LumaTTS.Instance.IsSpeaking && waited < 45f)
        {
            waited += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        SetGameplayEnabled(false);
        SetPanelActive(gameplayPanel, false);
        SetPanelActive(endScreenPanel, true);

        Time.timeScale = 0f;
    }

    /// <summary>Hook to Restart button OnClick().</summary>
    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>Hook to Exit button OnClick().</summary>
    public void OnExitButtonPressed()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void SetGameplayEnabled(bool enabled)
    {
        if (playerMover != null)
            playerMover.enabled = enabled;

        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null && !enabled)
                rb.linearVelocity = Vector3.zero;
        }
    }

    static void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }
}
