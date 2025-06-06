using UnityEngine;


[System.Serializable]
public struct InputFrameData
{
    public byte buttons; // Bitmask: Jump = 1 << 0, Attack = 1 << 1, etc.
    public sbyte xAxis;  // Ranged -127 to 127 (fixed-point style)
    public sbyte yAxis;

    public static InputFrameData Empty => new InputFrameData { buttons = 0, xAxis = 0, yAxis = 0 };
}

public class InputManager
{

}
