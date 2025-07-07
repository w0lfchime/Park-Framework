using UnityEngine;

public class IInputSource
{
	bool HasNewInput(); // Only returns true if a fresh input happened this frame
	InputFrameData GetInputForFrame(int frame); // Should be deterministic
}
