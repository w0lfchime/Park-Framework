




public interface IInputSource
{
	void Update();
	bool HasNewInput(int frame); // True if a new input occurred this frame
	InputFrameData GetInputForFrame(int frame); // Deterministic input
	void UpdateInput(int frame); // Called each frame to sample new data
}
