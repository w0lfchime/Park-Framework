using System;
using UnityEngine;

public class FP_BoxCollider
{
    public FP_Body Body;
    public FixVec2 Size; // Full size (width/height), not half-extents

    public FP_BoxCollider(FP_Body body, FixVec2 size)
    {
        Body = body;
        Size = size;
    }

    // Get min and max corners of the AABB
    public FixVec2 Min => Body.Position - (Size * Fix64.FromFloat(0.5f));
    public FixVec2 Max => Body.Position + (Size * Fix64.FromFloat(0.5f));

    public bool Overlaps(FP_BoxCollider other)
    {
        FixVec2 aMin = this.Min;
        FixVec2 aMax = this.Max;
        FixVec2 bMin = other.Min;
        FixVec2 bMax = other.Max;

        return (aMin.x < bMax.x && aMax.x > bMin.x &&
                aMin.y < bMax.y && aMax.y > bMin.y);
    }
}
