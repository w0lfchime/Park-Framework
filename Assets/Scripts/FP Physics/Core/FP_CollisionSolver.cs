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
		if (a.IsTrigger || b.IsTrigger) return;

		// --- 1. Compute AABB overlap ---
		var half = Fix64.FromFloat(0.5f);
		var aMin = a.GetWorldPosition() - a.Size * half;
		var aMax = a.GetWorldPosition() + a.Size * half;
		var bMin = b.GetWorldPosition() - b.Size * half;
		var bMax = b.GetWorldPosition() + b.Size * half;

		if (aMin.x >= bMax.x || aMax.x <= bMin.x ||
			aMin.y >= bMax.y || aMax.y <= bMin.y) return;        // no overlap

		Fix64 dx = Fix64.Min(aMax.x - bMin.x, bMax.x - aMin.x);
		Fix64 dy = Fix64.Min(aMax.y - bMin.y, bMax.y - aMin.y);

		// --- 2. Pick MTV with the **correct sign** ---
		FixVec2 mtv;
		if (dx < dy)     // resolve on X
			mtv = new FixVec2(a.GetWorldPosition().x < b.GetWorldPosition().x ? -dx : dx, Fix64.Zero);
		else             // resolve on Y
			mtv = new FixVec2(Fix64.Zero, a.GetWorldPosition().y < b.GetWorldPosition().y ? -dy : dy);

		// --- 3. Decide who moves ---
		FP_Body2D bodyA = a.ParentBody;
		FP_Body2D bodyB = b.ParentBody;

		FP_Body2D movable =
			bodyA.IsStatic ? (bodyB.IsStatic ? null : bodyB) :
			bodyB.IsStatic ? bodyA :
			bodyA.IsDynamic && bodyB.IsDynamic ? null /* later: split 50-50 */ :
			bodyA.IsDynamic ? bodyA : bodyB;     // dynamic vs kinematic

		if (movable == null) return;

		// --- 4. Positional correction (one MoveInput only) ---
		movable.Move(mtv);

		// --- 5. Kill velocity into the surface ---
		FixVec2 n = FixVec2.Normalize(mtv);
		Fix64 vDot = FixVec2.Dot(movable.Velocity, n);
		if (vDot < Fix64.Zero)
			movable.Velocity -= n * vDot;

		// Snap to ground if basically resting
		if (n == FixVec2.up && Fix64.Abs(movable.Velocity.y) < Fix64.FromFloat(0.05f))
			movable.Velocity = new FixVec2(movable.Velocity.x, Fix64.Zero);
	}


}
