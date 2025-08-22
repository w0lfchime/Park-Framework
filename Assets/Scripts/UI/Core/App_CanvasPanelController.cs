using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class App_CanvasPanelController : MonoBehaviour
{
	[Header("Canvas Groups")]
	public CanvasGroup DebugView1;
	public CanvasGroup PauseMenu;
	public CanvasGroup PairMenu;
	public CanvasGroup PairingModeUI;

	[Header("Buttons")]
	public Button AddPlayerButton;
	public Button RemovePlayerButton;

	[Header("Player UI Slots")]
	public List<CanvasGroup> playerSlots = new List<CanvasGroup>(); // Assign 4 slots in inspector

	[Header("Player Settings")]
	public int MaxPlayers = 4;
	public int PlayerCount = 1;

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

		// Initialize player slots
		for (int i = 0; i < playerSlots.Count; i++)
		{
			if (i == 0) SetPlayerSlotActive(playerSlots[i], true); // First player always active
			else SetPlayerSlotActive(playerSlots[i], false);
		}
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
	public void EnterPairingMenu()
	{
		if (currentlyPairing || isPairingMenuVisible) return;

		isPairingMenuVisible = true;
		SetCanvasGroupActive(PairMenu, true);

		currentlyPairing = false;
		SetCanvasGroupActive(PairingModeUI, false);

		CheckAddRemoveButtonUsageAllowed();
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
		if (currentlyPairing) return;

		isPairingMenuVisible = true;
		SetCanvasGroupActive(PairMenu, true);

		currentlyPairing = true;
		SetCanvasGroupActive(PairingModeUI, true);

		SetButtonActive(AddPlayerButton, false);
		SetButtonActive(RemovePlayerButton, false);

		//AppManager.Instance.SystemInputManager.SetState(SysInputManagerState.Pairing);
	}

	public void ExitPairingMode()
	{
		if (!isPairingMenuVisible || !currentlyPairing) return;

		currentlyPairing = false;
		SetCanvasGroupActive(PairingModeUI, false);

		SetButtonActive(AddPlayerButton, true);
		SetButtonActive(RemovePlayerButton, true);

		CheckAddRemoveButtonUsageAllowed();


		//AppManager.Instance.SystemInputManager.SetState(SysInputManagerState.Disabled);
	}

	// -------------------
	// PLAYER SLOT LOGIC
	// -------------------
	public void SetPlayerSlotActive(CanvasGroup player, bool active)
	{
		if (player == null) return;
		player.alpha = active ? 1f : 0.05f; // full opacity if active, faded if inactive
		player.interactable = active;
		player.blocksRaycasts = active;
	}

	public void AddPlayer()
	{
		if (PlayerCount >= MaxPlayers) return; // already max

		PlayerCount++;
		SetPlayerSlotActive(playerSlots[PlayerCount - 1], true);

		CheckAddRemoveButtonUsageAllowed();
	}

	public void RemovePlayer()
	{
		if (PlayerCount <= 1) return; // at least one player must exist

		SetPlayerSlotActive(playerSlots[PlayerCount - 1], false);
		PlayerCount--;

		CheckAddRemoveButtonUsageAllowed();
	}

	public void CheckAddRemoveButtonUsageAllowed()
	{
		if (PlayerCount > 1)
		{
			SetButtonActive(RemovePlayerButton, true);
		} 
		else
		{
			SetButtonActive(RemovePlayerButton, false);
		}
		if (PlayerCount < MaxPlayers)
		{
			SetButtonActive(AddPlayerButton, true);
		}
		else
		{
			SetButtonActive(AddPlayerButton, false);
		}
	}

	private void SetCanvasGroupActive(CanvasGroup group, bool active)
	{
		if (group == null) return;

		group.alpha = active ? 1 : 0;
		group.interactable = active;
		group.blocksRaycasts = active;
	}
	public void SetButtonActive(Button button, bool active)
	{
		if (button == null) return;

		// Get the CanvasGroup attached to the button
		CanvasGroup group = button.GetComponent<CanvasGroup>();
		if (group == null) return;

		// Visual state
		group.alpha = active ? 1f : 0.3f;
		group.interactable = active;
		group.blocksRaycasts = active;

		// Explicitly disable/enable the Button component
		button.interactable = active;
	}

}
