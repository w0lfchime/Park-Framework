




public class InputSource_UnityGamepad : IInputSource
{
	//somewhere here I also need to handle that I have two action maps in my input action asset, one for 
	//cursor control and one for gameplay. Should I be able to poll either at any time based on the system input manager?
	//or, should I be switching action map here. Not sure.

	private Gamepad pad;

	private InputFrameData lastData;

	public InputSource_UnityGamepad(Gamepad pad)
	{
		this.pad = pad;
	}


	public InputFrameData GetInputForFrame() //not sure of this emthod, may need remaning /refactoring/ removing
	{
		return lastData; // D
	}

	private InputFrameData GetCurrentInput()
	{
		//I have an Input Action asset with 2 action maps that is mostly set up and I will attach a screenshot. 
		//Should I be processing input here? If so can you help with that? 
	}
}
