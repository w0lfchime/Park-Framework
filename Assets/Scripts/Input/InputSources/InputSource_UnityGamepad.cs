using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;







public class InputSource_UnityGamepad : IInputSource
{
	private InputAction move, look;
	private InputAction basicAttack, specialAttack, jump, slide;
	private InputAction guard1, guard2, sneak, grab;
	private InputAction moveAlt, lookAlt;
	private InputAction misc1, misc2, misc3, misc4;

	private InputFrameData liveState; // continuously updated
	private Dictionary<int, InputFrameData> frameBuffer = new();

	public InputSource_UnityGamepad(InputActionMap actionMap)
	{
		// Axes
		move = actionMap["Move"];
		look = actionMap["Look"];

		// Buttons
		basicAttack = actionMap["BasicAttack"];
		specialAttack = actionMap["SpecialAttack"];
		jump = actionMap["Jump"];
		slide = actionMap["Slide"];
		guard1 = actionMap["Guard1"];
		guard2 = actionMap["Guard2"];
		sneak = actionMap["Sneak"];
		grab = actionMap["Grab"];
		moveAlt = actionMap["MoveAlt"];
		lookAlt = actionMap["LookAlt"];
		misc1 = actionMap["Misc1"];
		misc2 = actionMap["Misc2"];
		misc3 = actionMap["Misc3"];
		misc4 = actionMap["Misc4"];

		// Subscribe to button events
		BindButton(basicAttack, InputButtons.BasicAttack);
		BindButton(specialAttack, InputButtons.SpecialAttack);
		BindButton(jump, InputButtons.Jump);
		BindButton(slide, InputButtons.Slide);
		BindButton(guard1, InputButtons.Guard1);
		BindButton(guard2, InputButtons.Guard2);
		BindButton(sneak, InputButtons.Sneak);
		BindButton(grab, InputButtons.Grab);
		BindButton(moveAlt, InputButtons.MoveAlt);
		BindButton(lookAlt, InputButtons.LookAlt);
		BindButton(misc1, InputButtons.Misc1);
		BindButton(misc2, InputButtons.Misc2);
		BindButton(misc3, InputButtons.Misc3);
		BindButton(misc4, InputButtons.Misc4);

		actionMap.Enable();
	}

	private void BindButton(InputAction action, ushort mask)
	{
		if (action == null) return;

		action.performed += ctx => SetButton(mask, true);
		action.canceled += ctx => SetButton(mask, false);
	}

	private void SetButton(ushort mask, bool pressed)
	{
		if (pressed)
			liveState.buttons |= mask;
		else
			liveState.buttons &= (ushort)~mask;
	}

	// Called from MonoBehaviour Update() (not sim tick)
	public void Update()
	{
		Vector2 moveVec = move.ReadValue<Vector2>();
		Vector2 lookVec = look.ReadValue<Vector2>();

		liveState.xAxis = (sbyte)Mathf.Clamp(moveVec.x * 127f, -127, 127);
		liveState.yAxis = (sbyte)Mathf.Clamp(moveVec.y * 127f, -127, 127);
		liveState.lookX = (sbyte)Mathf.Clamp(lookVec.x * 127f, -127, 127);
		liveState.lookY = (sbyte)Mathf.Clamp(lookVec.y * 127f, -127, 127);
	}

	// Called at 60Hz from SysInputManager
	public void UpdateInput(int frame)
	{
		frameBuffer[frame] = liveState; // freeze current state for this frame
	}

	public InputFrameData GetInputForFrame(int frame)
	{
		return frameBuffer.TryGetValue(frame, out var input) ? input : InputFrameData.Empty;
	}

	public bool HasNewInput(int frame)
	{
		if (!frameBuffer.TryGetValue(frame, out var current)) return false;
		if (!frameBuffer.TryGetValue(frame - 1, out var previous)) return true; // first frame input
		return !current.Equals(previous);
	}
}
