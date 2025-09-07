
//----------------------------------------------------------------------------------
// Copyright (2025) NITER
//
// This code is part of the PARK-v6 Unity Framework. It is proprietary software.  
// Unauthorized use, modification, or distribution is not permitted without  
// explicit permission from the owner.  
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterID
{
	None = 0,
	Ric = 1000,
	Storm = 2000,
}

//for local input processing 
public enum CharacterInputType
{
	None,
	AllButtons,
	ButtonsAndSticks
}

public abstract class Character : MonoBehaviour
{
	//======// /==/==/==/=||[FIELDS]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region fields
	//=//-----|General|-----------------------------------------------------------//=//
	#region general
	[Header("Meta")]
	public string CharacterName;
	public string InstanceName;
	public string StandardClassPrefix;
	public CharacterID CharacterID;
	public int CharacterInstanceID;
	public int playerID;
	public bool nonPlayer = false;


	[Header("Debug")]
	public bool debug = true;
	public Transform debugParentTransform;
	public TextMeshPro stateText;
	public TextMeshPro debugTextPlus;

	[Header("Stats")]
	public CharacterStats ucs; //Universal
	public CharacterStats bcs; //Base Character Specific
	public CharacterStats acs; //Active Universal+Base 

	[Header("Time")]
	private int currentFrame = 0;
	#endregion general
	//=//-----|Physics|-----------------------------------------------------------//=//
	#region physics
	[Header("Component Refs")]
	public FP_Body2D FPBody;
	public FP_BoxCollider2D bc2d;
	

	#endregion physics
	//=//-----|State|-------------------------------------------------------------//=//
	#region state
	[Header("State Machine")]
	public CStateMachine csm;
	public string currentStateName;
	public readonly HashSet<int> StateBlacklist = new HashSet<int>(); //for debug and development

	[Header("CSM debug")]
	public int requestQueueSize;
	public bool currStateExitAllowed;
	public bool currStatePriority;
	#endregion state
	//=//-----|Animation|---------------------------------------------------------//=//
	#region animation
	[Header("Animation Refs")]
	public FDAPController FDAP_Controller;

	#endregion animation
	//=//-----|Input|-------------------------------------------------------------//=//
	#region input
	[Header("Input Refs")]
	protected ProcessedInputFrameData CurrentInput;
	#endregion input
	//=//-----|Action Queue|------------------------------------------------------//=//
	#region hitstop

	public bool isHitstopped;
	public int hitstopFramesRemaining;
	public FixVec2 hitstopStoredVelocity;

	#endregion hitstop
	//=//-----|Action Queue|------------------------------------------------------//=//
	#region action_queue
	[Header("Action Queue")]
	private readonly Queue<(int frame, Action action)> actionQueue = new();
	private readonly Queue<(int frame, Action<object> action, object param)> paramActionQueue = new();
	#endregion action_queue
	//=//-----|Gameplay Data|-----------------------------------------------------//=//
	#region gameplay_data
	[Header("Character Dimensions")]
	public Fix64 characterHeight;

	[Header("Movement Variables")]
	public Fix64 characterSpeed;
	public Fix64 velocityX;
	public Fix64 velocityY;

	[Header("Ground Checking Variables")]
	public LayerMask groundLayer;
	public bool isGrounded;
	public bool isGroundedByState;
	public bool onGrounding;
	public bool onUngrounding;
	public Fix64 distanceToGround;
	public Fix64 lastGroundedCheckTime = Fix64.Zero;
	public Fix64 timeSinceLastGrounding = Fix64.Zero;

	[Header("HandleNaturalRotation Variables")]
	public bool facingRight;

	[Header("Jump Variables")]
	public float lastJumpTime;
	public int jumpCount;
	public float jumpForceLerp;

	[Header("Physics Variables")]
	public FixVec2 position;
	public FixVec2 velocity;
	public FixVec2 appliedForce = FixVec2.zero;
	public FixVec2 appliedImpulseForce = FixVec2.zero;
	#endregion gameplay_data
	//=//-------------------------------------------------------------------------//=//
	#endregion fields
	/////////////////////////////////////////////////////////////////////////////////////



	//======// /==/==/==/=||[UPDATES & MONO]||==/==/==/==/==/==/==/==/==/ //======//
	#region update_calls
	public void FixedPhysicsUpdate()
	{
		LogCore.Log(LogType.General, "HELLO?");
		csm.CurrentState.FixedPhysicsUpdate();
		UpdateCharacterData();
	}
	public void FixedFrameUpdate()
	{
		csm.CurrentState.FixedFrameUpdate();
		csm.PSMFixedFrameUpdate();
	}
	#endregion update_calls
	//----------------------------------
	#region mono
	private void Awake()
	{
		MonoAwake();
		//...
	}
	private void Start()
	{
		MonoStart();
		//...
	}
	private void OnEnable()
	{
		MonoOnEnable();
		//...
	}
	private void OnDisable()
	{
		MonoOnDisable();
		//...
	}
	private void Update()
	{
		MonoUpdate();
		// ...
	}
	private void FixedUpdate() 
	{
		MonoFixedUpdate();
		//...
	}
	private void LateUpdate()
	{
		MonoLateUpdate();
		//...
	}
	#endregion mono
	//----------------------------------------
	#region mono_virtuals
	protected virtual void MonoAwake() { }
	protected virtual void MonoStart() { }
	protected virtual void MonoOnEnable() { }
	protected virtual void MonoOnDisable() { }
	protected virtual void MonoUpdate() { }
	protected virtual void MonoFixedUpdate() { }
	protected virtual void MonoLateUpdate() { }
	#endregion mono_virtuals 
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[LOCAL]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//=//-----|Data|-------------------------------------------------------------//=//
	#region data
	protected void GetInput()
	{
		AppManager.Instance.SystemInputManager.

	}
	protected void UpdateACS()
	{
		// ucd + bcd = acd

		if (ucs == null || bcs == null || acs == null)
		{
			Debug.LogError("Error: ucs, bcs, or acs null.");
			return;
		}

		// Get all fields of the ScriptableObject type
		FieldInfo[] fields = typeof(CharacterStats).GetFields(BindingFlags.Public | BindingFlags.Instance);

		foreach (var field in fields)
		{
			// Get the values from the first two objects
			object value1 = field.GetValue(ucs);
			object value2 = field.GetValue(bcs);

			// Handle addition for specific types
			if (value1 is int intValue1 && value2 is int intValue2)
			{
				field.SetValue(acs, intValue1 + intValue2);
			}
			else if (value1 is float floatValue1 && value2 is float floatValue2)
			{
				field.SetValue(acs, floatValue1 + floatValue2);
			}
			else if (value1 is Vector3 vectorValue1 && value2 is Vector3 vectorValue2)
			{
				field.SetValue(acs, vectorValue1 + vectorValue2);
			}
		}
	}
	#endregion data
	//=//-----|Action Queue|-----------------------------------------------------//=//
	#region action_queue
	private void ProcessActionQueue()
	{
		currentFrame++;

		// Execute non-param actions
		while (actionQueue.Count > 0 && actionQueue.Peek().frame <= currentFrame)
		{
			var (_, action) = actionQueue.Dequeue();
			action?.Invoke();
		}
		// Execute param actions
		while (paramActionQueue.Count > 0 && paramActionQueue.Peek().frame <= currentFrame)
		{
			var (_, action, param) = paramActionQueue.Dequeue();
			action?.Invoke(param);
		}
	}
	public void ScheduleAction(int framesFromNow, Action action)
	{
		if (framesFromNow <= 0)
		{
			action?.Invoke();
			return;
		}

		actionQueue.Enqueue((currentFrame + framesFromNow, action));
	}
	public void ScheduleAction<T>(int framesFromNow, Action<T> action, T param)
	{
		if (framesFromNow <= 0)
		{
			action?.Invoke(param);
			return;
		}

		paramActionQueue.Enqueue((currentFrame + framesFromNow, (p) => action((T)p), param));
	}
	#endregion action_queue
	//=//-----|csm|--------------------------------------------------------------//=//
	#region csm
	/// <summary>
	/// For pushing states from states
	/// </summary>
	public void StatePushState(int? stateID, int pushForce, int frameLifetime)
	{
		csm.PushState((int)stateID, pushForce, frameLifetime);
	}
	private void CharacterPushState(int? stateID, int pushForce, int frameLifetime)
	{
		csm.PushState((int)stateID, pushForce, frameLifetime);
	}
	public void CSMDebugUpdate()
	{
		requestQueueSize = csm.RequestQueue.Count;
		currStateExitAllowed = csm.CurrentState.IsExitAllowed() == true;
	}
	#endregion csm
	//=//-----|Physics|----------------------------------------------------------//=//
	#region physics
	private void HandleRegularForce()
	{
		if (!isHitstopped)
		{
			FPBody.AddForce(appliedForce);
		}

		appliedForce = FixVec2.zero;
	}
	private void HandleImpulseForce()
	{
		//rigidBody.AddForce(appliedImpulseForce, ForceMode.Impulse);
		appliedImpulseForce = FixVec2.zero;
	}
	#endregion physics
	//=//-----|Hitstop|----------------------------------------------------------//=//
	#region hitstop
	public void EnableHitstop()
	{
		if (isHitstopped)
		{
			return;
		}
		isHitstopped = true;

		//hitstopStoredVelocity = rigidBody.linearVelocity;
		//rigidBody.linearVelocity = Vector3.zero;
		//rigidBody.isKinematic = true;

	}
	public void DisableHitstop()
	{
		if (!isHitstopped)
		{
			return;
		}
		isHitstopped = false;

		//rigidBody.isKinematic = false;
		//rigidBody.linearVelocity = hitstopStoredVelocity;
		hitstopStoredVelocity = FixVec2.zero;
	}
	public void AddHitstop(int frames)
	{
		if (frames <= 0)
		{
			return;
		}
		hitstopFramesRemaining += frames;

	}

	//Per fixed frame update
	protected void ProcessHitstop()
	{
		if (hitstopFramesRemaining <= 0)
		{
			DisableHitstop();
			hitstopFramesRemaining = 0;
		}
		else
		{
			EnableHitstop();
			hitstopFramesRemaining--;
		}
	}


	#endregion hitstop
	//=//------------------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region base 
	//Base methods and their helpers 
	//=//-----|Setup|------------------------------------------------------------//=//
	#region setup
	protected virtual void CharacterSetup()
	{
		//Local init functions
		SetMemberVariables();
		SetReferences();

		//The rest
		RegisterCommands();
		//GlobalData.characterInitialized = true;	//HACK: do we need this?

		//magic numbers
		UpdateACS();

		//wiring data (runs in update)
		UpdateCharacterData();

		//animation 
		FDAP_Controller = new FDAPController(this);
		FDAP_Controller.Setup();


		//LAST
		//state
		csm = new CStateMachine(this); //special init proc

		LogCore.Log(LogType.Character, $"Character initialized: {InstanceName}");

		//post setup
		if (csm.Verified)
		{
			CharacterPushState(CStateIDs.Suspended, 9, 9);
		}
	}
	protected virtual void RegisterCommands()
	{
		//if (GlobalData.characterInitialized)
		//{
		//	//register commands here (?)

		//}
	}
	protected virtual void SetMemberVariables()
	{
		//meta
		this.StandardClassPrefix = GetType().Name;
		this.InstanceName = StandardClassPrefix + "_1"; //HACK: hack solution for character instance name 
		this.CharacterName = acs.characterName;
	}
	protected virtual void SetReferences()
	{
		//input
		//playerInput = GetComponent<PlayerInput>(); //HACK: Replace
		//playerInputHandler = GetComponent<PlayerInputHandler>(); //HACK: replacing needed

		//physics
		//rigidBody = GetComponent<Rigidbody>();
		//capsuleCollider = GetComponent<CapsuleCollider>();
		groundLayer = LayerMask.GetMask("Ground");

		//animation
		//animator = GetComponent<Animator>();

		//debug
		debugParentTransform = transform.Find("Debug");
		stateText = debugParentTransform.Find("CharacterStateText")?.GetComponent<TextMeshPro>();

	}
	#endregion setup
	//=//-----|Data|-------------------------------------------------------------//=//
	#region data
	protected virtual void UpdateCharacterData() //TODO: better name 
	{
		//this.characterHeight = capsuleCollider.height;
	}
	#endregion data
	#endregion base
	/////////////////////////////////////////////////////////////////////////////////////


	//======// /==/==/==/=||[UTILITY]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region utility
	public bool FlipCoin()
	{
		int randomNumber = UnityEngine.Random.Range(0, 100);
		return randomNumber < 50;
	}

	//=//------------------------------------------------------------------------//=//
	#endregion utility 
	/////////////////////////////////////////////////////////////////////////////////////

	//======// /==/==/==/=||[DEBUG]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region debug
	//=//-----|General|----------------------------------------------------------//=//
	#region general
	public virtual void SetDebug(bool isEnabled)
	{
		//set debug
		debug = isEnabled;

		//set other debug components and what not
		debugParentTransform.gameObject.SetActive(debug);
	}
	public virtual string CName(string message)
	{
		return $"{this.GetType().Name}: {message}";
	}
	#endregion general
	//=//-----|State|------------------------------------------------------------//=//
	#region state
	public void DebugOnStateSet()
	{
		currentStateName = csm.GetCurrentState().StateName;

		if (debug && stateText != null)
		{
			// Remove the character class name prefix if it exists
			if (currentStateName.StartsWith(StandardClassPrefix))
			{
				currentStateName = currentStateName.Substring(StandardClassPrefix.Length);
			}

			stateText.text = currentStateName;
		}
	}
	private void UpdateDebugText()
	{
		if (debugTextPlus != null)
		{
			//TODO: idk
		}
	}
	#endregion state
	//=//------------------------------------------------------------------------//=//
	#endregion debug 
	/////////////////////////////////////////////////////////////////////////////////////
}
