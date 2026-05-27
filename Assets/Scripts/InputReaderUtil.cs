using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Small helper to read keyboard input from the new Input System.
/// </summary>
public static class InputReaderUtil
{
    public static Vector2 ReadMoveAxes()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector2.zero;

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            horizontal -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            horizontal += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            vertical -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            vertical += 1f;

        Vector2 result = new Vector2(horizontal, vertical);
        return result.sqrMagnitude > 1f ? result.normalized : result;
    }

    public static bool IsPressed(Key key)
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard[key].isPressed;
    }

    public static bool WasPressedThisFrame(Key key)
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard[key].wasPressedThisFrame;
    }
}
