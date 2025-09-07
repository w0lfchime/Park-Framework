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
		FP_PhysicsSpace.Instance.RegisterCollider(this);
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

#if UNITY_EDITOR
	//private void OnDrawGizmos()
	//{
	//	if (ParentBody == null || !IsActive) return;

	//	Vector2 worldPos = GetWorldPosition().ToVector2(); // assuming FixVec2 has a conversion
	//	Vector2 size = Size.ToVector2(); // same assumption

	//	Gizmos.color = IsTrigger ? new Color(1f, 0.5f, 0f, 0.5f) : new Color(0f, 1f, 0f, 0.5f);
	//	Gizmos.DrawWireCube(worldPos, size);
	//}
#endif
}
