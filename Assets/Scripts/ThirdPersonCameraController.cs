using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Third-person camera: follows the player, rotates with mouse (hold RMB) or Q/E.
/// Player movement (PlayerMover) uses this camera's facing for WASD direction.
/// Attach to Main Camera.
/// </summary>
public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    [Tooltip("Height on the player to look at (chest/head).")]
    public float lookAtHeight = 1.4f;

    [Header("Orbit")]
    public float distance = 9f;
    public float minDistance = 4f;
    public float maxDistance = 14f;
    [Tooltip("Starting pitch in degrees.")]
    public float defaultPitch = 22f;
    public float minPitch = -15f;
    public float maxPitch = 60f;

    [Header("Smoothing")]
    public float positionSmooth = 12f;
    public float rotationSmooth = 14f;

    [Header("Rotation Input")]
    public float mouseSensitivity = 0.12f;
    public float keyboardYawSpeed = 120f;
    [Tooltip("Hold right mouse button to rotate with mouse.")]
    public bool requireRightMouseToRotate = true;

    private float yaw;
    private float pitch;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }

        pitch = defaultPitch;
        if (target != null)
            yaw = target.eulerAngles.y;
        else
            yaw = transform.eulerAngles.y;

        SnapBehindTarget();
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        if (!IsCameraControlAllowed())
            return;

        HandleRotationInput();

        Vector3 lookPoint = target.position + Vector3.up * lookAtHeight;
        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = lookPoint + orbitRotation * new Vector3(0f, 0f, -distance);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            positionSmooth * Time.deltaTime);

        Quaternion desiredRotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmooth * Time.deltaTime);
    }

    void HandleRotationInput()
    {
        Mouse mouse = Mouse.current;
        bool mouseRotate = false;

        if (mouse != null)
        {
            if (!requireRightMouseToRotate || mouse.rightButton.isPressed)
                mouseRotate = true;
        }

        if (mouseRotate && mouse != null)
        {
            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * mouseSensitivity;
            pitch -= delta.y * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        if (InputReaderUtil.IsPressed(Key.Q))
            yaw -= keyboardYawSpeed * Time.deltaTime;
        if (InputReaderUtil.IsPressed(Key.E))
            yaw += keyboardYawSpeed * Time.deltaTime;

        float scroll = mouse != null ? mouse.scroll.ReadValue().y : 0f;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance -= scroll * 0.01f;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    bool IsCameraControlAllowed()
    {
        if (GameFlowManager.Instance == null)
            return true;

        return GameFlowManager.Instance.IsGameplayActive;
    }

    void SnapBehindTarget()
    {
        if (target == null)
            return;

        Vector3 lookPoint = target.position + Vector3.up * lookAtHeight;
        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = lookPoint + orbitRotation * new Vector3(0f, 0f, -distance);
        transform.rotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
    }
}
