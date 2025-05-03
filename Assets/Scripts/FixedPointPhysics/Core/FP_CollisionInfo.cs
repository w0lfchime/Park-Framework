
public struct FP_CollisionInfo
{
    public FP_Body Self;
    public FP_Body Other;
    public FixVec2 Normal;     // Direction to resolve from other
    public Fix64 Penetration;  // How deep into other we were
    public FixVec2 ContactPoint; // Optional – for fancier stuff

    public bool IsGround => Normal.y > Fix64.FromFloat(0.5f);
    public bool IsWall => Fix64.Abs(Normal.x) > Fix64.FromFloat(0.5f);

    public FP_CollisionInfo(FP_Body self, FP_Body other, FixVec2 normal, Fix64 depth)
    {
        Self = self;
        Other = other;
        Normal = normal;
        Penetration = depth;
        ContactPoint = FixVec2.zero; // Can be set later
    }
}