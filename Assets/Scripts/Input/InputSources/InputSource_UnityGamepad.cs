public class InputSource_UnityGamepad : IInputSource
{
    private InputAction move, look;

    private InputAction basicAttack, specialAttack, jump, slide;
    private InputAction guard1, guard2, sneak, grab;
    private InputAction moveAlt, lookAlt;
    private InputAction misc1, misc2, misc3, misc4;
    private InputAction menu, menuAlt;

    private InputFrameData current, previous;
    private Dictionary<int, InputFrameData> frameBuffer = new();

    public InputSource_UnityGamepad(InputActionMap actionMap)
    {
        move          = actionMap["Move"];
        look          = actionMap["Look"];
        basicAttack   = actionMap["BasicAttack"];
        specialAttack = actionMap["SpecialAttack"];
        jump          = actionMap["Jump"];
        slide         = actionMap["Slide"];
        guard1        = actionMap["Guard1"];
        guard2        = actionMap["Guard2"];
        sneak         = actionMap["Sneak"];
        grab          = actionMap["Grab"];
        moveAlt       = actionMap["MoveAlt"];
        lookAlt       = actionMap["LookAlt"];
        misc1         = actionMap["Misc1"];
        misc2         = actionMap["Misc2"];
        misc3         = actionMap["Misc3"];
        misc4         = actionMap["Misc4"];
        menu          = actionMap["Menu"];
        menuAlt       = actionMap["MenuAlt"];

        actionMap.Enable();
    }

    public void UpdateInput(int frame)
    {
        previous = current;
        current = SampleInput();
        frameBuffer[frame] = current;
    }

    public InputFrameData GetInputForFrame(int frame)
    {
        return frameBuffer.TryGetValue(frame, out var input) ? input : InputFrameData.Empty;
    }

    public bool HasNewInput()
    {
        return !current.Equals(previous);
    }

    private InputFrameData SampleInput()
    {
        Vector2 moveVec = move.ReadValue<Vector2>();
        Vector2 lookVec = look.ReadValue<Vector2>();

        ushort buttons = 0;
        if (basicAttack.IsPressed())   buttons |= InputButtons.BasicAttack;
        if (specialAttack.IsPressed()) buttons |= InputButtons.SpecialAttack;
        if (jump.IsPressed())          buttons |= InputButtons.Jump;
        if (slide.IsPressed())         buttons |= InputButtons.Slide;
        if (guard1.IsPressed())        buttons |= InputButtons.Guard1;
        if (guard2.IsPressed())        buttons |= InputButtons.Guard2;
        if (sneak.IsPressed())         buttons |= InputButtons.Sneak;
        if (grab.IsPressed())          buttons |= InputButtons.Grab;
        if (moveAlt.IsPressed())       buttons |= InputButtons.MoveAlt;
        if (lookAlt.IsPressed())       buttons |= InputButtons.LookAlt;
        if (misc1.IsPressed())         buttons |= InputButtons.Misc1;
        if (misc2.IsPressed())         buttons |= InputButtons.Misc2;
        if (misc3.IsPressed())         buttons |= InputButtons.Misc3;
        if (misc4.IsPressed())         buttons |= InputButtons.Misc4;
        if (menu.IsPressed())          buttons |= InputButtons.Menu;
        if (menuAlt.IsPressed())       buttons |= InputButtons.MenuAlt;

        return new InputFrameData
        {
            buttons = buttons,
            xAxis   = (sbyte)Mathf.Clamp(moveVec.x * 127f, -127, 127),
            yAxis   = (sbyte)Mathf.Clamp(moveVec.y * 127f, -127, 127),
            lookX   = (sbyte)Mathf.Clamp(lookVec.x * 127f, -127, 127),
            lookY   = (sbyte)Mathf.Clamp(lookVec.y * 127f, -127, 127),
        };
    }
}
