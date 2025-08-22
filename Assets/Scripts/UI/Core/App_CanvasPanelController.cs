using UnityEngine;
using UnityEngine.UI;

public class App_CanvasPanelController : MonoBehaviour
{
	[Header("Canvas Groups")]
	public CanvasGroup DebugView1;
	public CanvasGroup PauseMenu;
	public CanvasGroup PairMenu;
	public CanvasGroup PairingModeUI;

	private bool isDebugVisible = true;
	private bool isPauseMenuVisible = false;
	private bool isPairingMenuVisible = false;

	private bool currentlyPairing = false;

	private void Awake()
	{
		DebugView1.gameObject.SetActive(true);
		PauseMenu.gameObject.SetActive(true);
		PairMenu.gameObject.SetActive(true);	
		PairingModeUI.gameObject.SetActive(true);

		SetCanvasGroupActive(DebugView1, true);
		SetCanvasGroupActive(PauseMenu, false);
		SetCanvasGroupActive(PairMenu, false);
		SetCanvasGroupActive(PairingModeUI, false);
	}

	void Update()
	{
		if (!currentlyPairing)
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
		else
		{

		}
	}

	//function exposed for unity button to access:
	public void EnterPairingMenu()
	{
		if (currentlyPairing || isPairingMenuVisible)
		{
			return;
		}

		isPairingMenuVisible = true;
		SetCanvasGroupActive(PairMenu, true);

		currentlyPairing = false;
		SetCanvasGroupActive(PairingModeUI, false);

	}

	public void ExitPairingMenu()
	{
		if (currentlyPairing)
		{
			ExitPairingMode();
			return;
		}

		isPairingMenuVisible = false;
		SetCanvasGroupActive(PairMenu, false);

	}

	public void EnterPairingMode()
	{
		if (currentlyPairing)
		{
			return;
		}

		isPairingMenuVisible = true;
		SetCanvasGroupActive(PairMenu, true);

		currentlyPairing = true;
		SetCanvasGroupActive(PairingModeUI, true);

		//AppManager.Instance.SystemInputManager.SetState(SysInputManagerState.Pairing);

		//enter pairing
	}

	public void ExitPairingMode()
	{
        if (!isPairingMenuVisible || !currentlyPairing)
        {
			return;
        }


		currentlyPairing = false;
		SetCanvasGroupActive(PairingModeUI, false);

		//AppManager.Instance.SystemInputManager.SetState(SysInputManagerState.Pairing);

		//exit pairing
	}


	private void SetCanvasGroupActive(CanvasGroup group, bool active)
	{
		if (group == null) return;

		group.alpha = active ? 1 : 0;
		group.interactable = active;
		group.blocksRaycasts = active;
	}


}
