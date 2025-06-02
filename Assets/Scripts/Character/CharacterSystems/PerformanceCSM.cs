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
	public Character machineOwner;
	public string machineName;
	public int stateCount;
	public bool debug = true;
	public bool verified = false;

	public CStateID currentStateID;
	public CStateID previousStateID;
	public CharacterState currentState;
	protected CharacterState[] stateArray;
	public RequestQueue requestQueue = new();

	public int perFrameProcessLimit = 10;
	private int currentFrame = 0;

	public PerformanceCSM(Character owner)
	{
		this.currentStateID = CStateID.Null;
		this.machineOwner = owner;

		stateCount = Enum.GetValues(typeof(CStateID)).Length;
		stateArray = new CharacterState[stateCount];

		machineName = $"{machineOwner.characterInstanceName} SM";

		RegisterStates();
		VerifyStates();
	}

	protected virtual void RegisterStates()
	{
		string stateOwnerClassName = machineOwner.characterStandardName;

		foreach (CStateID stateID in Enum.GetValues(typeof(CStateID)))
		{
			if (stateID == CStateID.Null) continue;

			string stateClassName = stateOwnerClassName + stateID.ToString();

			Type stateClass = Type.GetType(stateClassName);
			if (stateClass == null)
			{
				//No character specific override found, create a generic state. 
				stateClassName = stateClassName.Substring(stateOwnerClassName.Length);
				stateClass = Type.GetType(stateClassName);

				if (stateClass == null)
				{

				}
			}

			var stateInstance = (CharacterState)Activator.CreateInstance(stateClass, this, machineOwner);
			stateInstance.StateID = stateID;
			stateInstance.SetStateMembers(); //get it in before state verification
			SetStateArrayState(stateID, stateInstance);
		}
	}

	public virtual void VerifyStates()
	{
		bool passed = true;
		for (int i = 1; i < stateArray.Length; i++)
		{
			if (stateArray[i] == null)
			{
				LogCore.Log("CSM_Error", $"Index {i} of {machineOwner.characterInstanceName}'s stateArray is null.");
				passed = false;
			}
			else if (!stateArray[i].VerifyState())
			{
				LogCore.Log("CSM_Error", $"State {stateArray[i].StateName} is invalid.");
				passed = false;
			} 
			else
			{
				LogCore.Log("CSM_Setup", $"Verified state {stateArray[i].StateName}");
			}
		}

		verified = passed;

		LogCore.Log(passed ? "CSM_Setup" : "CSM_Error",
			passed ? "Successfully verified all registered character states."
				   : "Failed to verify all registered character states.");
	}

	public CharacterState GetState(CStateID stateID) => stateArray[(int)stateID];

	public CharacterState GetState() => stateArray[(int)currentStateID];

	protected void SetStateArrayState(CStateID stateID, CharacterState state)
	{
		int index = (int)stateID;
		LogCore.Log("CSM_Setup", $"Setting index {index} of state array to state {state.StateName}.");
		stateArray[index] = state;
	}

	public void PushState(CStateID stateID, int pushForce, int frameLifetime)
	{
		bool coss = (bool)GetState(stateID).ClearFromQueueOnCharacterSetNewState;
		var newRequest = new SetStateRequest(stateID, pushForce, coss);

		requestQueue.Add(newRequest, frameLifetime);

		if (currentState == null || newRequest.PushForce > currentState.GetPriority())
		{
			SetCurrentState(stateID);
		}
	}

	private void ProcessStateQueue()
	{
		if (requestQueue.Count == 0 || (currentState != null && currentState.exitAllowed != true)) return;

		if (requestQueue.TryGetHighestPriority(out SetStateRequest bestRequest))
		{
			if (currentState == null || bestRequest.PushForce > currentState.GetPriority())
			{
				SetCurrentState(bestRequest.StateID);
			}
		}
	}

	private void SetCurrentState(CStateID newStateID)
	{
		previousStateID = currentStateID;

		currentState?.Exit();
		currentState = GetState(newStateID);
		currentStateID = newStateID;

		if (currentState.ForceClearQueueOnEntry == true)
		{
			requestQueue.Clear();
		}

		requestQueue.ClearClearOnSetState();
		currentState.Enter();

		machineOwner.OnStateSet();

		LogCore.Log("PSM_Detail", $"Switched from {previousStateID} to {currentStateID}");
	}

	public void PSMUpdate()
	{
		currentState?.Update();
	}

	public void PSMFixedFrameUpdate()
	{
		currentFrame++;
		requestQueue.FixedFrameUpdate();
		currentState?.FixedFrameUpdate();
		ProcessStateQueue();
	}

	public void PSMFixedPhysicsUpdate()
	{
		currentState?.FixedPhysicsUpdate();
	}

	public void PSMLateUpdate()
	{
		currentState?.LateUpdate();
	}
}
