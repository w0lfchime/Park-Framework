using System;
using System.Diagnostics;
using UnityEngine;

public class CharacterState : PerformanceState
{
	//Level 1 Abstract State

	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	[Header("Parent")]
	protected Character ch;

	[Header("Component Refs")]
	protected Rigidbody rb;
	protected CapsuleCollider cc;
	protected PlayerInputHandler cih;
	protected AAPController aapc;


	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////



	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	//Methods from the base class, performance state.
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public CharacterState(PerformanceCSM sm, Character character) : base(sm)
	{
		this.ch = character;

		SetStateReferences();

	}
	protected override void SetStateReferences()
	{
		base.SetStateReferences();

		this.rb = ch.rigidBody;
		this.cc = ch.capsuleCollider;
		this.cih = ch.playerInputHandler;
		this.aapc = ch.aapController;

	}
	public override void SetStateMembers()
	{
		base.SetStateMembers();
		//...
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
	#region routing
	protected override void StatePushState(CStateID? stateID, int pushForce, int lifeTime)
	{
		ch.StatePushState(stateID, pushForce, lifeTime);
	}
	protected override void RouteState()
	{
		if (ch.debug)
		{
			//Flight
			if (Input.GetKeyDown(KeyCode.F))
			{
				if (ch.csm.currentStateID == CStateID.Flight)
				{
					StatePushState(CStateID.OO_IdleAirborne, 99, 2);
				}
				else
				{
					StatePushState(CStateID.Flight, 99, 2);
				}
			}
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
	//=//-----|CustomEvents|---------------------------------------------//=//
	#region custom_events
	protected virtual void OnReceiveHit()
	{

	}
	#endregion custom_events
	//=//----------------------------------------------------------------//=//
	#endregion base
	/////////////////////////////////////////////////////////////////////////////





}
