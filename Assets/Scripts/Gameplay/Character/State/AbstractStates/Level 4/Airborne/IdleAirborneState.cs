using System;
using UnityEngine;

public class IdleAirborneState : AirborneState
{
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
	public IdleAirborneState(PerformanceCSM sm, Character character) : base(sm, character)
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
		exitState = CStateID.OO_IdleAirborne;
		allowDrift = true;
		clearFromQueueOnSetState = true;
		forceClearQueueOnEntry = false;
		priority = 2;
		stateDuration = 0;
		minimumStateDuration = ch.stdMinStateDuration;
		exitOnStateComplete = false;
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

		aapc.PlayAnimatorState(STDAnimState.IdleAirborne);
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
    protected override void WatchGrounding()
    {
        base.WatchGrounding();

		float dtg = ch.distanceToGround / ch.acs.groundCheckingDistance;

		if (dtg != 1 && dtg > 0)
		{
			if (aapc.currentAnimatorState != STDAnimState.NearGrounding)
			{
				aapc.PlayAnimatorState(STDAnimState.NearGrounding, 0.2f);
			}
			aapc.animator.SetFloat("DistanceToGround", dtg);
		}

    }
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
    //=//-----|Flow|-----------------------------------------------------//=//
    //=//----------------------------------------------------------------//=//
    #endregion level_4
    /////////////////////////////////////////////////////////////////////////////
}
