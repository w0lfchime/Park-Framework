
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MatchState
{
	Null,
	Cinematic,
	Gameplay,

}

public class MatchAPS : AppState
{



	public AppCameraController Camera;
	public string MatchScene;



	private FP_GameSpace gameSpace;

	public MatchAPS(string matchScene)
	{
		this.MatchScene = matchScene;
		this.Camera = App.app_camera.GetComponent<AppCameraController>();
		if (this.Camera == null)
		{
			LogCore.Log(LogType.Camera, "Match APS failed to get AppCameraController ref.");
		}
	}


	public override void OnEnter()
	{
		base.OnEnter();

		// Subscribe before loading
		SceneManager.sceneLoaded += OnSceneLoaded;

		AppManager.LoadScene(MatchScene);

	}



	//Runs once upon scene loading. 
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Unsubscribe so it only runs once
		SceneManager.sceneLoaded -= OnSceneLoaded;

		if (scene.name == MatchScene)
		{
			// Find the physics space in the loaded scene
			this.gameSpace = Object.FindAnyObjectByType<FP_GameSpace>();

			if (gameSpace != null)
			{
				LogCore.Log(LogType.PhysicsSetup, "Found FP_GameSpace in scene: " + gameSpace.name);
			}
			else
			{
				LogCore.Log(LogType.PhysicsSetup, $"Failed to find GameSpace within loaded scene: {MatchScene}");
				DebugCore.StopGame();
			}

			InitCamera();
		}
	}

	private void InitCamera()
	{
		//Camera setup
		Camera.SetMode(AppCameraController.CameraMode.LerpToTarget);
		Camera.FindAndCopySceneCamera();
	}

	public void SpawnCharacter(CharacterID character_id)
	{

	}

	public override void FixedPhysicsUpdate()
	{
		//no base needed 

		gameSpace?.FixedPhysicsSpaceUpdate();
	}

	public override void OnMonoUpdate()
	{

	}


}
