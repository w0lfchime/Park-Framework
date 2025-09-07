using UnityEngine;

public class ProcessedInputFrameData
{

	public bool BasicAttack { get; private set; }
	public bool SpecialAttack { get; private set; }
	public bool Jump { get; private set; }
	public bool Slide { get; private set; }
	public bool Guard1 { get; private set; }
	public bool Guard2 { get; private set; }
	public bool Sneak { get; private set; }
	public bool Grab { get; private set; }


	public bool MoveAlt { get; private set; }
	public bool LookAlt { get; private set; }
	public bool Misc1 { get; private set; }
	public bool Misc2 { get; private set; }
	public bool Misc3 { get; private set; }
	public bool Misc4 { get; private set; }

	// === Axes as fixed-point vectors ===
	public FixVec2 Move { get; private set; }
	public FixVec2 Look { get; private set; }


	public static ProcessedInputFrameData FromRaw(InputFrameData raw)
	{
		var processed = new ProcessedInputFrameData();

		// --- Buttons ---
		processed.BasicAttack = (raw.buttons & InputButtons.BasicAttack) != 0;
		processed.SpecialAttack = (raw.buttons & InputButtons.SpecialAttack) != 0;
		processed.Jump = (raw.buttons & InputButtons.Jump) != 0;
		processed.Slide = (raw.buttons & InputButtons.Slide) != 0;
		processed.Guard1 = (raw.buttons & InputButtons.Guard1) != 0;
		processed.Guard2 = (raw.buttons & InputButtons.Guard2) != 0;
		processed.Sneak = (raw.buttons & InputButtons.Sneak) != 0;
		processed.Grab = (raw.buttons & InputButtons.Grab) != 0;
		processed.MoveAlt = (raw.buttons & InputButtons.MoveAlt) != 0;
		processed.LookAlt = (raw.buttons & InputButtons.LookAlt) != 0;
		processed.Misc1 = (raw.buttons & InputButtons.Misc1) != 0;
		processed.Misc2 = (raw.buttons & InputButtons.Misc2) != 0;
		processed.Misc3 = (raw.buttons & InputButtons.Misc3) != 0;
		processed.Misc4 = (raw.buttons & InputButtons.Misc4) != 0;

		processed.Move = new FixVec2(
			FixVec2.FromSByte(raw.xAxis),
			FixVec2.FromSByte(raw.yAxis)
		);

		processed.Look = new FixVec2(
			FixVec2.FromSByte(raw.lookX),
			FixVec2.FromSByte(raw.lookY)
		);

		return processed;
	}
}
