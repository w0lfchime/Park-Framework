using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public abstract class PerformanceState
{
	//THE BASE STATE

	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	//meta
	public string stateName;
	//refs
	public PerformanceCSM stateMachine;
	//Flow Params
	public CStateID? stateID;
	public CStateID? exitState;
	public bool? clearFromQueueOnSetState;
	public bool? forceClearQueueOnEntry;
	public int? priority;
	public int? stateDuration; //0, if indefinite 
	public int? minimumStateDuration; //anti fluttering
	public bool? exitOnStateComplete;
	//Flow variables
	public int? currentFrame;
	public bool? exitAllowed; //overules priority
	public bool? stateComplete;

	//clearOnExitState
	//forceClearStateHeapOnEntry
	//priority
	//stateDuration
	//minimumStateDuration
	//exitOnStateComplete

	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[LOCAL]||==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//Methods for only this class 
	//=//-----|Get & set|------------------------------------------------//=//
	#region get_and_set
	public int GetPriority()
	{
		return (int)priority;
	}
	#endregion get_and_set
	//=//-----|Debug|----------------------------------------------------//=//
	#region debug
	protected bool CheckStateForNullFields()
	{
		bool passed = true;
		Type type = this.GetType();
		List<string> unsetFields = new(); //TODO: setup as fuckin hash key value thing to clarify null locations
		string failedMessage = $" unset field(s) in {type.Name}."; 
		while (type != null && type != typeof(object))
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(this);
				if (value == null)
				{
					passed = false;
					string message = field.Name;
					if (!unsetFields.Contains(message))
					{
						unsetFields.Add(message);
					}

				}
			}
			type = type.BaseType;
		}
		if (passed == false)
		{
			LogCore.Log("CriticalError", $"Check FAILED: {unsetFields.Count} {failedMessage}");
			foreach (string str in unsetFields)
			{
				LogCore.Log(str);
			}
			DebugCore.StopGame();
		}
		return passed;
	}
	#endregion debug
	//=//----------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	//Methods from the base class, performance state.
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public PerformanceState(PerformanceCSM sm)
	{
		this.stateName = GetType().Name;
		this.stateMachine = sm;
		
	}
	protected virtual void SetStateReferences()
	{
		//...
	}
	public virtual void SetStateMembers()
	{
		SetOnEntry();
		//...
	}
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	protected virtual void ProcessInput()
	{
		//...
	}
	protected virtual void SetOnEntry()
	{
		exitAllowed = false;
		currentFrame = 0;
		stateComplete = false;
	}
	protected virtual void PerFrame()
	{
		currentFrame++;

		if (currentFrame <= minimumStateDuration)
		{
			exitAllowed = false;
			stateComplete = false;
		}
		if (currentFrame > minimumStateDuration)
		{
			exitAllowed = true;
		}
		if (stateDuration != 0 && currentFrame >= stateDuration)
		{
			stateComplete = true;
		}

		//...
	}
	#endregion data_management
	//=//-----|Routing|--------------------------------------------------//=//
	#region routing
	protected abstract void StatePushState(CStateID? stateID, int pushForce, int lifetime); //for push state
	protected virtual void RouteState()
	{
		//...
		if (exitOnStateComplete == true && stateComplete == true)
		{
			StatePushState(exitState, (int)priority + 1, 2);
		}
	}
	protected virtual void RouteStateFixed()
	{

	}
	#endregion routing
	//=//-----|Flow|-----------------------------------------------------//=//
	#region flow
	public virtual void Enter()
	{
		LogCore.Log("PSM_Flow", $"Entering State {stateName}.");
		SetOnEntry();
		//...
	}
	public virtual void Exit()
	{
		LogCore.Log("PSM_Flow", $"Exting State {stateName}.");
		//...
	}
	#endregion flow
	//=//-----|Mono|-----------------------------------------------------//=//
	#region mono
	public virtual void Update()
	{
		ProcessInput();
		//...
		RouteState();
	}
	public virtual void FixedFrameUpdate()
	{
		PerFrame();
		//...
		RouteStateFixed();
	}
	public virtual void FixedPhysicsUpdate() { }
	public virtual void LateUpdate() { }
	#endregion mono
	//=//-----|Debug|----------------------------------------------------//=//
	#region debug
	public virtual bool VerifyState()
	{
		return CheckStateForNullFields();
	}
	#endregion debug
	//=//----------------------------------------------------------------//=//
	#endregion base
	/////////////////////////////////////////////////////////////////////////////

}
