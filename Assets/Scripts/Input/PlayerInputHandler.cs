using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }
    private InputDevice assignedDevice;

    private Dictionary<string, bool> buttonDown = new();
    private Dictionary<string, bool> buttonHold = new();
    private Dictionary<string, bool> buttonUp = new();

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool anyActionAtAll { get; private set; }

    //input processing settings:
    private const float DEAD_ZONE_THRESHOLD = 0.25f; // Adjust this value as needed


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        InitializeDictionaries();
    }

    private void Start()
    {
        AssignDeviceAtStart();
    }

    private void InitializeDictionaries()
    {
        foreach (var action in playerInput.actions)
        {
            buttonDown[action.name] = false;
            buttonHold[action.name] = false;
            buttonUp[action.name] = false;
        }
    }

    private void AssignDeviceAtStart()
    {
        var allDevices = Gamepad.all;
        if (allDevices.Count > 0)
        {
            int playerIndex = playerInput.playerIndex; // P1 -> 0, P2 -> 1, etc.
            if (playerIndex < allDevices.Count)
            {
                AssignDevice(allDevices[playerIndex]);
            }
            else
            {
                AssignDevice(allDevices[0]); // Fallback: Assign first controller
            }
        }
    }

    private void Update()
    {
        if (assignedDevice == null) return;

        anyActionAtAll = false;

        foreach (var action in playerInput.actions)
        {
            buttonDown[action.name] = false; // Reset at the start of the frame
            buttonUp[action.name] = false;   // Reset at the start of the frame
        }

        foreach (var action in playerInput.actions)
        {
            bool pressed = action.IsPressed();
            if (action.WasPressedThisFrame()) buttonDown[action.name] = true;
            if (action.WasReleasedThisFrame()) buttonUp[action.name] = true;
            buttonHold[action.name] = pressed;

            if (pressed) anyActionAtAll = true;
        }

        // Read Vector2 input for movement and look actions
        MoveInput = ApplyDeadZone(playerInput.actions["Move"].ReadValue<Vector2>());
        LookInput = ApplyDeadZone(playerInput.actions["Look"].ReadValue<Vector2>());
    }

    private Vector2 ApplyDeadZone(Vector2 input)
    {
        return input.magnitude < DEAD_ZONE_THRESHOLD ? Vector2.zero : input;
    }

    public void AssignDevice(InputDevice device)
    {
        if (device == null) return;

        assignedDevice = device;
        string scheme = device is Gamepad ? "Gamepad" : "KeyboardMouse"; // Match control scheme
        playerInput.SwitchCurrentControlScheme(scheme, device);
    }

    public bool GetButtonDown(string actionName) => buttonDown.TryGetValue(actionName, out bool value) && value;
    public bool GetButtonHold(string actionName) => buttonHold.TryGetValue(actionName, out bool value) && value;
    public bool GetButtonUp(string actionName) => buttonUp.TryGetValue(actionName, out bool value) && value;
}
