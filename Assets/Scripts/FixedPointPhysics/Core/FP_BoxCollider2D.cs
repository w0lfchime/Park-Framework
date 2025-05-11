using UnityEngine;

public class FP_BoxCollider2D : MonoBehaviour
{
    public FP_Body2D ParentBody { get; private set; }

    public FixVec2 Size { get; private set; }
    public FixVec2 Offset { get; private set; }

    public bool IsTrigger { get; private set; } = false;
    public bool IsActive { get; private set; } = true;

    private void Awake()
    {
        FP_GameSpace.Instance.RegisterCollider(this);
    }

    public void Initialize(FP_Body2D parent, BoxCollider2D sourceCollider)
    {
        ParentBody = parent;
        IsTrigger = sourceCollider.isTrigger;

        Offset = new FixVec2(sourceCollider.offset.x, sourceCollider.offset.y);
        Size = new FixVec2(sourceCollider.size.x, sourceCollider.size.y);
        FixVec2 scale = new FixVec2(parent.transform.localScale);
        Size = Size * scale;
    }

    public FixVec2 GetWorldPosition()
    {
        return ParentBody.Position + Offset;
    }
}
