using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class DebugPanel : MonoBehaviour
{
	[Header("UI References")]
	public Text debugText; // Assign in inspector (Text or TMP_Text)

	[Header("Optional Settings")]
	public float updateInterval = 0.25f;

	private float timeSinceLastUpdate;



	void Start()
	{

		if (debugText == null)
		{
			Debug.LogError("DebugPanel: DebugText is not assigned.");
		}
	}

	void Update()
	{
		timeSinceLastUpdate += Time.unscaledDeltaTime;
		if (timeSinceLastUpdate >= updateInterval)
		{
			UpdateDebugInfo();
			timeSinceLastUpdate = 0f;
		}
	}

	void UpdateDebugInfo()
	{
		if (debugText == null) return;

		StringBuilder sb = new StringBuilder();

		sb.AppendLine($"AppState: {AppManager.CurrentState?.GetType().Name ?? "None"}");
		sb.AppendLine($"Time: {Time.time:F2}");
		sb.AppendLine($"FPS: {(1f / Time.unscaledDeltaTime):F0}");

		// Add more as needed...
		// sb.AppendLine($"Players: {appManager.PlayerCount}");

		debugText.text = sb.ToString();
	}
}
