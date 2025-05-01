using System;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.XR;



public class GroundedIdleState : GroundedState
{
	//Level x state

	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields


	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[LOCAL]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//Functions exlcusive to this member of the state heirarchy

	//=//----------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	//Overrides of the base class, performance state.
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public GroundedIdleState(PerformanceCSM sm, Character character) : base(sm, character)
	{
		//...
	}
	protected override void SetStateReferences()
	{
		base.SetStateReferences();
		//...
	}
	public override void SetStateMembers()
	{
		base.SetStateMembers();
		//...
		exitState = CStateID.OO_GroundedIdle; //clearly, itself
		clearFromQueueOnSetState = true;
		forceClearQueueOnEntry = false;
		priority = 1;
		stateDuration = 0;
		minimumStateDuration = ch.stdMinStateDuration;
		exitOnStateComplete = false;

		isLocomotion = false;
	}
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	protected override void ProcessInput()
	{
		base.ProcessInput();
		//...

	}
	protected override void SetOnEntry()
	{
		base.SetOnEntry();
		//...
		currGLSpeed = 0;
	}
	protected override void PerFrame()
	{
		base.PerFrame();
		//...
	}
	#endregion data_management
	//=//-----|Routing|--------------------------------------------------//=//
	#region routings
	protected override void RouteState()
	{
		//if (ch.characterLookDirection.x > 0)
		//{
		//	StatePushState(CStateID.OO_GroundedForwardChargeAttack, (int)priority + 1, 2);
		//}

		if (ch.inputMoveDirection.x != 0) //HACK: CANT MOVE DETECTION
		{
			if (sneakHold == runHold)
			{
				StatePushState(CStateID.OO_Walk, (int)priority + 1, 2);
			}
			else if (runHold)
			{
				StatePushState(CStateID.OO_Run, (int)priority + 1, 2);
			}
			else if (sneakHold)
			{
				//TODO
			}
		} 
		else
		{
			//Stay here
		}
		//...
		base.RouteState();
	}
	protected override void RouteStateFixed()
	{
		//...
		base.RouteStateFixed();
	}
	#endregion routing
	//=//-----|Flow|-----------------------------------------------------//=//
	#region flow
	public override void Enter()
	{
		base.Enter();
		//...

		//animation
		float playSpeed = 0.0f;
		switch (ch.csm.previousStateID)
		{
			case CStateID.OO_IdleAirborne:
				playSpeed = 0.1f;
				break;
			default:
				playSpeed = ch.stdFade;
				break;
		}
		aapc.PlayAnimatorState(STDAnimState.IdleGrounded, playSpeed);
	}
	public override void Exit()
	{
		base.Exit();
		//...
	}
	#endregion flow
	//=//-----|Mono|-----------------------------------------------------//=//
	#region mono
	public override void Update()
	{
		base.Update();
        //...

    }
	public override void FixedFrameUpdate()
	{

        //...
        base.FixedFrameUpdate();
	}
	public override void FixedPhysicsUpdate()
	{

		//...
		base.FixedPhysicsUpdate();
	}
	public override void LateUpdate()
	{
		//...
		base.LateUpdate();
	}
	#endregion mono
	//=//-----|Debug|----------------------------------------------------//=//
	#region debug
	public override bool VerifyState()
	{
		return base.VerifyState();
	}
    #endregion debug
    //=//----------------------------------------------------------------//=//
    #endregion base
    /////////////////////////////////////////////////////////////////////////////



    //======// /==/==/==/==||[LEVEL 1]||==/==/==/==/==/==/==/==/==/==/ //======//
    #region level_1
    //=//----------------------------------------------------------------//=//
    #endregion level_1
    /////////////////////////////////////////////////////////////////////////////



    //======// /==/==/==/==||[LEVEL 2]||==/==/==/==/==/==/==/==/==/==/ //======//
    #region level_2
    //=//----------------------------------------------------------------//=//
    #endregion level_2
    /////////////////////////////////////////////////////////////////////////////



    //======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//
    #region level_3
    //=//----------------------------------------------------------------//=//
    #endregion level_3
    /////////////////////////////////////////////////////////////////////////////




    //======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
    #region level_4
    //=//----------------------------------------------------------------//=//
    #endregion level_4
    /////////////////////////////////////////////////////////////////////////////




    //======// /==/==/==/==||[LEVEL 5]||==/==/==/==/==/==/==/==/==/==/ //======//
    #region level_5
    //=//----------------------------------------------------------------//=//
    #endregion level_5
    /////////////////////////////////////////////////////////////////////////////
}
