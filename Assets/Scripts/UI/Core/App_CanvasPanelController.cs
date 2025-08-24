using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum PlayerControllerType
{
	None,
	Keyboard,
	Gamepad,
}

[System.Serializable]
public class PlayerSlot
{
	public CanvasGroup canvasGroup;
	[HideInInspector] public PlayerControllerType controllerType = PlayerControllerType.None;
	[HideInInspector] public GameObject spawnedIcon; // track spawned prefab
}


public class App_CanvasPanelController : MonoBehaviour
{
	[Header("Controller Icons")]
	public GameObject keyboardIconPrefab;
	public GameObject gamepadIconPrefab;

	[Header("Canvas Groups")]
	public CanvasGroup DebugView1;
	public CanvasGroup PauseMenu;
	public CanvasGroup PairMenu;
	public CanvasGroup PairingModeUI;

	[Header("Buttons")]
	public Button AddPlayerButton;
	public Button RemovePlayerButton;

	[Header("Player UI Slots")]
	public List<PlayerSlot> playerSlots = new List<PlayerSlot>(); // Assign in inspector





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

		SetCanvasGroupActive(DebugView1, false);
		SetCanvasGroupActive(PauseMenu, false);
		SetCanvasGroupActive(PairMenu, false);
		SetCanvasGroupActive(PairingModeUI, false);

		// Initialize player slots
		for (int i = 0; i < playerSlots.Count; i++)
		{
			if (i == 0) SetPlayerSlotActive(playerSlots[i].canvasGroup, true); // First player always active
			else SetPlayerSlotActive(playerSlots[i].canvasGroup, false);
		}
	}

	private void OnEnable()
	{
		if (AppManager.Instance?.SystemInputManager != null)
		{
			AppManager.Instance.SystemInputManager.OnDevicePaired += HandleDevicePaired;
			AppManager.Instance.SystemInputManager.OnDeviceUnpaired += HandleDeviceUnpaired;
		}
	}

	private void OnDisable()
	{
		if (AppManager.Instance?.SystemInputManager != null)
		{
			AppManager.Instance.SystemInputManager.OnDevicePaired -= HandleDevicePaired;
			AppManager.Instance.SystemInputManager.OnDeviceUnpaired -= HandleDeviceUnpaired;
		}
	}


	void Update()
	{
		if (!currentlyPairing)
		{
			// Toggle DebugView1 with ~
			//if (Input.GetKeyDown(KeyCode.BackQuote))
			//{
			//	if (GlobalDebugFlags.GlobalDebug)
			//	{
			//		isDebugVisible = !isDebugVisible;
			//		SetCanvasGroupActive(DebugView1, isDebugVisible);
			//	}
			//}

			// Toggle PauseMenu with Esc
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (AppManager.OpenPauseAllowed)
				{
					ExitPairingMenu();
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

		UpdatePlayerSlotColorsAndPairingIcons();
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

		UpdatePlayerSlotColorsAndPairingIcons();

		AppManager.Instance.SystemInputManager.EnterPairingMode();
	}

	public void ExitPairingMode()
	{
		if (!isPairingMenuVisible || !currentlyPairing) return;

		currentlyPairing = false;
		SetCanvasGroupActive(PairingModeUI, false);

		SetButtonActive(AddPlayerButton, true);
		SetButtonActive(RemovePlayerButton, true);

		CheckAddRemoveButtonUsageAllowed();

		UpdatePlayerSlotColorsAndPairingIcons();

		AppManager.Instance.SystemInputManager.ExitPairingMode();
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
		SetPlayerSlotActive(playerSlots[PlayerCount - 1].canvasGroup, true);

		AppManager.Instance.SystemInputManager.AddPlayer();

		CheckAddRemoveButtonUsageAllowed();
	}

	public void RemovePlayer()
	{
		if (PlayerCount <= 1) return; // at least one player must exist

		SetPlayerSlotActive(playerSlots[PlayerCount - 1].canvasGroup, false);
		PlayerCount--;

		AppManager.Instance.SystemInputManager.RemovePlayer();

		CheckAddRemoveButtonUsageAllowed();

		UpdatePlayerSlotColorsAndPairingIcons();
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

	private void HandleDevicePaired(int playerId, PlayerControllerType type)
	{
		if (playerId - 1 < 0 || playerId - 1 >= playerSlots.Count) return;

		playerSlots[playerId - 1].controllerType = type;
		UpdatePlayerSlotColorsAndPairingIcons(); // refresh visuals
	}

	private void HandleDeviceUnpaired(int playerId)
	{
		if (playerId - 1 < 0 || playerId - 1 >= playerSlots.Count) return;

		playerSlots[playerId - 1].controllerType = PlayerControllerType.None;
		UpdatePlayerSlotColorsAndPairingIcons(); // refresh visuals
	}

	public void UpdatePlayerSlotColorsAndPairingIcons()
	{
		foreach (var slot in playerSlots)
		{
			if (slot.canvasGroup == null) continue;

			// Get or add the Image component
			var image = slot.canvasGroup.GetComponent<UnityEngine.UI.Image>();
			if (image == null) continue;

			// Reset color + icon
			image.color = Color.white;
			if (slot.spawnedIcon != null) Destroy(slot.spawnedIcon);

			switch (slot.controllerType)
			{
				case PlayerControllerType.None:
					image.color = Color.white;
					break;

				case PlayerControllerType.Keyboard:
					image.color = new Color(0.7f, 0.85f, 1f); // light blue
					if (keyboardIconPrefab != null)
					{
						slot.spawnedIcon = Instantiate(
							keyboardIconPrefab,
							slot.canvasGroup.transform
						);
						slot.spawnedIcon.transform.localPosition = new Vector3(0, -50f, 0); // adjust as needed
					}
					break;

				case PlayerControllerType.Gamepad:
					image.color = new Color(0.7f, 0.85f, 1f); // light blue
					if (gamepadIconPrefab != null)
					{
						slot.spawnedIcon = Instantiate(
							gamepadIconPrefab,
							slot.canvasGroup.transform
						);
						slot.spawnedIcon.transform.localPosition = new Vector3(0, -50f, 0); // adjust as needed
					}
					break;
			}
		}
	}


}
