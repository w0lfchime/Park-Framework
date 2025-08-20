using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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

	//players and pairing 
	private Dictionary<InputDevice, int> deviceToPlayer = new();
	private int nextPlayerId = 1; // Start with Player 1
	private const int MaxPlayers = 4;

	public static event Action<int, InputDevice> OnPlayerPaired;
	public static event Action<int> OnPlayerUnpaired;


	public SysInputManager()
	{
		// Register the command
		CommandHandler.RegisterCommand("listdevices", args =>
		{
			if (InputSystem.devices.Count == 0)
			{
				LogCore.Log(LogType.Response, "No input devices detected.");
				return;
			}

			LogCore.Log(LogType.Response, $"Detected {InputSystem.devices.Count} input devices:");

			foreach (var device in InputSystem.devices)
			{
				LogCore.Log(LogType.Response, $"- {device.displayName} ({device.deviceId}, {device.description.interfaceName})");
			}
		});
	}


	public void Update()
	{
		// Optional: input detection for pairing or UI actions

		foreach (var kvp in inputSources)
		{
			int playerId = kvp.Key;
			IInputSource source = kvp.Value;

			source.Update();
		}


		switch (currentState)
		{
			case SysInputManagerState.Disabled:
				return;

			case SysInputManagerState.Debug:

				break;
			case SysInputManagerState.Pairing:
				CheckForPairingInput();
				break;
			case SysInputManagerState.CursorsOnly:

				break;
			case SysInputManagerState.CharactersOnly:

				break;
		}
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
		if (newstate == currentState)
		{
			return; 
		}


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

				nextPlayerId = 1;
				deviceToPlayer.Clear();
				
				break;
			case SysInputManagerState.CursorsOnly:

				break;
			case SysInputManagerState.CharactersOnly:

				break;
		}

		currentState = newstate;
	}

	private void PairDevice(InputDevice device)
	{
		int playerId = nextPlayerId++;
		deviceToPlayer[device] = playerId;

		IInputSource source = new InputSource_UnityGamepad(new InputActionMap("Player" + playerId));
		RegisterPlayer(playerId, source);

		LogCore.Log(LogType.General, $"Paired {device.displayName} to Player {playerId}");

		OnPlayerPaired?.Invoke(playerId, device); // <-- notify UI
	}

	private void CheckForPairingInput()
	{
		if (nextPlayerId > MaxPlayers) return; // All players paired

		foreach (var device in InputSystem.devices)
		{
			if (deviceToPlayer.ContainsKey(device))
				continue; // Already paired

			// Check if any control on this device is actuated
			foreach (var control in device.allControls)
			{
				if (control is ButtonControl button && button.wasPressedThisFrame)
				{
					PairDevice(device);
					return; // One device per frame is enough
				}
			}
		}
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
