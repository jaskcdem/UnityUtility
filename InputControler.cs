using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 輸入控制台 </summary>
internal static class InputControler
{
    internal static KeyCode[] UpKey = { KeyCode.UpArrow, KeyCode.W }, DownKey = { KeyCode.DownArrow, KeyCode.S },
         RightKey = { KeyCode.RightArrow, KeyCode.A }, LeftKey = { KeyCode.LeftArrow, KeyCode.D };

    internal const string HorizontalInput = "Horizontal", VerticalInput = "Vertical", MouseXInput = "Mouse X", MouseYInput = "Mouse Y", MouseScrollInput = "Mouse ScrollWheel";

    /// <summary> 取得水平變量 </summary>
    internal static float GetHorizontalAxis => Input.GetAxis(HorizontalInput);
    /// <summary> 取得垂直變量 </summary>
    internal static float GetVerticalAxis => Input.GetAxis(VerticalInput);
    /// <summary> 取得水平滑鼠變量 </summary>
    internal static float GetMouseXAxis => Input.GetAxis(MouseXInput);
    /// <summary> 取得垂直滑鼠變量 </summary>
    internal static float GetMouseYAxis => Input.GetAxis(MouseYInput);
    /// <summary> 取得滑鼠滾輪變量 </summary>
    internal static float GetMouseWheelAxis => Input.GetAxis(MouseScrollInput);
}
