using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Shows glowing path markers when stress is high or player presses H.
/// Assign disabled marker objects in the Inspector.
/// </summary>
public class GuidePathController : MonoBehaviour
{
    [Header("Path Markers")]
    public GameObject[] pathMarkers;

    [Header("Settings")]
    public int autoShowStressThreshold = 61;
    public Key toggleKey = Key.H;

    private bool manualToggle;

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (InputReaderUtil.WasPressedThisFrame(toggleKey))
        {
            manualToggle = !manualToggle;
            GameManager.Instance.SetStatus(manualToggle
                ? "Guide path activated (H)."
                : "Guide path hidden.");
        }

        bool shouldShow = manualToggle || GameManager.Instance.stressLevel >= autoShowStressThreshold;
        SetPathVisible(shouldShow);
    }

    void SetPathVisible(bool visible)
    {
        if (pathMarkers == null)
            return;

        foreach (GameObject marker in pathMarkers)
        {
            if (marker != null)
                marker.SetActive(visible);
        }
    }
}
