using System.Collections.Generic;

public class FP_Body
{

    
    public FixVec2 Position;
    public FixVec2 Velocity;
    public FixVec2 Acceleration;

    public Fix64 Mass = Fix64.FromFloat(1f);
    public Fix64 Drag = Fix64.FromFloat(0f);
    public bool affectedByGravity = true;
    public bool isStatic = false;
    public bool isKinematic = false;


    //flags
    public bool isWall = false;
    public bool isGround = false;
    public bool isOneWayPlatform = false

    public static readonly Fix64 Gravity = Fix64.FromFloat(-50f);

    public List<FP_CollisionInfo> CollisionsThisFrame = new List<FP_CollisionInfo>();


    public void Step(Fix64 deltaTime)
    {
        if (isStatic) return;

        if (affectedByGravity)
            Acceleration.y += Gravity;

        Velocity += Acceleration * deltaTime;

        if (Drag.RawValue > 0)
        {
            // Drag multiplier (very simplified)
            var dragMultiplier = Fix64.FromFloat(1f) - Drag * deltaTime;
            Velocity *= dragMultiplier;
        }

        Position += Velocity * deltaTime;
        Acceleration = FixVec2.zero;
    }

    public void AddForce(FixVec2 force)
    {
        if (isStatic) return;
        Acceleration += force / Mass;
    }

    public void Register()
    {
        FP_PhysicsSpace.Instance?.RegisterBody(this);
    }

    public void Unregister()
    {
        FP_PhysicsSpace.Instance?.UnregisterBody(this);
    }
}
