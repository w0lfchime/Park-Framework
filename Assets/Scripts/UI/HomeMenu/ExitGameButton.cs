using UnityEngine;

public class ExitGameButton : MonoBehaviour
{
	public void ExitGame()
	{
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in Editor
#else
		Application.Quit(); // Quit built app
#endif
	}
}
