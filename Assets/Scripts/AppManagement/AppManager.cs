using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;


public class AppManager : MonoBehaviour
{
	public static AppManager Instance { get; private set; }


	private string currentLoadedScene;


	public string BootAppState;
	private AppState currentState;

	private void Awake()
	{




	}

	private void Update()
	{
		currentState?.OnUpdate();
	}

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

	public void SetAppState(String newState)
	{

		System.Type newStateType = System.Type.GetType(BootAppState);

		if (newStateType == null)
		{
			LogCore.Log("FatalError", $"Could not generate an AppState with {newState}.");
		}



		currentState?.OnExit();
		currentState = newState;
		currentState?.OnEnter();
	}
}
