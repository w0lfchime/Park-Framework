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

public class AAPController
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

    public AAPController(Character character)
    {
        this.owner = character;
        this.animator = owner.animator;
        this.ups = owner.logicUPS;
    }

    // Animator Setup
    private Dictionary<STDAnimState, AnimationClip> animStateData = new Dictionary<STDAnimState, AnimationClip>();

    public void Setup()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("Animator or Animator Controller is missing!");
            return;
        }

        animStateData.Clear();


    }

    public void PlayInFrames(STDAnimState anim, int animFrameDuration, int framesToPlayIn)
    {
        // Ensure valid input
        if (animFrameDuration <= 0 || framesToPlayIn <= 0)
        {
            Debug.LogWarning("Invalid frame duration or frames to play in.");
            return;
        }

        // Calculate the speed factor
        // The animation is animFrameDuration frames long at 30 FPS
        float animationLength = animFrameDuration / 30f; // Convert frame count to seconds
        float targetDuration = framesToPlayIn / 60f; // Convert game logic frames to seconds
        float speed = animationLength > 0 ? animationLength / targetDuration : 1f;

        // Play animation and adjust speed
        PlayAnimatorState(anim);
        animator.speed = speed;
    }

    public void PlayAnimatorState(STDAnimState state)
    {
        animator.speed = 1.0f;
        currentAnimatorState = state;
        animator.CrossFadeInFixedTime(state.ToString(), owner.stdFade);
    }
    public void PlayAnimatorState(STDAnimState state, float cfTime)
    {
        animator.speed = 1.0f;
        currentAnimatorState = state;
        animator.CrossFadeInFixedTime(state.ToString(), cfTime);
    }
    public void AffirmAnimatorState(STDAnimState state)
    {
        if (state == currentAnimatorState)
        {
            return;
        }
        PlayAnimatorState(state);
    }




    //Playables
    public void PlayFrameStepped(AnimationClip clip)
    {
        if (clip == null) return;

        currentClip = clip;
        currentFrame = 0;
        useFrameStepping = true;

        if (!playableGraph.IsValid())
        {
            SetupPlayables();
        }

        clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
        playableOutput.SetSourcePlayable(clipPlayable);
        playableGraph.Play();
    }

    private void SetupPlayables()
    {
        playableGraph = PlayableGraph.Create("FSA_Playables");
        playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
    }

    public void StepFrame(int frames = 1)
    {
        if (!useFrameStepping || currentClip == null) return;

        float frameRate = currentClip.frameRate;
        currentFrame += frames;

        float normalizedTime = (currentFrame / frameRate) / currentClip.length;
        normalizedTime = Mathf.Clamp01(normalizedTime);

        clipPlayable.SetTime(normalizedTime * currentClip.length);
        playableGraph.Evaluate(); // Apply manual frame stepping
    }


    public void Cleanup()
    {
        if (playableGraph.IsValid())
        {
            playableGraph.Destroy();
        }
    }
}
