//----------------------------------------------------------------------------------
// PerformanceCSM.cs
//
// Copyright (2025) NITER
//
// This code is part of the PARK-v6 Unity Framework. It is proprietary software.  
// Unauthorized use, modification, or distribution is not permitted without  
// explicit permission from the owner.  
//----------------------------------------------------------------------------------

using System;
using UnityEngine;

public struct SetStateRequest
{
	public CStateID StateID;
	public int PushForce;
	public bool ClearOnSetState;

	public SetStateRequest(CStateID stateID, int pushForce, bool clearOnStateSwitch)
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

public class PerformanceCSM
{
	//Character State Machine
	public Character MachineOwner;
	public string MachineName;
	public int StateCount;
	public bool Debug = true;
	public bool Verified = false;

	public CStateID CurrentStateID;
	public CStateID PreviousStateID;
	public CharacterState CurrentState;
	protected CharacterState[] StateSet;
	public RequestQueue RequestQueue = new();

	public int PerFrameProcessLimit = 10;
	private int currentFrame = 0;

	public PerformanceCSM(Character owner)
	{
		this.CurrentStateID = CStateID.Null;
		this.MachineOwner = owner;

		StateCount = Enum.GetValues(typeof(CStateID)).Length;
		StateSet = new CharacterState[StateCount];

		MachineName = $"{MachineOwner.InstanceName} SM";

		RegisterStates();
		VerifyStates();
	}

	protected virtual void RegisterStates()
	{
		string stateOwnerClassName = MachineOwner.StandardClassPrefix;

		LogCore.Log(LogType.CSM_Setup, $"Registering states for {MachineOwner.InstanceName}.");

		foreach (CStateID stateID in Enum.GetValues(typeof(CStateID)))
		{
			if (stateID == CStateID.Null) continue;

			string stateClassName = stateOwnerClassName + stateID.ToString();

			LogCore.Log(LogType.CSM_Setup, $"Attempting to create state {stateClassName}.");

			Type stateClass = Type.GetType(stateClassName);
			if (stateClass == null)
			{
				//No character specific override found, create a generic state. 
				stateClassName = stateID.ToString() + "State";
				LogCore.Log(LogType.CSM_Setup, $"No state override found. Creating generic state as {stateClassName}.");
				stateClass = Type.GetType(stateClassName);

				if (stateClass == null)
				{
					LogCore.Log(LogType.CSM_Setup, $"Fatal: No generic state exists for {stateClassName}.");
					DebugCore.StopGame();
				}
			}

			var stateInstance = (CharacterState)Activator.CreateInstance(stateClass, this, MachineOwner);
			stateInstance.StateID = stateID;
			SetStateArrayState(stateID, stateInstance);
		}
	}

	public virtual void VerifyStates()
	{
		bool passed = true;
		for (int i = 1; i < StateSet.Length; i++)
		{
			if (StateSet[i] == null)
			{
				LogCore.Log(LogType.CSM_Error, $"Index {i} of {MachineOwner.InstanceName}'s stateArray is null.");
				passed = false;
			}
			else if (!StateSet[i].VerifyState())
			{
				LogCore.Log(LogType.CSM_Error, $"State {StateSet[i].StateName} is invalid.");
				passed = false;
			} 
			else
			{
				LogCore.Log(LogType.CSM_Setup, $"Verified state {StateSet[i].StateName}");
			}
		}

		Verified = passed;

		LogCore.Log(passed ? LogType.CSM_Setup : LogType.CSM_Error,
			passed ? "Successfully verified all registered character states."
				   : "Failed to verify all registered character states.");
	}

	public CharacterState GetState(CStateID stateID) => StateSet[(int)stateID];

	public CharacterState GetState() => StateSet[(int)CurrentStateID];

	protected void SetStateArrayState(CStateID stateID, CharacterState state)
	{
		int index = (int)stateID;
		LogCore.Log(LogType.CSM_Setup, $"Setting index {index} of state array to state {state.StateName}.");
		StateSet[index] = state;
	}

	public void PushState(CStateID stateID, int pushForce, int frameLifetime)
	{
		bool clearOnSetThisState = (bool)GetState(stateID).ClearFromQueueOnCharacterSetNewState;
		var newRequest = new SetStateRequest(stateID, pushForce, clearOnSetThisState);

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

	private void SetCurrentState(CStateID newStateID)
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
		currentFrame++;
		RequestQueue.FixedFrameUpdate();
		CurrentState?.FixedFrameUpdate();
		ProcessStateQueue();
	}
	public void PSMUpdate()
	{
		CurrentState?.Update();
	}

	public void PSMLateUpdate()
	{
		CurrentState?.LateUpdate();
	}
}
