[System.Flags]
public enum FP_PhysicsFlags
{
    None = 0,
    Ground = 1 << 0,
    Wall = 1 << 1,
    OneWayPlatform = 1 << 2,
    Slippery = 1 << 3,
    Climbable = 1 << 4,
    // Add more as needed
}
