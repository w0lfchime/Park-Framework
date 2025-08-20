using System.Collections.Generic;
using UnityEngine;

public class HomeMenuManager : MonoBehaviour
{
	public static HomeMenuManager Instance { get; private set; }

	[SerializeField] private List<MenuState> states;
	[SerializeField] private string initialState = "Title";

	private Dictionary<string, MenuState> stateDict;
	private Stack<MenuState> stateStack = new();
	private MenuState currentState;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		stateDict = new Dictionary<string, MenuState>();
		foreach (var state in states)
		{
			if (state != null && !string.IsNullOrEmpty(state.stateName))
				stateDict[state.stateName] = state;

			state.stateRoot.SetActive(false); // Hide all on start
		}

		SetState(initialState);
	}

	public void SetState(string stateName)
	{
		//HACK: push to stack 
		bool pushToStack = true;
		if (!stateDict.TryGetValue(stateName, out var newState))
		{
			Debug.LogWarning($"MoveFlick state '{stateName}' not found.");
			return;
		}

		if (currentState != null)
		{
			if (pushToStack)
				stateStack.Push(currentState);
			currentState.stateRoot.SetActive(false);
		}

		currentState = newState;
		currentState.stateRoot.SetActive(true);
	}

	public void ReturnToPreviousState()
	{
		if (stateStack.Count == 0)
		{
			Debug.Log("No previous state to return to.");
			return;
		}

		var previous = stateStack.Pop();
		if (currentState != null)
			currentState.stateRoot.SetActive(false);

		currentState = previous;
		currentState.stateRoot.SetActive(true);
	}
}
