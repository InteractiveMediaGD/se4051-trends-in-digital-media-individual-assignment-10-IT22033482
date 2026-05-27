using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Basic WASD / arrow-key movement for the player capsule.
/// Attach to the Player object (requires Rigidbody).
/// </summary>
public class PlayerMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.5f;
    public float acceleration = 18f;
    public float deceleration = 22f;
    public bool useCameraRelativeMovement = true;
    public Transform cameraTransform;
    public float turnSpeed = 14f;
    public float jumpForce = 6f;
    [Tooltip("Extra distance below the collider to still count as grounded.")]
    public float groundCheckDistance = 0.2f;
    [Tooltip("Allows a small jump grace window after leaving the ground.")]
    public float groundedGraceTime = 0.12f;
    public LayerMask groundLayers = ~0;

    private Rigidbody rb;
    private Collider playerCollider;
    private bool jumpQueued;
    private float lastGroundedTime = -999f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        bool spacePressed = InputReaderUtil.WasPressedThisFrame(Key.Space) ||
                            (keyboard != null && keyboard.spaceKey.wasPressedThisFrame);
        if (spacePressed)
            jumpQueued = true;
    }

    void FixedUpdate()
    {
        Vector2 moveAxes = InputReaderUtil.ReadMoveAxes();
        float horizontal = moveAxes.x;
        float vertical = moveAxes.y;

        Vector3 moveDirection = GetMoveDirection(horizontal, vertical);

        Vector3 targetHorizontalVelocity = moveDirection * moveSpeed;
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float accelRate = moveDirection.sqrMagnitude > 0.001f ? acceleration : deceleration;
        Vector3 newHorizontalVelocity = Vector3.MoveTowards(
            currentHorizontalVelocity,
            targetHorizontalVelocity,
            accelRate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(
            newHorizontalVelocity.x,
            rb.linearVelocity.y,
            newHorizontalVelocity.z);

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            Quaternion smoothRotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothRotation);
        }

        if (jumpQueued)
        {
            jumpQueued = false;
            if (CanJump())
            {
                Vector3 current = rb.linearVelocity;
                current.y = 0f;
                rb.linearVelocity = current;
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }
        }
    }

    bool CanJump()
    {
        if (IsGrounded())
            lastGroundedTime = Time.time;

        return Time.time - lastGroundedTime <= groundedGraceTime;
    }

    bool IsGrounded()
    {
        if (playerCollider == null)
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            return Physics.Raycast(origin, Vector3.down, 1f + groundCheckDistance, groundLayers, QueryTriggerInteraction.Ignore);
        }

        Bounds bounds = playerCollider.bounds;
        float radius = Mathf.Max(0.08f, Mathf.Min(bounds.extents.x, bounds.extents.z) * 0.8f);
        Vector3 castOrigin = bounds.center + Vector3.up * 0.05f;
        float distance = bounds.extents.y + groundCheckDistance;

        RaycastHit[] hits = Physics.SphereCastAll(castOrigin, radius, Vector3.down, distance, groundLayers, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider hitCollider = hits[i].collider;
            if (hitCollider == null || hitCollider == playerCollider)
                continue;

            return true;
        }

        return false;
    }

    Vector3 GetMoveDirection(float horizontal, float vertical)
    {
        Vector3 input = new Vector3(horizontal, 0f, vertical);
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        if (!useCameraRelativeMovement)
            return input;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform == null)
            return input;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 worldMove = forward * vertical + right * horizontal;
        if (worldMove.sqrMagnitude > 1f)
            worldMove.Normalize();

        return worldMove;
    }
}
