using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class AppManager : MonoBehaviour
{
	public static AppManager Instance { get; private set; }

	public AppState currentState;



	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}

	private void Start()
	{

	}

	private void Update()
	{

	}






	public void SetState(string newState)
	{
		string oldName = currentState?.GetType().Name ?? "None";
		Type type = Type.GetType(newState);

		if (type == null)
		{
			Debug.Log($"AppManager: Failed to generate AppState type from {newState}.");
		}
		else
		{
			currentState?.Exit();
			currentState = (AppState)Activator.CreateInstance(type);
			currentState.Enter();
		}

		Debug.Log($"AppManager: Switched from AppState {oldName} to {newState}.");
	}
}
