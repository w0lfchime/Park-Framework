//----------------------------------------------------------------------------------
// CStateMachine.cs
//
// Copyright (2025) NITER
//
// This code is part of the PARK-v6 Unity Framework. It is proprietary software.  
// Unauthorized use, modification, or distribution is not permitted without  
// explicit permission from the owner.  
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

public struct SetStateRequest
{
	public int StateID;
	public int PushForce;
	public bool ClearOnSetState;

	public SetStateRequest(int stateID, int pushForce, bool clearOnStateSwitch)
	{
		StateID = stateID;
		PushForce = pushForce;
		ClearOnSetState = clearOnStateSwitch;
	}

	public override bool Equals(object obj)
	{
		if (obj is SetStateRequest other)
		{
			return StateID == other.StateID &&
				   PushForce == other.PushForce &&
				   ClearOnSetState == other.ClearOnSetState;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(StateID, PushForce, ClearOnSetState);
	}
}

public class CStateMachine
{
	//Character State Machine
	public Character MachineOwner;
	public string MachineName;
	public int StateCount;
	public bool Debug = true;
	public bool Verified = false;

	public int CurrentStateID;
	public int PreviousStateID;
	public CState CurrentState;
	protected Dictionary<int, CState> StateDict;
	public RequestQueue RequestQueue = new();

	public int PerFrameProcessLimit = 10;

	public CStateMachine(Character owner)
	{
		this.CurrentStateID = CStateIDs.Null;
		this.MachineOwner = owner;

		StateDict = new Dictionary<int, CState>();

		MachineName = $"{MachineOwner.InstanceName} SM";

		RegisterStates();
		VerifyStates();
	}

	protected virtual void RegisterStates()
	{
		string stateOwnerClassName = MachineOwner.StandardClassPrefix;
		int ownerCharacterID = MachineOwner.CharacterID;

		LogCore.Log(LogType.CSM_Setup, $"Registering states for {MachineOwner.InstanceName}.");

		for (int i = 0; i < 999 + ownerCharacterID; i++)
		{
			if (i == 999)
			{
				i = ownerCharacterID;
			}
			if (MachineOwner.StateBlacklist.Contains(i)) continue;
			string stateName = CStateIDs.GetStateName(i);
			if (stateName == null) continue;
			if (stateName == "Null") continue;

			string stateClassName = stateOwnerClassName + stateName;

			LogCore.Log(LogType.CSM_Setup, $"Attempting to create state {stateClassName}.");

			Type stateClass = Type.GetType(stateClassName);
			if (stateClass == null)
			{
				//No character specific override found, create a generic state. 
				stateClassName = stateName + "State";
				LogCore.Log(LogType.CSM_Setup, $"No character specific override found. Creating generic state as {stateClassName}.");
				stateClass = Type.GetType(stateClassName);

				if (stateClass == null)
				{
					LogCore.Log(LogType.CSM_Setup, $"Fatal: No generic state exists as {stateClassName}.");
					DebugCore.StopGame();
				}
			}

			var stateInstance = (CState)Activator.CreateInstance(stateClass, this);
			SetStateDictState(i, stateInstance);

		}
	}

	public virtual void VerifyStates()
	{
		bool passed = true;

		foreach (var kvp in StateDict)
		{
			int id = kvp.Key;
			var state = kvp.Value;

			if (state == null)
			{
				LogCore.Log(LogType.CSM_Error, $"State at key {id} of {MachineName} state dict is null.");
				passed = false;
			}
			else if (!state.VerifyState())
			{
				LogCore.Log(LogType.CSM_Error, $"State {state.StateName} is invalid.");
				passed = false;
			}
			else
			{
				LogCore.Log(LogType.CSM_Setup, $"Verified state {state.StateName}");
			}
		}

		Verified = passed;

		LogCore.Log(passed ? LogType.CSM_Setup : LogType.CSM_Error,
			passed ? "Successfully verified all registered character states."
				   : "Failed to verify all registered character states.");

		if (!passed)
		{
			DebugCore.StopGame();
		}
	}


	public CState GetState(int stateID) => StateDict[stateID];

	public CState GetCurrentState() => StateDict[(int)CurrentStateID];

	//sets a state of the state set
	protected void SetStateDictState(int stateID, CState state)
	{
		LogCore.Log(LogType.CSM_Setup, $"Adding state {state.StateName} to {this.MachineName} with ID key {stateID}.");
		StateDict[stateID] = state;
	}

	public void PushState(int stateID, int pushForce, int frameLifetime)
	{
		bool clearQueueOnSetThisState = (bool)GetState(stateID).ClearFromQueueOnSetNewState;
		var newRequest = new SetStateRequest(stateID, pushForce, clearQueueOnSetThisState);

		RequestQueue.Add(newRequest, frameLifetime);

		if (CurrentState == null || newRequest.PushForce > CurrentState.GetPriority())
		{
			SetCurrentState(stateID);
		}
	}

	private void ProcessStateQueue()
	{
		if (RequestQueue.Count == 0 || (CurrentState != null && !CurrentState.IsExitAllowed())) return;

		if (RequestQueue.TryGetHighestPriority(out SetStateRequest bestRequest))
		{
			if (CurrentState == null || bestRequest.PushForce > CurrentState.GetPriority())
			{
				SetCurrentState(bestRequest.StateID);
			}
		}
	}

	private void SetCurrentState(int newStateID)
	{
		PreviousStateID = CurrentStateID;

		CurrentState?.Exit();
		CurrentState = GetState(newStateID);
		CurrentStateID = newStateID;

		if (CurrentState.ForceClearQueueOnEntry == true)
		{
			RequestQueue.Clear();
		}

		RequestQueue.ClearClearOnSetState();
		CurrentState.Enter();

		MachineOwner.OnStateSet();

		LogCore.Log(LogType.CSM_Flow, $"Switched from {PreviousStateID} to {CurrentStateID}");
	}


	public void PSMFixedFrameUpdate()
	{
		RequestQueue.FixedFrameUpdate();
		ProcessStateQueue();
	}
	
}
