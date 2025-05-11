using System.Collections.Generic;
using UnityEngine;

public class HomeMenuManager : MonoBehaviour
{
    public static HomeMenuManager Instance { get; private set; }

    [SerializeField] private List<MenuState> states;
    [SerializeField] private string initialState = "Title";

    private Dictionary<string, MenuState> stateDict;
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
        if (!stateDict.ContainsKey(stateName))
        {
            Debug.LogWarning($"Menu state '{stateName}' not found.");
            return;
        }

        if (currentState != null)
            currentState.stateRoot.SetActive(false);

        currentState = stateDict[stateName];
        currentState.stateRoot.SetActive(true);
    }
}
