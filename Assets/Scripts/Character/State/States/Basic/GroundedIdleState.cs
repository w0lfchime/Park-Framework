using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GroundedIdleState : PhysicalState
{

	//======// /==/==/==/=||[FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields

	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public GroundedIdleState(CStateMachine sm) : base(sm)
	{

	}
	public override void SetGenericStateDefinition()
	{
		DefaultExitState = CStateGlobal.Airborne;
		ClearFromQueueOnSetNewState = true;
		ForceClearQueueOnEntry = false;
		DefaultPriority = 5;
		StateDuration = 0;
		ExitOnStateComplete = false;
		MinimumStateDuration = 2;


	}
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	public override void SetOnEntry()
	{
		base.SetOnEntry();
	}
	protected override void PerFrame()
	{
		base.PerFrame();
	}
	#endregion data_management
	//=//-----|Routing|--------------------------------------------------//=//
	protected override void RouteState()
	{
		
	}
	//=//-----|Driver Calls|--------------------------------------//=//
	#region driver_calls
	public override void Enter()
	{
		base.Enter();
	}
	public override void Exit()
	{
		base.Exit();
	}
	public override void FixedPhysicsUpdate() //run first
	{
		base.FixedPhysicsUpdate();
	}
	public override void FixedFrameUpdate() //run second
	{
		base.FixedFrameUpdate();
	}
	#endregion driver_calls
	//=//-----|Unity Mono|----------------------------------------------------//=////HACK: THIS
	#region unity_mono
	//TODO: IF I EVER NEED THIS. I GUESS. LOL

	#endregion unity_mono
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
}
