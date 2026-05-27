using UnityEngine;

/// <summary>
/// Corrupted area trigger. Increases stress and changes forest mood on entry.
/// Requires a trigger collider (Box Collider recommended).
/// </summary>
public class ShadowZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || GameManager.Instance == null)
            return;

        GameManager.Instance.EnterShadowZone();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || GameManager.Instance == null)
            return;

        GameManager.Instance.ExitShadowZone();
    }
}
