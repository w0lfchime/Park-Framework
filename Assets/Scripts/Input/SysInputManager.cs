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

public enum SysInputManagerState
{
    Disabled,
    Pairing,
    CursorsOnly,
    CharactersOnly,
}

public class SysInputManager
{
    //players up to 4 or 8 total
    private Dictionary<int, IInputSource> inputSources = new();



    //state, dont worry much about this for now
    public SysInputManagerState currentState;


    //recording input
    public bool recordingInput;
    //some sort of data structure of input frame data stuct, per each player, per each frame. (or the most optimal way to record inputs)
    //hoping to use this for rollback, or other netcode related processes, possibly (?)


    public SysInputManager()
    {
        currentState = SysInputManagerState.Disabled;
    }


    public void Update()
    {
        switch (currentState)
        {
            case SysInputManagerState.Disabled:

                break;  
            case SysInputManagerState.Pairing:

                break;
            case SysInputManagerState.CursorsOnly:

                break;
            case SysInputManagerState.CharactersOnly:

                break;
            default:
                break;
        }

        
    }

    public void FixedGameUpdate() //60hz
    {
		switch (currentState)
		{
			case SysInputManagerState.Disabled:

				break;
			case SysInputManagerState.Pairing:

				break;
			case SysInputManagerState.CursorsOnly:

				break;
			case SysInputManagerState.CharactersOnly:
                if (recordingInput)
                {
                    RecordInputs();
                }
				break;
			default:
				break;
		}

	}

    private void RecordInputs()
    {
        //need help with this. 
    }


	public void RegisterPlayer(int playerId, IInputSource source)
    {
        inputSources[playerId] = source;

    }

	public void PollInputs(int frame)
	{
		foreach (var kvp in inputSources)
		{
			int playerId = kvp.Key;
			IInputSource source = kvp.Value;


			var input = source.GetInputForFrame(frame);

		}
	}

    //need help implementing the following methods. 

	public InputFrameData GetInput(int playerId, int frame)
    {

    }

    public void SetInput(int playerId, int frame, InputFrameData input)
    {

    }

    public void RollbackToFrame(int frame)
    {

    }
}
