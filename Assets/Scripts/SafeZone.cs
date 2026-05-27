using UnityEngine;

/// <summary>
/// Safe resting area where player stress decreases over time.
/// Requires a trigger collider.
/// </summary>
public class SafeZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || GameManager.Instance == null)
            return;

        GameManager.Instance.EnterSafeZone();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || GameManager.Instance == null)
            return;

        GameManager.Instance.ExitSafeZone();
    }
}
