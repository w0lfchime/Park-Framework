
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



//for local input processing 
public abstract class Character : MonoBehaviour
{
	//

	//======// /==/==/==/=||[FIELDS]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region fields
	//=//-----|General|-----------------------------------------------------------//=//
	#region general
	[Header("Meta")]
	public string Name;
	public string InstanceName;
	public string ClassPrefix;
	public CharacterID ID;
	public int InstanceID;
	public int PlayerID;
	public bool NonPlayer = false;


	[Header("Debug")]
	public bool Debug = true;
	public Transform DebugGameObject;
	public TextMeshPro StateTextMesh;

	[Header("Stats")]
	public CharacterStats ucs; //Universal
	public CharacterStats bcs; //Base Character Specific
	public CharacterStats acs; //Active Universal+Base 

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
	public CStateMachine CSM;
	public string CurrentStateName;
	public readonly HashSet<int> StateBlacklist = new HashSet<int>(); //for debug and development

	[Header("CSM Info")]
	public int RequestQueueSize;
	public bool CurrStateExitAllowed;
	public bool CurrStatePriority;
	#endregion state
	//=//-----|Animation|---------------------------------------------------------//=//
	#region animation
	[Header("Animation Refs")]
	public FDAPController FDAP_Controller;

	#endregion animation
	//=//-----|Input|-------------------------------------------------------------//=//
	#region input
	[Header("Input")]
	public ProcessedInputFrameData CurrentInput;
	#endregion input
	//=//-----|Action Queue|------------------------------------------------------//=//
	#region hitstop

	public bool IsHitstopped;
	public int HitstopFramesRemaining;
	public FixVec2 hitstopStoredVelocity;

	#endregion hitstop
	//=//-----|Action Queue|------------------------------------------------------//=//
	#region action_queue
	[Header("Action Queue")]
	public int ActionQueueTicks;
	private readonly Queue<(int frame, Action action)> ActionQueue = new();
	private readonly Queue<(int frame, Action<object> action, object param)> ParamActionQueue = new();
	#endregion action_queue
	//=//-----|Gameplay Data|-----------------------------------------------------//=//
	#region gameplay_data
	[Header("Character Dimensions")]
	public float FP_CharacterHeight;

	[Header("Movement Variables")]
	public Fix64 FP_CharacterSpeed;
	public Fix64 FP_VelocityX;
	public Fix64 FP_VelocityY;

	[Header("Ground Checking Variables")]
	public LayerMask groundLayer;
	public bool IsGrounded;
	public bool IsGroundedByState;
	public bool OnGrounding;
	public bool OnUngrounding;
	public Fix64 DistanceToGround;
	public int LastGroundedCheckTime = 0;
	public int TimeSinceLastGrounding = 0;

	[Header("HandleNaturalRotation Variables")]
	public bool FacingRight;

	[Header("Jump Variables")]
	public float LastJumpTime;
	public int JumpCount;
	public float JumpForceLerp;

	[Header("Physics Variables")]
	public FixVec2 FP_Position;
	public FixVec2 FP_Velocity;
	private FixVec2 _appliedForce = FixVec2.zero;
	private FixVec2 _appliedImpulseForce = FixVec2.zero;
	#endregion gameplay_data
	//=//-------------------------------------------------------------------------//=//
	#endregion fields
	/////////////////////////////////////////////////////////////////////////////////////



	//======// /==/==/==/=||[UPDATES & MONO]||==/==/==/==/==/==/==/==/==/ //======//
	#region update_calls
	public virtual void FixedPhysicsUpdate()
	{
		
		UpdateCharacterData();		
		CSM.CSMFixedPhysicsUpdate();

		ApplyImpulseForces();
		ApplyRegularForces();
		
	}
	public virtual void FixedFrameUpdate()
	{
		DebugStateRotues();
		GetInput();
		CSM.CSMFixedFrameUpdate();
		ProcessActionQueue();
		RenderDebugVectors();
		CSMDebugUpdate();
		
		
	}
	#endregion update_calls
	//----------------------------------
	#region mono
	private void Awake()
	{
		CharacterSetup();
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
		CurrentInput = AppManager.Instance.SystemInputManager.players[this.PlayerID].GetInput();

	}
	protected void UpdateACS()
	{
		// ucd + bcd = acd

		if (ucs == null || bcs == null || acs == null)
		{
			UnityEngine.Debug.LogError("Error: ucs, bcs, or acs null.");
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
		ActionQueueTicks++;

		// Execute non-param actions
		while (ActionQueue.Count > 0 && ActionQueue.Peek().frame <= ActionQueueTicks)
		{
			var (_, action) = ActionQueue.Dequeue();
			action?.Invoke();
		}
		// Execute param actions
		while (ParamActionQueue.Count > 0 && ParamActionQueue.Peek().frame <= ActionQueueTicks)
		{
			var (_, action, param) = ParamActionQueue.Dequeue();
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

		ActionQueue.Enqueue((ActionQueueTicks + framesFromNow, action));
	}
	public void ScheduleAction<T>(int framesFromNow, Action<T> action, T param)
	{
		if (framesFromNow <= 0)
		{
			action?.Invoke(param);
			return;
		}

		ParamActionQueue.Enqueue((ActionQueueTicks + framesFromNow, (p) => action((T)p), param));
	}
	#endregion action_queue
	//=//-----|csm|--------------------------------------------------------------//=//
	#region csm
	/// <summary>
	/// For pushing states from states
	/// </summary>
	public void CharacterPushState(int? stateID, int pushForce, int frameLifetime)
	{
		CSM.PushState((int)stateID, pushForce, frameLifetime);
		DebugOnStateSet();
	}
	public void CSMDebugUpdate()
	{
		RequestQueueSize = CSM.RequestQueue.Count;
		CurrStateExitAllowed = CSM.CurrentState.IsExitAllowed() == true;
	}
	#endregion csm
	//=//-----|Physics|----------------------------------------------------------//=//
	#region physics
	public void AddRegularForce(FixVec2 force)
	{
		_appliedForce += force;
	}
	public void AddImpulseForce(FixVec2 force)
	{
		_appliedImpulseForce += force;
	}
	private void ApplyRegularForces()
	{
		if (!IsHitstopped)
		{
			FPBody.AddForce(_appliedForce);
		}

		_appliedForce = FixVec2.zero;
	}
	private void ApplyImpulseForces()
	{
		//rigidBody.AddForce(_appliedImpulseForce, ForceMode.Impulse);
		_appliedImpulseForce = FixVec2.zero;
	}
	#endregion physics
	//=//-----|Hitstop|----------------------------------------------------------//=//
	#region hitstop
	public void EnableHitstop()
	{
		if (IsHitstopped)
		{
			return;
		}
		IsHitstopped = true;

		//hitstopStoredVelocity = rigidBody.linearVelocity;
		//rigidBody.linearVelocity = Vector3.zero;
		//rigidBody.isKinematic = true;

	}
	public void DisableHitstop()
	{
		if (!IsHitstopped)
		{
			return;
		}
		IsHitstopped = false;

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
		HitstopFramesRemaining += frames;

	}

	//Per fixed frame update
	protected void ProcessHitstop()
	{
		if (HitstopFramesRemaining <= 0)
		{
			DisableHitstop();
			HitstopFramesRemaining = 0;
		}
		else
		{
			EnableHitstop();
			HitstopFramesRemaining--;
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
		string temp = this.GetType().Name;
		LogCore.Log(LogType.Character_Setup, $"Creating new character with class: {temp}");

		//Local init functions
		SetMemberVariables();
		SetReferences();

		//The rest
		RegisterCommands();

		//magic numbers
		UpdateACS();

		//wiring data (runs in update)
		UpdateCharacterData();

		//animation 
		FDAP_Controller = new FDAPController(this);
		FDAP_Controller.Setup();


		//LAST
		//state
		CSM = new CStateMachine(this); //special init proc



		//post setup
		if (CSM.Verified)
		{
			CharacterPushState(CStateID.Suspended, 9, 9);
		}

		LogCore.Log(LogType.Character, $"Character initialized: {InstanceName}");

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
		this.ClassPrefix = GetType().Name;
		this.InstanceName = ClassPrefix + "_1"; //HACK: hack solution for character instance name 
		this.Name = acs.characterName;
	}
	protected virtual void SetReferences()
	{
		groundLayer = LayerMask.GetMask("Ground");

		
		DebugGameObject = transform.Find("Debug");
		StateTextMesh = DebugGameObject.Find("CStateText")?.GetComponent<TextMeshPro>();

	}
	public virtual void EnterGameSpace()
	{
		GetInput();

		LogCore.Log(LogType.GameSpace, $"Character {InstanceName} entering game space with player ID {PlayerID}");

		//...
	}
	#endregion setup
	//=//-----|Data|-------------------------------------------------------------//=//
	#region data
	protected virtual void UpdateCharacterData() //TODO: better name 
	{
		//this.FP_CharacterHeight = capsuleCollider.height;
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
		Debug = isEnabled;

		//set other debug components and what not
		DebugGameObject.gameObject.SetActive(Debug);
	}
	public virtual string AddCharacterNameTo(string message)
	{
		return $"{this.GetType().Name}: {message}";
	}
	public void RenderDebugVectors()
	{
		string vectorName = AddCharacterNameTo("MoveVector");
		FixVec2 vector = CurrentInput.Move;
		Transform vectorLocation = gameObject.transform;

		DebugVectorRenderer.Instance.RenderVector(vectorName, vectorLocation, vector, Color.grey);


		vectorName = AddCharacterNameTo("LookVector");
		vector = CurrentInput.Look;
		Vector3 offset = vectorLocation.position;
		offset.y += FP_CharacterHeight;
		vectorLocation.position = offset;

		DebugVectorRenderer.Instance.RenderVector(vectorName, vectorLocation, vector, Color.white);
	}
	#endregion general
	//=//-----|State|------------------------------------------------------------//=//
	#region state
	private void DebugStateRotues()
	{
		//Flight toggling
		if (Input.GetKeyDown(KeyCode.F))
		{
			if (CSM.CurrentStateID == CStateID.Flight)
			{
				CharacterPushState(CStateID.Airborne, 9, 2);
			}
			else
			{
				CharacterPushState(CStateID.Flight, 9, 2);
			}

		}
	}
	public void DebugOnStateSet()
	{
		CurrentStateName = CSM.GetCurrentState().Name;

		if (Debug && StateTextMesh != null)
		{
			// Remove the character class name prefix if it exists
			if (CurrentStateName.StartsWith(ClassPrefix))
			{
				CurrentStateName = CurrentStateName.Substring(ClassPrefix.Length);
			}

			StateTextMesh.text = CurrentStateName;
		}
	}
	private void UpdateDebugText()
	{

	}
	public void BlacklistAllStates() //applies to state generation 
	{
		for (int i = 0; i <= CStateID.GetHighestGenericStateId(); i++)
		{
			StateBlacklist.Add(i);
		}
	}
	public void WhitelistState(int stateID)
	{
		StateBlacklist.Remove(stateID);
	}
	#endregion state
	//=//------------------------------------------------------------------------//=//
	#endregion debug 
	/////////////////////////////////////////////////////////////////////////////////////
}
