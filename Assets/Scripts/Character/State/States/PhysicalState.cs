using System;
using System.Security.Cryptography;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PhysicalState : CState
{
	// Level 2 State 
	//======// /==/==/==/=||[Local Fields]||==/==/==/==/==/==/==/==/==/ //======//
	#region Local_Fields
	private int? onGroundingHoldFrames = 5;
	private int? onUngroundingHoldFrames = 5;
	//=//----------------------------------------------------------------//=//
	#endregion Local_Fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[Local]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region Local
	//=//----------------------------------------------------------------//=//
	#endregion Local
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[Base]||=/==/==/==/==/==/==/==/==/==/==/==/ //======// Performance State
	// Overrides of the Base Class, CState.
	#region Base
	//=//-----|Setup|----------------------------------------------------//=//
	#region Setup
	public PhysicalState(CStateMachine sm) : base(sm)
	{
		//...
	}
	public override void SetGenericStateDefinition()
	{
		
	}
	#endregion Setup
	//=//-----|Data Management|------------------------------------------//=//
	#region Data_Management
	public override void SetOnEntry()
	{
		base.SetOnEntry();
		//...
		PhysicalDataUpdates(); // Call here to prevent nulls or whatever
	}
	protected override void PerFrame()
	{
		base.PerFrame();
		//...
	}
	#endregion Data_Management
	//=//-----|Routing|--------------------------------------------------//=//
	#region Routings

	#endregion Routing
	//=//-----|Flow|-----------------------------------------------------//=//
	#region Flow
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
	#endregion Flow
	//=//-----|Mono|-----------------------------------------------------//=//
	#region Mono
	public override void Update()
	{
		base.Update();
		//...
		PhysicalDataUpdates();
		//HandleNaturalRotation();
	}
	public override void FixedFrameUpdate()
	{
		//SetGrounding();
		//...
		base.FixedFrameUpdate();
	}
	public override void FixedPhysicsUpdate()
	{
		//WatchGrounding();
		ApplyGravity();
		//...
		base.FixedPhysicsUpdate();
	}
	public override void LateUpdate()
	{
		//...
		base.LateUpdate();
	}
	#endregion Mono
	//=//-----|Debug|----------------------------------------------------//=//
	#region Debug
	public override bool VerifyState()
	{
		return base.VerifyState();
	}
	#endregion Debug
	//=//----------------------------------------------------------------//=//
	#endregion Base
	/////////////////////////////////////////////////////////////////////////////





	//======// /==/==/==/==||[Level 1]||==/==/==/==/==/==/==/==/==/==/ //======// Physical State
	#region Level_1
	//=//-----|Data Management|-----------------------------------------//=//
	#region Data_Management
	protected virtual void PhysicalDataUpdates()
	{
		//ch.FP_Position = ch.transform.FP_Position;

		//Vector3 lv = rb.linearVelocity;
		//ch.FP_Velocity = lv;
		//ch.FP_VelocityX = lv.x;
		//ch.FP_VelocityY = lv.y;
		//ch.FP_CharacterSpeed = lv.magnitude;

		//// Debug 
		//ch.UpdateDebugVector("FP_Velocity", lv, Color.green);

	}
	#endregion Data_Management
	//=//-----|Force|---------------------------------------------------//=//
	#region Force
	public virtual void AddForce(string forceName, Vector3 force)
	{
		//Ch.UpdateDebugVector(forceName, force, Color.yellow);

		//Ch._appliedForce += force;
	}
	public virtual void AddImpulseForce(string forceName, Vector3 impulseForce)
	{
		//Ch.StampDebugVector(forceName, impulseForce, Color.red);
		//Ch._appliedImpulseForce += impulseForce;
	}
	protected virtual void AddForceByTargetVelocity(string forceName, Vector3 targetVelocity, float forceFactor)
	{
		//// Debug
		//string tvName = $"{forceName}_TargetVelocity";
		//Ch.UpdateDebugVector(tvName, targetVelocity, Color.white);

		//// Force
		//Vector3 forceByTargetVelocity = Vector3.zero;
		//forceByTargetVelocity += targetVelocity - Ch.FP_Velocity;
		//forceByTargetVelocity *= forceFactor;
		//AddForce(forceName, forceByTargetVelocity);
	}
	protected virtual void ApplyGravity()
	{
		//Vector3 gravForceVector = Vector3.up * Ch.acs.gravityTerminalVelocity;
		//AddForce("gravity", gravForceVector);
	}
	#endregion Force
	//=//-----|Grounding|-----------------------------------------------//=//
	#region grounding
	protected virtual void WatchGrounding()
	{
		//float sphereRadius = cc.radius;
		//Vector3 capsuleRaycastStart = ch.transform.FP_Position + new Vector3(0, sphereRadius + 0.1f, 0);

		//UnityEngine.Debug.DrawRay(capsuleRaycastStart, Vector3.down * ch.acs.groundCheckingDistance, Color.red);
		//UnityEngine.Debug.DrawRay(capsuleRaycastStart + new Vector3(0.1f, 0, 0), Vector3.down * ch.acs.isGroundedDistance, Color.blue);

		//RaycastHit hit;

		//if (Physics.SphereCast(capsuleRaycastStart, sphereRadius, Vector3.down, out hit, ch.acs.groundCheckingDistance, ch.groundLayer))
		//{
		//	ch.DistanceToGround = hit.distance - sphereRadius;
		//}
		//else
		//{
		//	ch.DistanceToGround = ch.acs.groundCheckingDistance;
		//}
	}
	public void SetGrounding()
	{
		//bool groundedByDistance = ch.DistanceToGround < ch.acs.isGroundedDistance;

		//if (groundedByDistance != ch.IsGrounded)
		//{
		//	if (Time.time - ch.LastGroundedCheckTime >= ch.acs.groundedSwitchCooldown)
		//	{
		//		ch.IsGrounded = groundedByDistance;
		//		ch.LastGroundedCheckTime = Time.time;

		//		// Reset jumps on grounded
		//		if (ch.IsGrounded)
		//		{
		//			ch.TimeSinceLastGrounding = Time.time;

		//			ch.OnGrounding = true;

		//			ch.ScheduleAction((int)onGroundingHoldFrames, () => ch.OnGrounding = false);
		//		}
		//		else
		//		{
		//			ch.OnUngrounding = true;

		//			ch.ScheduleAction((int)onUngroundingHoldFrames, () => ch.OnUngrounding = false);
		//		}
		//	}
		//}
	}
	#endregion grounding
	//=//-----|Rotation|------------------------------------------------//=//
	#region rotation
	public void HandleNaturalRotation()
	{
		//if (ch.IsGrounded)
		//{
		//	// ch.FacingRight = ch.FP_VelocityX > 0;
		//}

		//if (ch.inputMoveDirection != Vector3.zero)
		//{
		//	ch.FacingRight = ch.inputMoveDirection.x > 0;
		//}

		//bool clockwiseRotation = ch.FlipCoin();

		//Vector3 directionFacing = ch.FacingRight ? Vector3.right : Vector3.left;

		//// Calculate the target rotation
		//Quaternion targetRotation = Quaternion.LookRotation(directionFacing, Vector3.up);

		//// Smoothly interpolate the rotation using Slerp
		//ch.rigAndMeshTransform.rotation = Quaternion.Slerp(
		//	ch.rigAndMeshTransform.rotation,
		//	targetRotation,
		//	Time.deltaTime * ch.acs.rotationSpeed
		//);
	}
	#endregion rotation
	//=//-----|Routes|--------------------------------------------------//=//
	#region Routes
	//protected virtual void HandleGrounding()
	//{
	//	if (ch.OnGrounding)
	//	{
	//		ch.JumpCount = 0;
	//		if (!ch.IsGroundedByState)
	//		{
	//			StatePushState(CStateID.GroundedIdle, (int)priority + 1, 2);
	//		}
	//	}
	//	if (ch.OnUngrounding)
	//	{
	//		if (ch.IsGroundedByState)
	//		{
	//			StatePushState(CStateID.Airborne, (int)priority + 1, 2);
	//		}
	//	}
	//}
	//protected virtual void HandleJump()
	//{
	//	// Assess
	//	bool jumpAllowed = true;
	//	if (ch.JumpCount > ch.acs.maxJumps)
	//	{
	//		jumpAllowed = false;
	//	}
	//	if (ch.inputEnabled == false)
	//	{
	//		jumpAllowed = false;
	//	}
	//	// Route 
	//	if (cih.GetButtonDown("jump") && jumpAllowed)
	//	{
	//		ch.JumpCount++;
	//		StatePushState(CStateID.Jump, 4, 4);
	//	}
	//}
	#endregion Routes
	//=//----------------------------------------------------------------//=//
	#endregion Level_1
	/////////////////////////////////////////////////////////////////////////////
}
