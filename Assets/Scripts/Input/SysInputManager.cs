using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InputFrameData
{
	public ushort buttons;   // 16-bit button bitmask
	public sbyte xAxis;
	public sbyte yAxis;
	public sbyte lookX;
	public sbyte lookY;

	public static InputFrameData Empty => new InputFrameData();
}

public enum SysInputManagerState
{
	Disabled,
	Debug,
	Pairing,
	CursorsOnly,
	CharactersOnly,
}

public class SysInputManager
{
	private Dictionary<int, IInputSource> inputSources = new();
	private Dictionary<int, SortedDictionary<int, InputFrameData>> inputHistory = new();

	public SysInputManagerState currentState = SysInputManagerState.Disabled;
	public bool recordingInput = true;

	public void Update()
	{
		// Optional: input detection for pairing or UI actions
	}



	public void ClearHistory()
	{
		foreach (var playerHistory in inputHistory.Values)
		{
			playerHistory.Clear();
		}
	}

	public void ClearAll()
	{
		inputHistory.Clear();
	}



	public void FixedGameUpdate() // Called by AppManager at 60Hz
	{
		int frame = FixedGameUpdateDriver.Clock;

		if (recordingInput)
		{
			RecordInputs(frame);
		}

		switch (currentState)
		{
			case SysInputManagerState.Disabled:
				return;

			case SysInputManagerState.Debug:
				DebugLogInputs(frame);
				break;
			case SysInputManagerState.Pairing:
				// Optional: add pairing logic
				break;

			case SysInputManagerState.CursorsOnly:

				break;
			case SysInputManagerState.CharactersOnly:

				break;
		}
	}




	public void SetState(SysInputManagerState newstate)
	{

		//on exit
		switch (currentState)
		{
			case SysInputManagerState.Disabled:
				return;

			case SysInputManagerState.Debug:

				break;
			case SysInputManagerState.Pairing:

				break;
			case SysInputManagerState.CursorsOnly:

				break;
			case SysInputManagerState.CharactersOnly:

				break;
		}


		//on enter
		switch (newstate)
		{
			case SysInputManagerState.Disabled:
				return;

			case SysInputManagerState.Debug:

				break;
			case SysInputManagerState.Pairing:
				// Optional: add pairing logic
				break;
			case SysInputManagerState.CursorsOnly:

				break;
			case SysInputManagerState.CharactersOnly:

				break;
		}

		currentState = newstate;
	}

	private void RecordInputs(int frame)
	{
		foreach (var kvp in inputSources)
		{
			int playerId = kvp.Key;
			IInputSource source = kvp.Value;

			source.UpdateInput(frame);
			var input = source.GetInputForFrame(frame);

			if (!inputHistory.ContainsKey(playerId))
				inputHistory[playerId] = new SortedDictionary<int, InputFrameData>();

			inputHistory[playerId][frame] = input;
		}
	}

	public void RegisterPlayer(int playerId, IInputSource source)
	{
		inputSources[playerId] = source;
		if (!inputHistory.ContainsKey(playerId))
			inputHistory[playerId] = new SortedDictionary<int, InputFrameData>();
	}

	public InputFrameData GetInput(int playerId, int frame)
	{
		if (inputHistory.TryGetValue(playerId, out var history) &&
			history.TryGetValue(frame, out var input))
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
		foreach (var playerHistory in inputHistory.Values)
		{
			var keysToRemove = new List<int>();
			foreach (var key in playerHistory.Keys)
			{
				if (key > frame)
					keysToRemove.Add(key);
			}

			foreach (var key in keysToRemove)
				playerHistory.Remove(key);
		}
	}





	//debug
	private void DebugLogInputs(int frame)
	{
		foreach (var kvp in inputSources)
		{
			int playerId = kvp.Key;
			InputFrameData input = GetInput(playerId, frame);

			string log = $"[InputLog][Frame {frame}][Player {playerId}] " +
						 $"Buttons: 0x{input.buttons:X4} | " +
						 $"Move: ({input.xAxis}, {input.yAxis}) | " +
						 $"Look: ({input.lookX}, {input.lookY})";

			Debug.Log(log);
		}
	}

}
