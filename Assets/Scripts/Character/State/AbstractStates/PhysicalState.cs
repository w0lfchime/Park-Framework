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
	public PhysicalState(CStateMachine sm, Character character) : base(sm, character)
	{
		//...
	}
	#endregion Setup
	//=//-----|Data Management|------------------------------------------//=//
	#region Data_Management
	protected override void SetOnEntry()
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
		HandleNaturalRotation();
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
		ch.position = ch.transform.position;

		Vector3 lv = rb.linearVelocity;
		ch.velocity = lv;
		ch.velocityX = lv.x;
		ch.velocityY = lv.y;
		ch.characterSpeed = lv.magnitude;

		// Debug 
		ch.UpdateDebugVector("velocity", lv, Color.green);

	}
	#endregion Data_Management
	//=//-----|Force|---------------------------------------------------//=//
	#region Force
	public virtual void AddForce(string forceName, Vector3 force)
	{
		ch.UpdateDebugVector(forceName, force, Color.yellow);

		ch.appliedForce += force;
	}
	public virtual void AddImpulseForce(string forceName, Vector3 impulseForce)
	{
		ch.StampDebugVector(forceName, impulseForce, Color.red);
		ch.appliedImpulseForce += impulseForce;
	}
	protected virtual void AddForceByTargetVelocity(string forceName, Vector3 targetVelocity, float forceFactor)
	{
		// Debug
		string tvName = $"{forceName}_TargetVelocity";
		ch.UpdateDebugVector(tvName, targetVelocity, Color.white);

		// Force
		Vector3 forceByTargetVelocity = Vector3.zero;
		forceByTargetVelocity += targetVelocity - ch.velocity;
		forceByTargetVelocity *= forceFactor;
		AddForce(forceName, forceByTargetVelocity);
	}
	protected virtual void ApplyGravity()
	{
		Vector3 gravForceVector = Vector3.up * ch.acs.gravityTerminalVelocity;
		AddForce("gravity", gravForceVector);
	}
	#endregion Force
	//=//-----|Grounding|-----------------------------------------------//=//
	#region Grounding
	//protected virtual void WatchGrounding()
	//{
	//	float sphereRadius = cc.radius;
	//	Vector3 capsuleRaycastStart = ch.transform.position + new Vector3(0, sphereRadius + 0.1f, 0);

	//	UnityEngine.Debug.DrawRay(capsuleRaycastStart, Vector3.down * ch.acs.groundCheckingDistance, Color.red);
	//	UnityEngine.Debug.DrawRay(capsuleRaycastStart + new Vector3(0.1f, 0, 0), Vector3.down * ch.acs.isGroundedDistance, Color.blue);

	//	RaycastHit hit;

	//	if (Physics.SphereCast(capsuleRaycastStart, sphereRadius, Vector3.down, out hit, ch.acs.groundCheckingDistance, ch.groundLayer))
	//	{
	//		ch.distanceToGround = hit.distance - sphereRadius;
	//	}
	//	else
	//	{
	//		ch.distanceToGround = ch.acs.groundCheckingDistance;
	//	}
	//}
	//public void SetGrounding()
	//{
	//	bool groundedByDistance = ch.distanceToGround < ch.acs.isGroundedDistance;

	//	if (groundedByDistance != ch.isGrounded)
	//	{
	//		if (Time.time - ch.lastGroundedCheckTime >= ch.acs.groundedSwitchCooldown)
	//		{
	//			ch.isGrounded = groundedByDistance;
	//			ch.lastGroundedCheckTime = Time.time;

	//			// Reset jumps on grounded
	//			if (ch.isGrounded)
	//			{
	//				ch.timeSinceLastGrounding = Time.time;

	//				ch.onGrounding = true;

	//				ch.ScheduleAction((int)onGroundingHoldFrames, () => ch.onGrounding = false);
	//			}
	//			else
	//			{
	//				ch.onUngrounding = true;

	//				ch.ScheduleAction((int)onUngroundingHoldFrames, () => ch.onUngrounding = false);
	//			}
	//		}
	//	}
	//}
	#endregion Grounding
	//=//-----|Rotation|------------------------------------------------//=//
	#region Rotation
	//public void HandleNaturalRotation()
	//{
	//	if (ch.isGrounded)
	//	{
	//		// ch.facingRight = ch.velocityX > 0;
	//	}

	//	if (ch.inputMoveDirection != Vector3.zero)
	//	{
	//		ch.facingRight = ch.inputMoveDirection.x > 0;
	//	}

	//	bool clockwiseRotation = ch.FlipCoin();

	//	Vector3 directionFacing = ch.facingRight ? Vector3.right : Vector3.left;

	//	// Calculate the target rotation
	//	Quaternion targetRotation = Quaternion.LookRotation(directionFacing, Vector3.up);

	//	// Smoothly interpolate the rotation using Slerp
	//	ch.rigAndMeshTransform.rotation = Quaternion.Slerp(
	//		ch.rigAndMeshTransform.rotation,
	//		targetRotation,
	//		Time.deltaTime * ch.acs.rotationSpeed
	//	);
	//}
	#endregion Rotation
	//=//-----|Routes|--------------------------------------------------//=//
	#region Routes
	//protected virtual void HandleGrounding()
	//{
	//	if (ch.onGrounding)
	//	{
	//		ch.jumpCount = 0;
	//		if (!ch.isGroundedByState)
	//		{
	//			StatePushState(CStateIDs.GroundedIdle, (int)priority + 1, 2);
	//		}
	//	}
	//	if (ch.onUngrounding)
	//	{
	//		if (ch.isGroundedByState)
	//		{
	//			StatePushState(CStateIDs.IdleAirborne, (int)priority + 1, 2);
	//		}
	//	}
	//}
	//protected virtual void HandleJump()
	//{
	//	// Assess
	//	bool jumpAllowed = true;
	//	if (ch.jumpCount > ch.acs.maxJumps)
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
	//		ch.jumpCount++;
	//		StatePushState(CStateIDs.Jump, 4, 4);
	//	}
	//}
	#endregion Routes
	//=//----------------------------------------------------------------//=//
	#endregion Level_1
	/////////////////////////////////////////////////////////////////////////////
}
