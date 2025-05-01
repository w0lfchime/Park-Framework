using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AppState : MonoBehaviour
{
	public AppManager appManager;

    public SubState currentSubState;
    public List<SubState> loadedSubStates;

	public AppState(AppManager appManager)
    {
		this.appManager = appManager;


    }
	//Scenes

	// Lifecycle Methods
	public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }

}
