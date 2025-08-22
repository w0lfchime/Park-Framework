
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using System.Linq;

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
	private Dictionary<int, Player> players = new();
	private int nextPlayerId = 1;
	private const int MaxPlayers = 4;

	public SysInputManagerState currentState = SysInputManagerState.Disabled;
	public bool recordingInput = true;

	public static event Action<Player> OnPlayerPaired;
	public static event Action<int> OnPlayerUnpaired;

	public SysInputManager()
	{
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
		foreach (var player in players.Values)
			player.Update();

		switch (currentState)
		{
			case SysInputManagerState.Pairing:
				CheckForPairingInput();
				break;
			case SysInputManagerState.Debug:
				break;
		}
	}

	public void FixedGameUpdate() // Called by AppManager at 60Hz
	{
		int frame = FixedGameUpdateDriver.Clock;

		if (recordingInput)
		{
			foreach (var player in players.Values)
				player.RecordFrame(frame);
		}

		if (currentState == SysInputManagerState.Debug)
			DebugLogInputs(frame);
	}

	public void SetState(SysInputManagerState newstate)
	{
		if (newstate == currentState) return;

		if (currentState == SysInputManagerState.Pairing)
		{
			// Reset pairing state when leaving
			nextPlayerId = 1;
			players.Clear();
		}

		currentState = newstate;
	}

	private void PairDevice(InputDevice device)
	{
		if (players.Count >= MaxPlayers) return;

		int playerId = nextPlayerId++;
		var source = new InputSource_UnityGamepad(new InputActionMap("Player" + playerId));
		var player = new Player(playerId, device, source);

		players[playerId] = player;

		LogCore.Log(LogType.General, $"Paired {device.displayName} to Player {playerId}");
		OnPlayerPaired?.Invoke(player);
	}

	private void CheckForPairingInput()
	{
		foreach (var device in InputSystem.devices)
		{
			if (players.Values.Any(p => p.Device == device))
				continue;

			foreach (var control in device.allControls)
			{
				if (control is ButtonControl button && button.wasPressedThisFrame)
				{
					PairDevice(device);
					return;
				}
			}
		}
	}

	public Player GetPlayer(int id) =>
		players.TryGetValue(id, out var player) ? player : null;

	public void UnpairPlayer(int playerId)
	{
		if (players.Remove(playerId))
			OnPlayerUnpaired?.Invoke(playerId);
	}

	private void DebugLogInputs(int frame)
	{
		foreach (var player in players.Values)
		{
			var input = player.GetFrame(frame);
			string log = $"[InputLog][Frame {frame}][Player {player.Id}] " +
						 $"Buttons: 0x{input.buttons:X4} | " +
						 $"Move: ({input.xAxis}, {input.yAxis}) | " +
						 $"Look: ({input.lookX}, {input.lookY})";
			LogCore.Log(log);
		}
	}
}
