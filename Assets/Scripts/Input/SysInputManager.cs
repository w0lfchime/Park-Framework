
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine.InputSystem.Utilities;



public enum SysInputManagerState
{
	Disabled,
	Pairing,
	CursorsOnly,
	CharactersOnly,
}


public class SysInputManager
{
	public delegate void DevicePairedHandler(int playerId, PlayerControllerType type);
	public delegate void DeviceUnpairedHandler(int playerId);

	public event DevicePairedHandler OnDevicePaired;
	public event DeviceUnpairedHandler OnDeviceUnpaired;

	public bool VerboseInputLogging;

	public Dictionary<int, Player> players = new();
	private int nextPlayerId = 1;

	public SysInputManagerState currentState = SysInputManagerState.Disabled;
	public bool recordingInput = true;

	public SysInputManager()
	{
		VerboseInputLogging = false;

		RegisterInputCommands();


		AddPlayer();
	}

	private void RegisterInputCommands()
	{
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

		CommandHandler.RegisterCommand("toggleinputlog", args =>
		{
			VerboseInputLogging = !VerboseInputLogging;

			string state = VerboseInputLogging ? "Enabled" : "Disabled";
			LogCore.Log(LogType.Response, $"{state} verbose input logging.");
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

		// helper command to list all players and their devices
		CommandHandler.RegisterCommand("listplayers", args =>
		{
			if (players.Count == 0)
			{
				LogCore.Log(LogType.Response, "No players exist.");
				return;
			}

			LogCore.Log(LogType.Response, $"Listing {players.Count} players:");
			foreach (var kvp in players)
			{
				int id = kvp.Key;
				var player = kvp.Value;

				string deviceInfo = "Unpaired";
				if (player.Device != null)
				{
					PlayerControllerType type = PlayerControllerType.None;
					if (player.Device is Keyboard) type = PlayerControllerType.Keyboard;
					else if (player.Device is Gamepad) type = PlayerControllerType.Gamepad;

					deviceInfo = $"{player.Device.displayName} ({type})";
				}

				LogCore.Log(LogType.Response, $"- Player {id}: {deviceInfo}");
			}
		});
	}

	// 
	// UPDATE LOOPS
	// 
	public void MonoUpdate()
	{
		foreach (var player in players.Values)
			player.Update();

		if (currentState == SysInputManagerState.Pairing)
			CheckForPairingInput();
	}

	public void FixedGameUpdate() // Called by AppManager at 60Hz
	{
		int frame = AppManager.Instance.FixedGameUpdateDriver.Clock;

		if (recordingInput)
		{
			foreach (var player in players.Values)
				player.RecordFrame(frame);
		}

		if (VerboseInputLogging)
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

		// Only allow keyboards and gamepads
		if (!(device is Keyboard) && !(device is Gamepad))
		{
			LogCore.Log(LogType.Pairing, $"Device {device.displayName} is not supported (only Gamepad or Keyboard).");
			return;
		}

		var iam = AppManager.Instance.STD_InputActions
			.FindActionMap("Character", throwIfNotFound: true)
			.Clone();

		// Restrict to only this player’s device
		iam.devices = new ReadOnlyArray<InputDevice>(new[] { device });

		var source = new InputSource_UnityGamepad(iam);
		players[playerId].AssignInput(device, source);

		PlayerControllerType type = PlayerControllerType.None;
		if (device is Keyboard) type = PlayerControllerType.Keyboard;
		else if (device is Gamepad) type = PlayerControllerType.Gamepad;

		LogCore.Log(LogType.Pairing, $"Paired {device.displayName} ({type}) to Player {playerId}");
		OnDevicePaired?.Invoke(playerId, type);
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

		// Fire event
		OnDeviceUnpaired?.Invoke(playerId);
	}

	public void UnpairAllPlayers()
	{
		foreach (var playerId in players.Keys.ToList())
		{
			if (players[playerId].Device != null)
			{
				UnpairDeviceFromPlayer(playerId);
			}
		}
		LogCore.Log(LogType.Pairing, "Unpaired all players.");
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
			//HACK: simply getting upmost frame. Instead of sampling frame by index. Must fix in the future. 
			InputFrameData input = player.GetFrame();
			if (input.IsEmpty())
			{
				continue;
			}
			string log = $"[InputLog][Frame {frame}][Player {player.Id}] " +
						 $"Buttons: 0x{input.buttons:X4} | " +
						 $"MoveInput: ({input.xAxis}, {input.yAxis}) | " +
						 $"Look: ({input.lookX}, {input.lookY})";
			LogCore.Log(log);
		}
	}
}