using UnityEngine;
using UnityEngine.UI;

public class C_CanvasPanelController : MonoBehaviour
{
	[Header("Canvas Groups")]
	public CanvasGroup DebugView1;
	public CanvasGroup PauseMenu;

	private bool isDebugVisible = true;
	private bool isPauseMenuVisible = false;

	private void Awake()
	{
		DebugView1.gameObject.SetActive(true);
		PauseMenu.gameObject.SetActive(true);

		SetCanvasGroupActive(DebugView1, true);
		SetCanvasGroupActive(PauseMenu, false);
	}

	void Update()
	{
		// Toggle DebugView1 with ~
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			if (GlobalDebugFlags.GlobalDebug)
			{
				isDebugVisible = !isDebugVisible;
				SetCanvasGroupActive(DebugView1, isDebugVisible);
			}
		}

		// Toggle PauseMenu with Esc
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (AppManager.OpenPauseAllowed)
			{
				isPauseMenuVisible = !isPauseMenuVisible;
				SetCanvasGroupActive(PauseMenu, isPauseMenuVisible);
			}
		}
	}

	private void SetCanvasGroupActive(CanvasGroup group, bool active)
	{
		if (group == null) return;

		group.alpha = active ? 1 : 0;
		group.interactable = active;
		group.blocksRaycasts = active;
	}
}
