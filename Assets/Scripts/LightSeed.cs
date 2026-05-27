using UnityEngine;

/// <summary>
/// Glowing collectible Light Seed. Requires a trigger collider.
/// </summary>
public class LightSeed : MonoBehaviour
{
    [Header("Glow")]
    public float glowPulseSpeed = 2f;
    public float nearbyDistance = 4f;

    private Light seedLight;
    private bool collected;
    private Transform player;

    void Start()
    {
        seedLight = GetComponentInChildren<Light>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (collected)
            return;

        PulseGlow();
        CheckNearby();
    }

    void PulseGlow()
    {
        if (seedLight == null)
            return;

        seedLight.intensity = 1.5f + Mathf.Sin(Time.time * glowPulseSpeed) * 0.5f;
    }

    void CheckNearby()
    {
        if (player == null || GameManager.Instance == null)
            return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= nearbyDistance)
            GameManager.Instance.SetStatus("A Light Seed is nearby.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        collected = true;

        if (GameManager.Instance != null)
            GameManager.Instance.CollectSeed();

        Destroy(gameObject);
    }
}
