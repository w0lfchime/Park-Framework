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

    // NEW: scene loading busy flag
    public bool SceneLoadingBusy { get; private set; }

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
        app_canvas = GameObject.Find("App_Canvas");
        app_camera = GameObject.Find("App_Camera");

        //services 
        SystemInputManager = new SysInputManager();
        FixedGameUpdateDriver = new FixedGameUpdateDriver();

        //appstate
        SetAppState(BootAppState);
    }



    private void Update()
    {
        FixedGameUpdateDriver.MonoUpdate();

        SystemInputManager.MonoUpdate();
        CurrentState?.OnMonoUpdate();
    }

    //Called by fixed game update driver. circular calling
    public void FixedGameUpdate()
    {
        SystemInputManager.FixedGameUpdate();

        //Our custom logic update
        CurrentState.FixedGameUpdate();
        //Our custom physics update
        CurrentState.FixedPhysicsUpdate();
    }


    //scene management
    public static void LoadScene(string sceneName)
    {
        if (Instance == null)
        {
            LogCore.Log(LogType.Fatal, "Tried to load a scene but AppManager.Instance is null.");
            return;
        }

        Instance.StartCoroutine(Instance.LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string newSceneName)
    {
        SceneLoadingBusy = true;
        try
        {
            // 1) If a scene with this name is already loaded, unload it first.
            Scene existingScene = SceneManager.GetSceneByName(newSceneName);
            if (existingScene.IsValid() && existingScene.isLoaded)
            {
                var preUnloadOp = SceneManager.UnloadSceneAsync(existingScene);
                while (preUnloadOp != null && !preUnloadOp.isDone)
                    yield return null;

                // If this was also tracked as the current loaded scene, clear it
                if (currentLoadedScene == newSceneName)
                    currentLoadedScene = null;
            }

            // 2) Your original behavior: unload the currently tracked scene, if any.
            if (!string.IsNullOrEmpty(currentLoadedScene))
            {
                var unloadOp = SceneManager.UnloadSceneAsync(currentLoadedScene);
                while (unloadOp != null && !unloadOp.isDone)
                    yield return null;
            }

            // 3) Load the new scene additively.
            var loadOp = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
            while (loadOp != null && !loadOp.isDone)
                yield return null;

            currentLoadedScene = newSceneName;
        }
        finally
        {
            SceneLoadingBusy = false;
        }
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
