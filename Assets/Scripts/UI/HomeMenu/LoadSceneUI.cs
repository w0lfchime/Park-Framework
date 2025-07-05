using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneUI : MonoBehaviour
{
	[Header("Setup")]
	public Transform contentParent;         // Scroll View's Content transform
	public GameObject buttonPrefab;         // Button prefab reference
	public string[] sceneNames;             // List of scenes to show

	void Start()
	{
		foreach (var sceneName in sceneNames)
		{
			var btn = Instantiate(buttonPrefab, contentParent);
			btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = sceneName;

			btn.GetComponent<Button>().onClick.AddListener(() =>
			{
				AppManager.LoadScene(sceneName);
			});
		}
	}
}

