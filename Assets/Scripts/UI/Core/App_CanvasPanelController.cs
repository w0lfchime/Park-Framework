using UnityEngine;
using UnityEngine.UI;

public class App_CanvasPanelController : MonoBehaviour
{
	[Header("Canvas Groups")]
	public CanvasGroup DebugView1;
	public CanvasGroup PauseMenu;
	public CanvasGroup PairMenu;

	private bool isDebugVisible = true;
	private bool isPauseMenuVisible = false;
	private bool currentlyPairing = false;

	private void Awake()
	{
		DebugView1.gameObject.SetActive(true);
		PauseMenu.gameObject.SetActive(true);
		PairMenu.gameObject.SetActive(true);	

		SetCanvasGroupActive(DebugView1, true);
		SetCanvasGroupActive(PauseMenu, false);
		SetCanvasGroupActive(PairMenu, false);
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
	}

	//function exposed for unity button to access:
	public void EnterPairingMode()
	{
		if (currentlyPairing)
		{
			return;
		}

		currentlyPairing = true;

		AppManager.Instance.SystemInputManager.SetState(SysInputManagerState.Pairing);
	}


	private void SetCanvasGroupActive(CanvasGroup group, bool active)
	{
		if (group == null) return;

		group.alpha = active ? 1 : 0;
		group.interactable = active;
		group.blocksRaycasts = active;
	}

	//private void OnEnable()
	//{
	//	SysInputManager.OnPlayerPaired += HandlePlayerPaired;
	//	SysInputManager.OnPlayerUnpaired += HandlePlayerUnpaired;
	//}

	//private void OnDisable()
	//{
	//	SysInputManager.OnPlayerPaired -= HandlePlayerPaired;
	//	SysInputManager.OnPlayerUnpaired -= HandlePlayerUnpaired;
	//}

	//private void HandlePlayerPaired(int playerId, InputDevice device)
	//{
	//	playerSlots[playerId - 1].text = $"Player {playerId}: {device.displayName}";
	//}

	//private void HandlePlayerUnpaired(int playerId)
	//{
	//	playerSlots[playerId - 1].text = $"Player {playerId}: <empty>";
	//}
}
