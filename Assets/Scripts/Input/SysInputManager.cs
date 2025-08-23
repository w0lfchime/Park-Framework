
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEditor.PackageManager.Requests;



public enum SysInputManagerState
{
	Disabled,
	Pairing,
	CursorsOnly,
	CharactersOnly,
}


public class SysInputManager
{
	public bool InputLogging;

	private Dictionary<int, Player> players = new();
	private int nextPlayerId = 1;

	public SysInputManagerState currentState = SysInputManagerState.Disabled;
	public bool recordingInput = true;

	public SysInputManager()
	{
		InputLogging = true;

		// commands 
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

		// helper command to pair first device to Player 1
		CommandHandler.RegisterCommand("pairfirst", args =>
		{
			if (InputSystem.devices.Count == 0)
			{
				LogCore.Log(LogType.Pairing, "No input devices available to pair.");
				return;
			}

			var device = InputSystem.devices.First(); // grab the first device in the list
			PairDeviceToPlayer(1, device);            // try to pair it to player 1
		});

		AddPlayer();
	}


	// 
	// UPDATE LOOPS
	// 
	public void Update()
	{
		foreach (var player in players.Values)
			player.Update();

		if (currentState == SysInputManagerState.Pairing)
			CheckForPairingInput();
	}

	public void FixedGameUpdate() // Called by AppManager at 60Hz
	{
		int frame = FixedGameUpdateDriver.Clock;

		if (recordingInput)
		{
			foreach (var player in players.Values)
				player.RecordFrame(frame);
		}

		if (InputLogging)
			DebugLogInputs(frame);
	}

	// 
	// PLAYER MANAGEMENT
	// 
	public Player AddPlayer()
	{
		int playerId = nextPlayerId++;
		var player = new Player(playerId, null, null); // initially unpaired
		players[playerId] = player;

		LogCore.Log(LogType.General, $"Created Player {playerId}");
		return player;
	}

	public void RemovePlayer()
	{
		if (players.Count == 0)
		{
			LogCore.Log(LogType.General, "No players to remove.");
			return;
		}

		// always remove the leftmost (highest? ID)
		int firstId = players.Keys.Max();
		players.Remove(firstId);

		nextPlayerId--;

		LogCore.Log(LogType.General, $"Removed Player {firstId}");
	}

	public Player GetPlayer(int id) =>
		players.TryGetValue(id, out var player) ? player : null;

	public IReadOnlyDictionary<int, Player> GetAllPlayers() => players;

	// 
	// CONTROLLER PAIRING / UNPAIRING
	// 
	public void PairDeviceToPlayer(int playerId, InputDevice device)
	{
		if (!players.ContainsKey(playerId))
		{
			LogCore.Log(LogType.Pairing, $"Cannot pair device, Player {playerId} does not exist.");
			return;
		}
		//HACK: input actions sourcing 

		var iam = AppManager.Instance.STD_InputActions.FindActionMap("Character", throwIfNotFound: true).Clone();

		var source = new InputSource_UnityGamepad(iam);
		players[playerId].AssignInput(device, source);

		LogCore.Log(LogType.Pairing, $"Paired {device.displayName} to Player {playerId}");
	}

	public void UnpairDeviceFromPlayer(int playerId)
	{
		if (!players.ContainsKey(playerId))
		{
			LogCore.Log(LogType.Pairing, $"Cannot unpair device, Player {playerId} does not exist.");
			return;
		}

		players[playerId].ClearInput();
		LogCore.Log(LogType.Pairing, $"Unpaired device from Player {playerId}");
	}

	private void CheckForPairingInput()
	{
		// look for the first unpaired player
		var unpaired = players.Values.FirstOrDefault(p => p.Device == null);
		if (unpaired == null) return;

		foreach (var device in InputSystem.devices)
		{
			if (players.Values.Any(p => p.Device == device))
				continue; // already taken

			foreach (var control in device.allControls)
			{
				if (control is ButtonControl button && button.wasPressedThisFrame)
				{
					PairDeviceToPlayer(unpaired.Id, device);
					return;
				}
			}
		}
	}

	// 
	// STATE MGMT
	// 
	public void EnterPairingMode()
	{
		if (currentState == SysInputManagerState.Pairing) return;
		LogCore.Log(LogType.Pairing, "Entering pairing mode...");
		SetState(SysInputManagerState.Pairing);
	}

	public void ExitPairingMode()
	{
		if (currentState != SysInputManagerState.Pairing) return;
		LogCore.Log(LogType.Pairing, "Exiting pairing mode...");
		SetState(SysInputManagerState.CursorsOnly);
	}

	public void SetState(SysInputManagerState newstate)
	{
		if (newstate == currentState) return;
		currentState = newstate;
	}

	// 
	// DEBUG
	// 
	private void DebugLogInputs(int frame)
	{
		foreach (var player in players.Values)
		{
			InputFrameData input = player.GetFrame(frame);
			if (input.IsEmpty())
			{
				continue;
			}
			string log = $"[InputLog][Frame {frame}][Player {player.Id}] " +
						 $"Buttons: 0x{input.buttons:X4} | " +
						 $"Move: ({input.xAxis}, {input.yAxis}) | " +
						 $"Look: ({input.lookX}, {input.lookY})";
			LogCore.Log(log);
		}
	}
}