using System;
using UnityEngine;

public class GroundedChargeAttackState : GroundedState
{
    //Level x state

    //======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
    #region local_fields
    //variables
    protected bool isChargingAttack;
    protected float chargeLevel;
    protected float chargeStartTime;
    protected int remainingFrames;
    protected float elapsedTime;

    //anim params
    protected STDAnimState? chargeState;
    protected STDAnimState? releaseState;
    protected float? maxChargeTime;
    protected int? attackMaxFrameDuration;
    protected int? attackMinFrameDuration;
    protected int? animationOriginalFrameDuration;
    protected float? chargeTimeVsReleaseTimeFactor;

    
    //=//----------------------------------------------------------------//=//
    #endregion local_fields
    /////////////////////////////////////////////////////////////////////////////




    //======// /==/==/==/=||[LOCAL]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
    #region local
    //Functions exlcusive to this member of the state heirarchy
    protected void ReleaseChargeAttack()
    {
        isChargingAttack = false;
        remainingFrames = (int)((attackMaxFrameDuration - ((attackMaxFrameDuration - attackMinFrameDuration) * (maxChargeTime / elapsedTime))) * chargeTimeVsReleaseTimeFactor);
        LogCore.Log("Attacks", "released");
        aapc.PlayInFrames((STDAnimState)releaseState, (int)animationOriginalFrameDuration, remainingFrames);
    }
    //=//----------------------------------------------------------------//=//
    #endregion local
    /////////////////////////////////////////////////////////////////////////////




    //======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======// 
    #region base
    //=//-----|Setup|----------------------------------------------------//=//
    #region setup
    public GroundedChargeAttackState(PerformanceCSM sm, Character character) : base(sm, character)
    {
        exitState = CStateID.OO_GroundedIdle; 
        clearFromQueueOnSetState = false;
        forceClearQueueOnEntry = false;
        priority = 2;
        stateDuration = 0;
        minimumStateDuration = ch.stdMinStateDuration;
        exitOnStateComplete = true;

        isLocomotion = false;
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
        if (ch.characterLookDirection == Vector3.zero && isChargingAttack)
        {
            ReleaseChargeAttack();
        } 
        if (Time.time - chargeStartTime > maxChargeTime && isChargingAttack)
        {
            ReleaseChargeAttack();
        }
    }
    protected override void SetOnEntry()
    {
        base.SetOnEntry();
        //...
        isChargingAttack = true;
        chargeLevel = 0;
        chargeStartTime = Time.time;

    }
    protected override void PerFrame()
    {
        base.PerFrame();
        //...
        if (!isChargingAttack)
        {
            remainingFrames--;
            if (remainingFrames == 0)
            {
                stateComplete = true;
            }
        }
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
        aapc.PlayAnimatorState((STDAnimState)chargeState);
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
        if (isChargingAttack)
        {
            elapsedTime = Time.time - chargeStartTime;
            chargeLevel = Mathf.SmoothStep(0, 1, elapsedTime / (float)maxChargeTime);
            aapc.animator.SetFloat("ChargeLevel", chargeLevel);
        }
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
