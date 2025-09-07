using System;
using System.Numerics;


[Serializable]
public struct FixVec2
{
    public Fix64 x, y;

    public static readonly FixVec2 zero = new(Fix64.Zero, Fix64.Zero);
    public static readonly FixVec2 one = new(Fix64.FromFloat(1), Fix64.FromFloat(1));
    public static readonly FixVec2 right = new(Fix64.FromFloat(1), Fix64.Zero);
    public static readonly FixVec2 up = new(Fix64.Zero, Fix64.FromFloat(1));

    public FixVec2(Fix64 x, Fix64 y)
    {
        this.x = x;
        this.y = y;
    }
    public FixVec2(float x, float y)
    {
        this.x = Fix64.FromFloat(x);
        this.y = Fix64.FromFloat(y);
    }

    public FixVec2(UnityEngine.Vector3 vector)
    {
        this.x = Fix64.FromFloat(vector.x);
        this.y = Fix64.FromFloat(vector.y);
    }
    public FixVec2(UnityEngine.Vector2 vector)
    {
        this.x = Fix64.FromFloat(vector.x);
        this.y = Fix64.FromFloat(vector.y);
    }

    //vector conversions 
	public UnityEngine.Vector2 ToVector2()
	{
		return new UnityEngine.Vector2(x.ToFloat(), y.ToFloat());
	}
	public static FixVec2 FromVector2(UnityEngine.Vector2 v)
	{
		return new FixVec2(Fix64.FromFloat(v.x), Fix64.FromFloat(v.y));
	}
	public UnityEngine.Vector3 ToVector3()
	{
		return new UnityEngine.Vector3(x.ToFloat(), y.ToFloat(), 0f);
	}
	public static FixVec2 FromVector3(UnityEngine.Vector3 v)
	{
		return new FixVec2(Fix64.FromFloat(v.x), Fix64.FromFloat(v.y));
	}
	public static Fix64 FromSByte(sbyte value)
	{

		return new Fix64((long)value * Fix64.ONE / 127);
	}

	public static FixVec2 Normalize(FixVec2 v)
    {
        Fix64 mag = v.Magnitude;
        return mag > Fix64.Zero ? v / mag : zero;
    }


    // Operators
    public static FixVec2 operator +(FixVec2 a, FixVec2 b) => new(a.x + b.x, a.y + b.y);
    public static FixVec2 operator -(FixVec2 a, FixVec2 b) => new(a.x - b.x, a.y - b.y);
    public static FixVec2 operator *(FixVec2 a, Fix64 b) => new(a.x * b, a.y * b);
    public static FixVec2 operator *(Fix64 b, FixVec2 a) => new(a.x * b, a.y * b);
    public static FixVec2 operator *(FixVec2 a, FixVec2 b) => new(a.x * b.x, a.y * b.y);

    public static FixVec2 operator /(FixVec2 a, Fix64 b) => new(a.x / b, a.y / b);
    public static FixVec2 operator -(FixVec2 v) => new(-v.x, -v.y); // Unary negation

    public static bool operator ==(FixVec2 a, FixVec2 b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(FixVec2 a, FixVec2 b) => !(a == b);

    // Equality Overrides
    public override bool Equals(object obj)
    {
        return obj is FixVec2 other && this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x.RawValue, y.RawValue);
    }

    // Magnitude (length)
    public Fix64 Magnitude => Fix64.Sqrt(SqrMagnitude);
    public Fix64 SqrMagnitude => x * x + y * y;

    public FixVec2 Normalized
    {
        get
        {
            Fix64 mag = Magnitude;
            return mag > Fix64.Zero ? this / mag : zero;
        }
    }

    // Dot product
    public static Fix64 Dot(FixVec2 a, FixVec2 b) => a.x * b.x + a.y * b.y;

    // Clamp to a max length
    public static FixVec2 ClampMagnitude(FixVec2 v, Fix64 maxLength)
    {
        var sqrMag = v.SqrMagnitude;
        var maxSqr = maxLength * maxLength;
        return sqrMag > maxSqr ? v.Normalized * maxLength : v;
    }

    // Perpendicular vector (like a 90 degree rotation)
    public FixVec2 Perpendicular() => new(-y, x);

    // Distance
    public static Fix64 Distance(FixVec2 a, FixVec2 b) => (a - b).Magnitude;

    // Lerp (Linear Interpolation)
    public static FixVec2 Lerp(FixVec2 a, FixVec2 b, Fix64 t)
    {
        return a + (b - a) * t;
    }

    // IsZero check (very common in physics)
    public bool IsZero => x == Fix64.Zero && y == Fix64.Zero;

    public override string ToString() => $"({x}, {y})";
}
