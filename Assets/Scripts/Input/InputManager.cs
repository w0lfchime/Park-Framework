using System.Collections.Generic;
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
    private Dictionary<int, IInputSource> inputSources = new();
    private Dictionary<int, SortedDictionary<int, InputFrameData>> inputHistory = new();

    public void RegisterPlayer(int playerId, IInputSource source)
    {
        inputSources[playerId] = source;
        inputHistory[playerId] = new SortedDictionary<int, InputFrameData>();
    }

    public void PollInputs(int frame)
    {
        foreach (var kvp in inputSources)
        {
            int playerId = kvp.Key;
            //var input = kvp.Value.GetInputForFrame(frame);
            //inputHistory[playerId][frame] = input;
        }
    }

    public InputFrameData GetInput(int playerId, int frame)
    {
        if (inputHistory.TryGetValue(playerId, out var buffer) && buffer.TryGetValue(frame, out var data))
            return data;
        return InputFrameData.Empty;
    }

    public void SetInput(int playerId, int frame, InputFrameData input)
    {
        if (!inputHistory.ContainsKey(playerId))
            inputHistory[playerId] = new SortedDictionary<int, InputFrameData>();
        inputHistory[playerId][frame] = input;
    }

    public void RollbackToFrame(int frame)
    {
        foreach (var buffer in inputHistory.Values)
        {
            //var keysToRemove = buffer.Keys.Where(f => f >= frame).ToList();
            //foreach (var key in keysToRemove)
            //    buffer.Remove(key);
        }
    }
}
