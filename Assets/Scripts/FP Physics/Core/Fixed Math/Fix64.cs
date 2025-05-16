using System;

public struct Fix64
{


    public const int SHIFT = 16;
    public const long ONE = 1L << SHIFT;
    public long RawValue;



    public static readonly Fix64 Zero = new(0);



    public Fix64(long raw) => RawValue = raw;
    public static Fix64 FromFloat(float f) => new((long)(f * ONE));
    public float ToFloat() => (float)RawValue / ONE;

    // Basic operators
    public static Fix64 operator +(Fix64 a, Fix64 b) => new(a.RawValue + b.RawValue);
    public static Fix64 operator -(Fix64 a, Fix64 b) => new(a.RawValue - b.RawValue);
    public static Fix64 operator -(Fix64 a) => new(-a.RawValue);
    public static Fix64 operator *(Fix64 a, Fix64 b) => new((a.RawValue * b.RawValue) >> SHIFT);
    public static Fix64 operator /(Fix64 a, Fix64 b) => new((a.RawValue << SHIFT) / b.RawValue);


    public static bool operator >(Fix64 a, Fix64 b) => a.RawValue > b.RawValue;
    public static bool operator <(Fix64 a, Fix64 b) => a.RawValue < b.RawValue;
    public static bool operator >=(Fix64 a, Fix64 b) => a.RawValue >= b.RawValue;
    public static bool operator ==(Fix64 a, Fix64 b) => a.RawValue == b.RawValue;
    public static bool operator !=(Fix64 a, Fix64 b) => a.RawValue != b.RawValue;

    // Also override Equals and GetHashCode:
    public override bool Equals(object obj)
    {
        return obj is Fix64 other && RawValue == other.RawValue;
    }

    public override int GetHashCode()
    {
        return RawValue.GetHashCode();
    }

    public static bool operator <=(Fix64 a, Fix64 b) => a.RawValue <= b.RawValue;

    public static Fix64 Abs(Fix64 value)
    {
        return value.RawValue < 0 ? new Fix64(-value.RawValue) : value;
    }

    public static Fix64 Min(Fix64 a, Fix64 b) => a.RawValue < b.RawValue ? a : b;
    public static Fix64 Max(Fix64 a, Fix64 b) => a.RawValue > b.RawValue ? a : b;

    public static Fix64 Clamp(Fix64 value, Fix64 min, Fix64 max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static int Sign(Fix64 value)
    {
        if (value.RawValue < 0) return -1;
        if (value.RawValue > 0) return 1;
        return 0;
    }

    public static Fix64 Sqrt(Fix64 value)
    {
        if (value.RawValue < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Cannot compute square root of a negative number");

        if (value.RawValue == 0)
            return Fix64.Zero;

        long n = value.RawValue;
        long x = n;
        long lastX;

        do
        {
            lastX = x;
            x = (x + (n << SHIFT) / x) >> 1;
        } while (Math.Abs(x - lastX) > 1);

        return new Fix64(x);
    }



    public override string ToString() => ToFloat().ToString("F4");
}
