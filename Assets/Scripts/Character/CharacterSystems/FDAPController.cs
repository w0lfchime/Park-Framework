using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;
using System;


public enum STDAnimState
{
    Other,
    IdleGrounded,
    GroundedLocomotion,
    Jump,
    IdleAirborne,
    NearGrounding,
    OnGrounding,
    GroundedChargeAttackForwardCharge,
    GroundedChargeAttackForwardRelease,
}

/// <summary>
/// The Animations and frame data hub
/// </summary>
public class FDAPController
{
    private Character owner;

    public Animator animator;
    private PlayableGraph playableGraph;
    private AnimationPlayableOutput playableOutput;
    private AnimationClipPlayable clipPlayable;

    private float ups;
    private bool useFrameStepping = false;
    private float currentFrame = 0;
    private AnimationClip currentClip;

    public STDAnimState currentAnimatorState;

    public FDAPController(Character character)
    {
        this.owner = character;
        //this.animator = owner.animator;
        this.ups = FixedGameUpdateDriver.FPS;
    }

    public void Setup()
    {
        LogCore.Log(LogType.Animation_Setup, "Initializing FDAPController...");
    }

}
