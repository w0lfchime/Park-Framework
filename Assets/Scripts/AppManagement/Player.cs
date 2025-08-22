using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player
{
	public int Id { get; private set; }
	public InputDevice Device { get; private set; }
	public IInputSource InputSource { get; private set; }

	private List<InputFrameData> inputHistory = new();

	public Player(int id, InputDevice device, IInputSource source)
	{
		Id = id;
		Device = device;
		InputSource = source;
	}

	public void Update() => InputSource.Update();

	public void RecordFrame(int frame)
	{
		InputSource.UpdateInput(frame);
		var input = InputSource.GetInputForFrame(frame);

		if (frame >= inputHistory.Count)
			inputHistory.Add(input);
		else
			inputHistory[frame] = input;
	}

	public InputFrameData GetFrame(int frame) =>
		frame < inputHistory.Count ? inputHistory[frame] : InputFrameData.Empty;

	public void SetFrame(int frame, InputFrameData data)
	{
		if (frame >= inputHistory.Count)
			inputHistory.Add(data);
		else
			inputHistory[frame] = data;
	}

	public void ClearHistory() => inputHistory.Clear();
}
