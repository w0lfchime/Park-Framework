using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public abstract class CState
{
	//THE BASE STATE

	//======// /==/==/==/=||[FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	//=//-----|Meta|-----------------------------------------------------------//=//
	#region meta
	//meta
	public CStateMachine StateMachine;
	public Character Ch;
	
	public string StateName;
	public int? StateID;
	#endregion meta
	//=//-----|State Definition|-----------------------------------------------//=//
	#region state_definiton



	public int? DefaultExitState;
	public bool? ClearFromQueueOnSetNewState;
	public bool? ForceClearQueueOnEntry;
	public int? DefaultPriority;
	//TODO: incomplete / complete priorities? 
	public int? CurrentPriority;

	//Duration Definition
	public int? StateDuration; //0, if cyclic or indefinite
	public bool? ExitOnStateComplete;
	public int? MinimumStateDuration;
	public int? CurrentFrame;

	//Duration Variables
	public bool? StateComplete; //will cause state to push DefaultExitState if ExitOnStateComplete is true. 


	#endregion state_definition
	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public CState(CStateMachine sm)
	{
		this.StateMachine = sm;
		
		Ch = sm.MachineOwner;
		string fullname = GetType().Name;
		StateName = fullname.Substring(0, fullname.Length - 5); //Get rid of "state" at the end
		StateID = CStateIDs.GetStateId(StateName);
		LogCore.Log(LogType.CSM_Setup, "State created: " + StateID + " " + StateName);
	}
	public abstract void SetGenericStateDefinition();
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	public virtual void SetOnEntry()
	{
		CurrentFrame = 0;
		CurrentPriority = DefaultPriority;
		StateComplete = false;
	}
	protected virtual void PerFrame()
	{
		CurrentFrame++;
		if (StateDuration > 0 && CurrentFrame > StateDuration)
		{
			StateComplete = true;
		}

		if (CurrentFrame > MinimumStateDuration)
		{
			RouteState();
		}
		
	}
	#endregion data_management
	//=//-----|Routing|--------------------------------------------------//=//
	#region routing
	public int GetPriority()
	{
		return (int)CurrentPriority;
	}
	public bool IsExitAllowed()
	{
		bool exitAllowed = CurrentFrame > MinimumStateDuration;

		//...

		return exitAllowed;
	}
	public void HandleStateComplete()
	{
		if (ExitOnStateComplete == true && StateComplete == true)
		{
			StatePushState(DefaultExitState, (int)DefaultPriority + 1, 2);
		}
	}
	protected void StatePushState(int? stateID, int pushPriority, int lifeTime)
	{
		Ch.CharacterPushState(stateID, pushPriority, lifeTime);
	}
	protected void StatePushState(int? stateID)
	{
		Ch.CharacterPushState(stateID, 5, 5);
	}
	protected virtual void RouteState()
	{

	}
	#endregion routing
	//=//-----|Driver Calls|--------------------------------------//=//
	#region driver_calls
	public virtual void Enter()
	{
		LogCore.Log(LogType.CSM_Flow, $"Entering State {StateName}.");
		SetOnEntry();
		//...
	}
	public virtual void Exit()
	{
		LogCore.Log(LogType.CSM_Flow, $"Exting State {StateName}.");
		//...
	}
	public virtual void FixedPhysicsUpdate() //run first
	{

	}
	public virtual void FixedFrameUpdate() //run second
	{
		PerFrame();

		HandleStateComplete();
		//...

	}
	#endregion driver_calls
	//=//-----|Unity Mono|----------------------------------------------------//=////HACK: THIS
	#region unity_mono
	//TODO: IF I EVER NEED THIS. I GUESS. LOL
	public virtual void Update() { }
	public virtual void LateUpdate() { }
	public virtual void FixedUpdate() { }
	#endregion unity_mono
	//=//-----|Debug|----------------------------------------------------//=//
	#region debug
	public virtual bool VerifyState()
	{
		return CheckStateForNullFields();
	}
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
            LogCore.Log(LogType.Fatal, $"Check FAILED: {unsetFields.Count} {failedMessage}");
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
    #endregion base
    /////////////////////////////////////////////////////////////////////////////
}
