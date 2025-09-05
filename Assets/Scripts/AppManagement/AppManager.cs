using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;


public class AppManager : MonoBehaviour
{
	public static AppManager Instance { get; private set; }

	//services
	public SysInputManager SystemInputManager { get; private set; }
	public FixedGameUpdateDriver FixedGameUpdateDriver { get; private set; }

	//appstate
	private string currentLoadedScene;

	public string BootAppState = "HomeMenuAPS";
	public static AppState CurrentState;

	public static bool OpenPauseAllowed = true;


	//Core Scene references
	public GameObject app_canvas;
	public GameObject app_camera;



	public InputActionAsset STD_InputActions; // assign in Inspector



	private void Awake()
	{
		//meta
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject); // Optional: protect against duplicates
			return;
		}
		Instance = this;


		

		//core scene
		app_canvas = GameObject.Find("C_Canvas");
		app_camera = GameObject.Find("C_Camera");


		//services 
		SystemInputManager = new SysInputManager();
		FixedGameUpdateDriver = new FixedGameUpdateDriver();

		//appstate
		SetAppState(BootAppState);





	}



	private void Update()
	{
		FixedGameUpdateDriver.Update();


		SystemInputManager.Update();
		CurrentState?.OnUpdate();
		
	}

	//Called by fixed game update driver. circular calling
	public void FixedGameUpdate()
	{
		SystemInputManager.FixedGameUpdate();
	}


	//scene management
	public static void LoadScene(string sceneName)
	{
		Instance.StartCoroutine(Instance.LoadSceneRoutine(sceneName));
	}

	private IEnumerator LoadSceneRoutine(string newSceneName)
	{
		if (!string.IsNullOrEmpty(currentLoadedScene))
		{
			var unloadOp = SceneManager.UnloadSceneAsync(currentLoadedScene);
			while (!unloadOp.isDone)
				yield return null;
		}

		var loadOp = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
		while (!loadOp.isDone)
			yield return null;

		currentLoadedScene = newSceneName;
	}


	public void SetAppState(string newStateName)
	{
		// Get the Type from the string
		Type newStateType = Type.GetType(newStateName);

		if (newStateType == null)
		{
			LogCore.Log(LogType.AppState, $"Could not generate an AppState with name: {newStateName}");
			return;
		}

		// Create an instance of the AppState
		AppState newState = Activator.CreateInstance(newStateType) as AppState;

		SetAppState(newState);
	}


	//appstate flow
	public void SetAppState(AppState newappstate)
	{
		if (newappstate == null)
		{
			LogCore.Log(LogType.Fatal, $"Tried to set null AppState.");
			return;
		}

		// Transition
		CurrentState?.OnExit();
		CurrentState = newappstate;
		CurrentState?.OnEnter();
	}

}
