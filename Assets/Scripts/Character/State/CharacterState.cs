using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public abstract class CharacterState
{
	//THE BASE STATE

	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields

	//meta
	public PerformanceCSM StateMachine;
	public Character Ch;
	
	public string StateName;
	public CStateID? StateID;

	#region state_definiton
	//refs


	public CStateID? DefaultExitState;
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

	//HACK: no is physical. (correct choice?) 


	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////

	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public CharacterState(PerformanceCSM sm)
	{
		this.StateName = GetType().Name;
		this.StateMachine = sm;
		this.Ch = sm.MachineOwner;

		SetOnEntry();

	}
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	protected virtual void SetOnEntry()
	{
		CurrentFrame = 0;
	}
	protected virtual void PollInput()
	{
		//HACK: LOL
	}
	protected virtual void PerFrame()
	{
		CurrentFrame++;


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
	protected void StatePushState(CStateID? stateID, int pushPriority, int lifeTime)
	{
		Ch.StatePushState(stateID, pushPriority, lifeTime);
	}
	#endregion routing
	//=//-----|Wrapper Events/Mono|--------------------------------------//=//
	#region mono

	#region core_csm_mono
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
	#endregion core_csm_mono


	#region unity_mono
	//TODO: physicX for environmental chaos and destruction ?
	public virtual void Update()
	{
		//PollInput(); //HACK: idk...
	}
	public virtual void LateUpdate() { }
	public virtual void FixedUpdate() { }
	#endregion unity_mono



	#endregion mono



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
