using System.Collections.Generic;

public static class FP_CollisionSystem
{
    public static void ProcessCollisions(List<FP_BoxCollider> colliders)
    {
        // Clear previous frame contacts
        foreach (var col in colliders)
            col.Body.CollisionsThisFrame.Clear();

        for (int i = 0; i < colliders.Count; i++)
        {
            var a = colliders[i];
            for (int j = i + 1; j < colliders.Count; j++)
            {
                var b = colliders[j];

                if (!a.Overlaps(b))
                    continue;

                // Calculate overlap resolution direction
                FixVec2 direction = b.Body.Position - a.Body.Position;
                FixVec2 normal = ResolveNormal(direction);
                Fix64 depth = EstimatePenetration(a, b, normal);

                var infoA = new FP_CollisionInfo(a.Body, b.Body, -normal, depth);
                var infoB = new FP_CollisionInfo(b.Body, a.Body, normal, depth);

                a.Body.CollisionsThisFrame.Add(infoA);
                b.Body.CollisionsThisFrame.Add(infoB);
            }
        }
    }

    private static FixVec2 ResolveNormal(FixVec2 delta)
    {
        Fix64 absX = Fix64.Abs(delta.x);
        Fix64 absY = Fix64.Abs(delta.y);

        return absX > absY
            ? new FixVec2(delta.x >= Fix64.Zero ? Fix64.FromFloat(1f) : Fix64.FromFloat(-1f), Fix64.FromFloat(0))
            : new FixVec2(Fix64.FromFloat(0), delta.y >= Fix64.Zero ? Fix64.FromFloat(1f) : Fix64.FromFloat(-1f));
    }

    private static Fix64 EstimatePenetration(FP_BoxCollider a, FP_BoxCollider b, FixVec2 normal)
    {
        FixVec2 aMin = a.Min;
        FixVec2 aMax = a.Max;
        FixVec2 bMin = b.Min;
        FixVec2 bMax = b.Max;

        if (normal.x != Fix64.FromFloat(0))
        {
            return Fix64.Min(aMax.x - bMin.x, bMax.x - aMin.x);
        }
        else
        {
            return Fix64.Min(aMax.y - bMin.y, bMax.y - aMin.y);
        }
    }
}
