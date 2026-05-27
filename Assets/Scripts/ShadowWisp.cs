using UnityEngine;

/// <summary>
/// Simple enemy that detects and slowly follows the player.
/// Demonstrates intelligent reactive behavior without combat.
/// </summary>
public class ShadowWisp : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Detection")]
    public float detectionRadius = 6f;
    public float followSpeed = 2.5f;
    public float stressRadius = 2.5f;
    public float idleBobSpeed = 1.5f;
    public float idleBobAmount = 0.3f;

    private Vector3 startPos;
    private float stressTick;

    void Start()
    {
        startPos = transform.position;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null)
        {
            IdleFloat();
            return;
        }

        bool playerInSafeZone = GameManager.Instance != null && GameManager.Instance.isInSafeZone;

        // Wisps stop chasing and return to idle when player is in a safe zone.
        if (playerInSafeZone)
        {
            ReturnToStart();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        float speedMultiplier = GameManager.Instance != null
            ? GameManager.Instance.GetWispSpeedMultiplier()
            : 1f;

        if (distance <= detectionRadius)
            ChasePlayer(distance, speedMultiplier);
        else
            IdleFloat();

        if (distance <= stressRadius)
            ApplyStress();
    }

    void ChasePlayer(float distance, float speedMultiplier)
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
            direction.Normalize();

        transform.position += direction * followSpeed * speedMultiplier * Time.deltaTime;

        if (distance <= detectionRadius * 0.8f)
            GameManager.Instance?.SetStatus("A Shadow Wisp has detected you!");
    }

    void ReturnToStart()
    {
        // Drift back toward spawn point while player is safe.
        Vector3 returnTarget = new Vector3(startPos.x, startPos.y, startPos.z);
        transform.position = Vector3.MoveTowards(transform.position, returnTarget, followSpeed * 0.5f * Time.deltaTime);
    }

    void IdleFloat()
    {
        float y = startPos.y + Mathf.Sin(Time.time * idleBobSpeed) * idleBobAmount;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    void ApplyStress()
    {
        if (GameManager.Instance == null || GameManager.Instance.isInSafeZone)
            return;

        stressTick += Time.deltaTime;
        if (stressTick >= 0.5f)
        {
            GameManager.Instance.ChangeStress(2);
            GameManager.Instance.SetStatus("Shadow Wisp is very close!");
            stressTick = 0f;
        }
    }
}
