using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class DebugCore
{
	private static bool stoppingGame = false;

	public static void StopGame()
	{
		if (!stoppingGame)
		{
			stoppingGame = true;
			Debug.Log("Stopping program...");
			LogCore.loggingEnabled = false;
#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
#else
    Application.Quit();
#endif 
		}
	}


}
