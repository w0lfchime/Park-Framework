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
	public string StateName;
	//refs
	public PerformanceCSM StateMachine;
	public Character Ch;


	#region psm
	//Definition
	public CStateID? StateID;
	public CStateID? DefaultExitState;
	public bool? ClearFromQueueOnCharacterSetNewState;
	public bool? ForceClearQueueOnEntry;
	public int? DefaultPriority;
	public int? CurrentPriority;

	//Variables
	#endregion psm

	
	#region duration
	//Duration Definition
	public int? StateDuration; //0, if indefinite
	public bool? ExitOnStateComplete;
	public int? MinimumStateDuration;
	public int? CurrentFrame;

	//Duration Variables
	public bool? StateComplete; //will cause state to push DefaultExitState if ExitOnStateComplete is true. 


	#endregion duration

	public bool? IsPhysical;


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
	protected virtual void ProcessInput()
	{
		//HACK: LOL
	}
	protected virtual void EachFrame()
	{
		CurrentFrame++;

		if (ExitOnStateComplete == true && StateComplete == true)
		{
			StatePushState(DefaultExitState, (int)DefaultPriority + 1, 2);
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

	protected void StatePushState(CStateID? stateID, int pushForce, int lifeTime)
	{
		Ch.StatePushState(stateID, pushForce, lifeTime);
	}
	#endregion routing
	//=//-----|Wrapper Events/Mono|--------------------------------------//=//
	#region mono
	public virtual void Enter()
	{
		LogCore.Log("CSM_Flow", $"Entering State {StateName}.");
		SetOnEntry();
		//...
	}
	public virtual void Exit()
	{
		LogCore.Log("CSM_Flow", $"Exting State {StateName}.");
		//...
	}
	public virtual void FixedFrameUpdate()
	{
		EachFrame();
		//...

	}
	public virtual void Update()
	{
		ProcessInput(); //HACK: idk...
	}
	public virtual void LateUpdate() { }
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
    #endregion base
    /////////////////////////////////////////////////////////////////////////////

}
