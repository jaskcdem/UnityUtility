using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> ��J����x </summary>
internal static class InputControler
{
    internal static KeyCode[] UpKey = { KeyCode.UpArrow, KeyCode.W }, DownKey = { KeyCode.DownArrow, KeyCode.S },
         RightKey = { KeyCode.RightArrow, KeyCode.A }, LeftKey = { KeyCode.LeftArrow, KeyCode.D };

    internal const string HorizontalInput = "Horizontal", VerticalInput = "Vertical", MouseXInput = "Mouse X", MouseYInput = "Mouse Y", MouseScrollInput = "Mouse ScrollWheel";

    /// <summary> ���o�����ܶq </summary>
    internal static float GetHorizontalAxis => Input.GetAxis(HorizontalInput);
    /// <summary> ���o�����ܶq </summary>
    internal static float GetVerticalAxis => Input.GetAxis(VerticalInput);
    /// <summary> ���o�����ƹ��ܶq </summary>
    internal static float GetMouseXAxis => Input.GetAxis(MouseXInput);
    /// <summary> ���o�����ƹ��ܶq </summary>
    internal static float GetMouseYAxis => Input.GetAxis(MouseYInput);
    /// <summary> ���o�ƹ��u���ܶq </summary>
    internal static float GetMouseWheelAxis => Input.GetAxis(MouseScrollInput);
}
