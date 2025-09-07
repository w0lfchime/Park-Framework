
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
	private string MatchScene;

	private FP_PhysicsSpace physicsSpace;

	public List<Character> characters;

	public MatchAPS(string matchScene)
	{
		this.MatchScene = matchScene;

		CommandHandler.RegisterCommand("listcharacters", args =>
		{
			ListCharacters();
		});
	}


	public override void OnEnter()
	{
		base.OnEnter();

		// Subscribe before loading
		SceneManager.sceneLoaded += OnSceneLoaded;

		AppManager.LoadScene(MatchScene);

		characters = new List<Character>(Object.FindObjectsByType<Character>(FindObjectsSortMode.None));
	}



	//Runs once upon scene loading. 
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Unsubscribe so it only runs once
		SceneManager.sceneLoaded -= OnSceneLoaded;

		if (scene.name == MatchScene)
		{
			// Find the physics space in the loaded scene
			this.physicsSpace = Object.FindAnyObjectByType<FP_PhysicsSpace>();

			if (physicsSpace != null)
			{
				LogCore.Log(LogType.PhysicsSetup, "Found FP_PhysicsSpace in scene: " + physicsSpace.name);
			}
			else
			{
				LogCore.Log(LogType.PhysicsSetup, $"Failed to find PhysicsSpace within loaded scene: {MatchScene}");
				DebugCore.StopGame();
			}
		}
	}

	public void SpawnCharacter(CharacterID character_id)
	{

	}

	public override void FixedPhysicsUpdate()
	{
		//no base needed 

		physicsSpace?.FixedPhysicsSpaceUpdate();
	}

	public override void OnMonoUpdate()
	{

	}
	public void ListCharacters()
	{
        foreach (var character in characters)
        {
            LogCore.Log(LogType.Response, $"{character.InstanceName}");
        }
	}

}
