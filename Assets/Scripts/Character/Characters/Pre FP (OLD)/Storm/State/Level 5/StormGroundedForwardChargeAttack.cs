//using System;
//using UnityEngine;

//public class StormGroundedForwardChargeAttack : GroundedChargeAttackState
//{
//    //Level x state

//    //======// /==/==/==/=||[LOCAL FIELDS]||==/==/==/==/==/==/==/==/==/ //======//
//    #region local_fields

//    //=//----------------------------------------------------------------//=//
//    #endregion local_fields
//    /////////////////////////////////////////////////////////////////////////////




//    //======// /==/==/==/=||[LOCAL]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
//    #region local
//    //Functions exlcusive to this member of the state heirarchy

//    //=//----------------------------------------------------------------//=//
//    #endregion local
//    /////////////////////////////////////////////////////////////////////////////




//    //======// /==/==/==/=||[BASE]||=/==/==/==/==/==/==/==/==/==/==/==/ //======// 
//    #region base
//    //=//-----|Setup|----------------------------------------------------//=//
//    #region setup
//    public StormGroundedForwardChargeAttack(PerformanceCSM sm, Character character) : base(sm, character)
//    {
//        //...
//        this.chargeState = STDAnimState.GroundedChargeAttackForwardCharge;
//        this.releaseState = STDAnimState.GroundedChargeAttackForwardRelease;
//        maxChargeTime = 2.0f;
//        attackMinFrameDuration = 20;
//        attackMaxFrameDuration = 40;
//        animationOriginalFrameDuration = 20;

//        chargeTimeVsReleaseTimeFactor = 1.0f;
//    }
//    protected override void SetStateReferences()
//    {
//        base.SetStateReferences();
//        //...
//    }
//    public override void SetStateMembers()
//    {
//        base.SetStateMembers();
//        //...
//    }
//    #endregion setup
//    //=//-----|Data Management|------------------------------------------//=//
//    #region data_management
//    protected override void PollInput()
//    {
//        base.PollInput();
//        //...
//    }
//    protected override void SetOnEntry()
//    {
//        base.SetOnEntry();
//        //...
//    }
//    protected override void PerFrame()
//    {
//        base.PerFrame();
//        //...
//    }
//    #endregion data_management
//    //=//-----|Routing|--------------------------------------------------//=//
//    #region routings
//    protected override void HandleStateComplete()
//    {
//        //...
//        base.HandleStateComplete();
//    }
//    protected override void RouteStateFixed()
//    {
//        //...
//        base.RouteStateFixed();
//    }
//    #endregion routing
//    //=//-----|Flow|-----------------------------------------------------//=//
//    #region flow
//    public override void Enter()
//    {
//        base.Enter();
//        //...
//    }
//    public override void Exit()
//    {
//        base.Exit();
//        //...
//    }
//    #endregion flow
//    //=//-----|Mono|-----------------------------------------------------//=//
//    #region mono
//    public override void Update()
//    {
//        base.Update();
//        //...
//    }
//    public override void FixedFrameUpdate()
//    {
//        //...
//        base.FixedFrameUpdate();
//    }
//    public override void FixedPhysicsUpdate()
//    {
//        //...
//        base.FixedPhysicsUpdate();
//    }
//    public override void LateUpdate()
//    {
//        //...
//        base.LateUpdate();
//    }
//    #endregion mono
//    //=//-----|Debug|----------------------------------------------------//=//
//    #region debug
//    public override bool VerifyState()
//    {
//        return base.VerifyState();
//    }
//    #endregion debug
//    //=//----------------------------------------------------------------//=//
//    #endregion base
//    /////////////////////////////////////////////////////////////////////////////




//    //======// /==/==/==/==||[LEVEL 1]||==/==/==/==/==/==/==/==/==/==/ //======//
//    #region level_1
//    //=//----------------------------------------------------------------//=//
//    #endregion level_1
//    /////////////////////////////////////////////////////////////////////////////





//    //======// /==/==/==/==||[LEVEL 2]||==/==/==/==/==/==/==/==/==/==/ //======//
//    #region level_2
//    //=//----------------------------------------------------------------//=//
//    #endregion level_2
//    /////////////////////////////////////////////////////////////////////////////





//    //======// /==/==/==/==||[LEVEL 3]||==/==/==/==/==/==/==/==/==/==/ //======//
//    #region level_3
//    //=//----------------------------------------------------------------//=//
//    #endregion level_3
//    /////////////////////////////////////////////////////////////////////////////





//    //======// /==/==/==/==||[LEVEL 4]||==/==/==/==/==/==/==/==/==/==/ //======//
//    #region level_4
//    //=//----------------------------------------------------------------//=//
//    #endregion level_4
//    /////////////////////////////////////////////////////////////////////////////




//    //======// /==/==/==/==||[LEVEL 5]||==/==/==/==/==/==/==/==/==/==/ //======//
//    #region level_5
//    //=//----------------------------------------------------------------//=//
//    #endregion level_5
//    /////////////////////////////////////////////////////////////////////////////
//}
