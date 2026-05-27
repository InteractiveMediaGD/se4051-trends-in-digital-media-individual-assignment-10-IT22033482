using UnityEngine;
using System.Collections;

/// <summary>
/// Final restoration object. Unlocks after 3 seeds; wins on player touch.
/// On restore: rises, spins, glows brighter, then notifies GameManager.
/// </summary>
public class ForestHeart : MonoBehaviour
{
    [Header("Materials")]
    public Material lockedMaterial;
    public Material unlockedMaterial;
    public Material restoredMaterial;

    [Header("Glow")]
    public Light heartLight;

    [Header("Collision")]
    [Tooltip("If true, ensures a solid collider (blocks player) plus a trigger (win detection).")]
    public bool useSolidAndTriggerColliders = true;
    [Tooltip("Trigger radius multiplier vs solid collider (slightly larger = easier to activate win).")]
    public float triggerRadiusMultiplier = 1.1f;

    [Header("Restore Animation")]
    [Tooltip("How high the heart rises during the win animation.")]
    public float floatRiseHeight = 2.5f;
    [Tooltip("Degrees per second the heart spins during animation.")]
    public float spinSpeed = 180f;
    [Tooltip("Total seconds the float+spin animation plays before the win screen triggers.")]
    public float animationDuration = 3f;
    [Tooltip("How quickly the heart rises upward.")]
    public float riseSpeed = 1.2f;

    public bool IsUnlocked { get; private set; }
    public bool IsRestored { get; private set; }

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (useSolidAndTriggerColliders)
            SetupColliders();
        ApplyLockedLook();
    }

    void SetupColliders()
    {
        SphereCollider solid = null;
        SphereCollider trigger = null;

        foreach (SphereCollider col in GetComponents<SphereCollider>())
        {
            if (col.isTrigger)
                trigger = col;
            else
                solid = col;
        }

        if (solid == null)
        {
            solid = gameObject.AddComponent<SphereCollider>();
            solid.isTrigger = false;
        }

        solid.isTrigger = false;

        if (trigger == null)
        {
            trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
        }

        trigger.isTrigger = true;
        trigger.radius = solid.radius * triggerRadiusMultiplier;
    }

    public void UnlockHeart()
    {
        IsUnlocked = true;
        ApplyUnlockedLook();
    }

    void OnTriggerEnter(Collider other)
    {
        TryInteractWithPlayer(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        TryInteractWithPlayer(collision.collider);
    }

    void TryInteractWithPlayer(Collider other)
    {
        if (!other.CompareTag("Player") || GameManager.Instance == null)
            return;

        if (IsRestored)
            return;

        if (GameManager.Instance.seedCount < 5)
        {
            GameManager.Instance.SetStatus("The Forest Heart needs 5 Light Seeds.");
            return;
        }

        if (!IsUnlocked)
        {
            GameManager.Instance.SetStatus("Collect all Light Seeds first.");
            return;
        }

        RestoreHeart();
    }

    void RestoreHeart()
    {
        IsRestored = true;

        if (rend != null && restoredMaterial != null)
            rend.material = restoredMaterial;

        // Disable solid collider so heart doesn't block player during animation.
        foreach (SphereCollider col in GetComponents<SphereCollider>())
            col.enabled = false;

        StartCoroutine(RestoreAnimation());
    }

    IEnumerator RestoreAnimation()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * floatRiseHeight;
        float startLightIntensity = heartLight != null ? heartLight.intensity : 1f;

        GameManager.Instance?.SetStatus("The Forest Heart is restored!");

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            // Rise smoothly upward.
            transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));

            // Spin around Y axis.
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

            // Pulse light intensity upward.
            if (heartLight != null)
                heartLight.intensity = Mathf.Lerp(startLightIntensity, 8f, t);

            yield return null;
        }

        // Notify win after animation finishes.
        GameManager.Instance?.WinGame();

        if (heartLight != null)
        {
            heartLight.color = new Color(1f, 0.85f, 0.3f);
            heartLight.intensity = 5f;
        }
    }

    void ApplyLockedLook()
    {
        if (rend != null && lockedMaterial != null)
            rend.material = lockedMaterial;

        if (heartLight != null)
            heartLight.intensity = 0.5f;
    }

    void ApplyUnlockedLook()
    {
        if (rend != null && unlockedMaterial != null)
            rend.material = unlockedMaterial;

        if (heartLight != null)
            heartLight.intensity = 1.5f;
    }
}
