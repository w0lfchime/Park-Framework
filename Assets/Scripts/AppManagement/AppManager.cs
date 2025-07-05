using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;


public class AppManager : MonoBehaviour
{
	public static AppManager Instance { get; private set; }


	private string currentLoadedScene;


	public string BootAppState = "HomeMenuAPS";
	public static AppState CurrentState;


	public static bool OpenPauseAllowed = true;


	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject); // Optional: protect against duplicates
			return;
		}

		Instance = this;

		SetAppState(BootAppState);
	}



	private void Update()
	{
		CurrentState?.OnUpdate();
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



	//appstate flow
	public void SetAppState(string newStateName)
	{
		// Get the Type from the string
		Type newStateType = Type.GetType(newStateName);

		if (newStateType == null)
		{
			LogCore.Log("FatalError", $"Could not generate an AppState with name: {newStateName}");
			return;
		}

		// Create an instance of the AppState
		AppState newState = Activator.CreateInstance(newStateType) as AppState;

		if (newState == null)
		{
			LogCore.Log("FatalError", $"Type {newStateName} is not a valid AppState.");
			return;
		}

		// Transition
		CurrentState?.OnExit();
		CurrentState = newState;
		CurrentState?.OnEnter();
	}

}
