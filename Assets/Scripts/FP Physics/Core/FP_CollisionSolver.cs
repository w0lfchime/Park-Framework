using UnityEngine;

public class FP_CollisionSolver : MonoBehaviour
{
    public static FP_CollisionSolver Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CheckAndResolve(FP_BoxCollider2D a, FP_BoxCollider2D b)
    {
        if (a.IsTrigger || b.IsTrigger)
        {
            // TODO: Handle triggers later
            return;
        }

        var aMin = a.GetWorldPosition() - (a.Size * Fix64.FromFloat(0.5f));
        var aMax = a.GetWorldPosition() + (a.Size * Fix64.FromFloat(0.5f));

        var bMin = b.GetWorldPosition() - (b.Size * Fix64.FromFloat(0.5f));
        var bMax = b.GetWorldPosition() + (b.Size * Fix64.FromFloat(0.5f));

        bool overlapX = aMin.x < bMax.x && aMax.x > bMin.x;
        bool overlapY = aMin.y < bMax.y && aMax.y > bMin.y;

        if (!(overlapX && overlapY))
            return;

        Fix64 overlapXAmt = Fix64.Min(aMax.x - bMin.x, bMax.x - aMin.x);
        Fix64 overlapYAmt = Fix64.Min(aMax.y - bMin.y, bMax.y - aMin.y);

        FP_Body2D bodyA = a.ParentBody;
        FP_Body2D bodyB = b.ParentBody;

        FP_Body2D movable = null;
        FixVec2 resolution = FixVec2.zero;


        // Determine resolution direction
        if (overlapXAmt < overlapYAmt)
        {
            resolution = new FixVec2(a.GetWorldPosition().x < b.GetWorldPosition().x ? overlapXAmt : -overlapXAmt, Fix64.Zero);
        }
        else
        {
            resolution = new FixVec2(Fix64.Zero, a.GetWorldPosition().y < b.GetWorldPosition().y ? overlapYAmt : -overlapYAmt);
        }

        // === Resolution Logic Based on Body Types ===
        if (bodyA.IsStatic && bodyB.IsStatic)
        {
            // Do nothing
            return;
        }
        else if (bodyA.IsStatic && (bodyB.IsDynamic || bodyB.IsKinematic))
        {
            movable = bodyB;
        }
        else if (bodyB.IsStatic && (bodyA.IsDynamic || bodyA.IsKinematic))
        {
            movable = bodyA;
        }
        else if (bodyA.IsDynamic && bodyB.IsKinematic)
        {
            movable = bodyA;
        }
        else if (bodyB.IsDynamic && bodyA.IsKinematic)
        {
            movable = bodyB;
        }
        else if (bodyA.IsDynamic && bodyB.IsDynamic)
        {
            // Basic version: move bodyB (or you could split the resolution later)
            movable = bodyB;
        }
        else if (bodyA.IsKinematic && bodyB.IsKinematic)
        {
            // Do nothing for now — you could support pushing other kinematics in special cases
            return;
        }

        resolution += FixVec2.Normalize(resolution) * Fix64.FromFloat(0.001f); // small nudge to prevent re-penetration
        // Apply correction
        movable?.Move(resolution);
    }

}
