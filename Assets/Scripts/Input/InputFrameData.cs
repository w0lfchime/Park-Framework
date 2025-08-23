


public struct InputFrameData
{
	public ushort buttons;   // 16-bit button bitmask
	public sbyte xAxis;
	public sbyte yAxis;
	public sbyte lookX;
	public sbyte lookY;

	public static InputFrameData Empty => new InputFrameData();

	public bool IsEmpty()
	{
		return buttons == 0 &&
			   xAxis == 0 &&
			   yAxis == 0 &&
			   lookX == 0 &&
			   lookY == 0;
	}
}