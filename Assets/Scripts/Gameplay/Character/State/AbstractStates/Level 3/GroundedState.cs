using System;
using UnityEngine;

public class GroundedState : PhysicalState
{
	//Level x state

	//======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	//=//----|Locomotion|------------------------------------------------//=//

	//Param
	protected int groundedStateAntiFlutter = 5;
	public bool? isLocomotion;

	//Varaible
	protected bool runHold;
	protected bool sneakHold;

	protected float currGLSpeed;
	protected float currGLAccFactor;

	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[LOCAL]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//=//----------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	//Overrides of the base class, performance state.
	#region base
	//=//-----|Setup|----------------------------------------------------//=//
	#region setup
	public GroundedState(PerformanceCSM sm, Character character) : base(sm, character)
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
	}
	#endregion setup
	//=//-----|Data Management|------------------------------------------//=//
	#region data_management
	protected override void ProcessInput()
	{
		base.ProcessInput();
		//...

		runHold = cih.GetButtonHold("Run");
		sneakHold = cih.GetButtonHold("Sneak");
	}
	protected override void SetOnEntry()
	{
		base.SetOnEntry();
        //...
        ch.isGroundedByState = true;
		currGLAccFactor = ch.acs.gAccFactor;
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
		HandleGlData();
		//...
		base.FixedFrameUpdate();
	}
	public override void FixedPhysicsUpdate()
	{
		HandleGLForce();
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
	#region grounded_locomotion
	private void HandleGLForce()
	{
		Vector3 tv = ch.inputMoveDirection;
		tv.y = 0;
		tv *= currGLSpeed;

		AddForceByTargetVelocity("GLForce", tv, currGLAccFactor);
	}
	private void HandleGlData()
	{
		if (isLocomotion == true)
		{
			float overdrive = 1.0f;
			float currentSpeed = Mathf.Abs(ch.velocityX);
			float maxSpeed = ch.acs.gRunSpeed;
			float glSpeed = currentSpeed / maxSpeed;

			if (glSpeed > 0 && glSpeed < 1.0005)
			{
				aapc.animator.SetFloat("GLSpeed", glSpeed);
				aapc.animator.SetFloat("GLOverdrive", overdrive);
			}
			else if (glSpeed > 1.0005)
			{
				//handle overdrive
			}


		}

	}

	//Animation

	private void AnimateGL()
	{
		
	}
	#endregion grounded_locomotion
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
