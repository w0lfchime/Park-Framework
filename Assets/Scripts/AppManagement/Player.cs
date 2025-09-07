using NUnit;
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

	public void Update()
	{
		if (InputSource != null)
		{
			InputSource.Update();
		}
	}

	public void RecordFrame(int frame)
	{
		if (InputSource == null)
		{
			return;
		}
		InputSource.UpdateInput(frame);
		var input = InputSource.GetInputForFrame(frame);

		if (frame >= inputHistory.Count)
			inputHistory.Add(input);
		else
			inputHistory[frame] = input;
	}

	public InputFrameData GetFrame(int frame) //HACK: major hack
	{
		LogCore.Log($"{inputHistory.Count} --- {frame}");
		InputFrameData data = inputHistory[frame];
		return data;
	}

	public InputFrameData GetFrame() //HACK: more hack. need to actually write this to sample frames
	{
		return inputHistory[inputHistory.Count - 1];
	}

	public ProcessedInputFrameData GetInput(int frame)
	{
		//TODO: IDK. DO WE NEED IT?

		return null;
	}

	public ProcessedInputFrameData GetInput()
	{
		return ProcessedInputFrameData.FromRaw(inputHistory[inputHistory.Count - 1]);
	}


	public void SetFrame(int frame, InputFrameData data)
	{
		if (frame >= inputHistory.Count)
			inputHistory.Add(data);
		else
			inputHistory[frame] = data;
	}

	public void AssignInput(InputDevice device, IInputSource source)
	{
		Device = device;
		InputSource = source;
	}

	public void ClearInput()
	{
		Device = null;
		InputSource = null;
	}

	public void ClearHistory() => inputHistory.Clear();
}
