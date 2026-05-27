using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Human-centered accessibility: press R to toggle reduced intensity mode.
/// Lowers fog density and reduces wisp pressure via GameManager.
/// </summary>
public class AccessibilityController : MonoBehaviour
{
    public Key toggleKey = Key.R;

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (!InputReaderUtil.WasPressedThisFrame(toggleKey))
            return;

        GameManager.Instance.reducedIntensity = !GameManager.Instance.reducedIntensity;

        if (GameManager.Instance.reducedIntensity)
        {
            RenderSettings.fogDensity *= 0.5f;
            GameManager.Instance.SetStatus("Reduced intensity mode ON — calmer visuals.");
        }
        else
        {
            RenderSettings.fogDensity = GameManager.Instance.isInShadowZone ? 0.08f : 0.02f;
            GameManager.Instance.SetStatus("Reduced intensity mode OFF.");
        }
    }
}
