using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct InputFrameData
{
	public ushort buttons;   // 16-bit mask
	public sbyte xAxis;      // Move.x
	public sbyte yAxis;      // Move.y
	public sbyte lookX;      // Look.x
	public sbyte lookY;      // Look.y

	public static InputFrameData Empty => new InputFrameData();
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
	private Dictionary<int, SortedDictionary<int, InputFrameData>> inputHistory = new();

	//state, placeholder code
	public SysInputManagerState currentState;

    public bool recordingInput;

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

    public void FixedGameUpdate() //runs at 60hz, gets called by AppManager
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


        if (recordingInput)
        {
            RecordInputs();
        }


	}

	private void RecordInputs()
	{
		int currentFrame = GameClock.CurrentFrame; // You must provide this value somehow. -ChatGPT
		//-my response: the systeminputmanager is persistent across scenes, so I'm trying to figure out 
		//if I should always be clocking frames, or only during a game. Supposedly, we would want to start 
		//clocking frames right as a match starts. 

		//I currently aim to have AppManager function as a state machine on "AppStates", which are their own classes.
		//Appstates will basically hard manage what actually happens, such as loading a level, managing inputs, netcode,
		//setup, etc.etc.etc. 

		//This means that SystemInputManager should remain a system alone that appstates can use in various ways to 
		//make the game happen. 

		//Does that sound like a good plan? If so, I assume we should add various methods like "ResetInputHistory",
		//"GetInputHistory" "EnablingRecording" (im not quite sure yet.) Give me your thoughts.

		foreach (var kvp in inputSources)
		{
			int playerId = kvp.Key;
			IInputSource source = kvp.Value;

			source.UpdateInput(currentFrame); // Sample from Unity Input System or other
			InputFrameData input = source.GetInputForFrame(currentFrame);

			if (!inputHistory.ContainsKey(playerId))
				inputHistory[playerId] = new SortedDictionary<int, InputFrameData>();

			inputHistory[playerId][currentFrame] = input;
		}
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


	public InputFrameData GetInput(int playerId, int frame)
	{
		if (inputHistory.TryGetValue(playerId, out var history) && history.TryGetValue(frame, out var input))
			return input;
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
		foreach (var history in inputHistory.Values)
		{
			var keysToRemove = new List<int>();
			foreach (var key in history.Keys)
			{
				if (key > frame)
					keysToRemove.Add(key);
			}
			foreach (var key in keysToRemove)
			{
				history.Remove(key);
			}
		}
	}

}
