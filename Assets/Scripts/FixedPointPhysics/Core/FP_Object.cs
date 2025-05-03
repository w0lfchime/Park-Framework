using UnityEngine;

[DisallowMultipleComponent]
public class FP_Object : MonoBehaviour
{
    [Header("Physics Setup")]
    public bool generateBody = true;
    public bool generateCollider = true;

    [Header("Body Settings")]
    public bool isStatic = false;
    public bool isKinematic = false;
    public bool affectedByGravity = true;
    public float mass = 1f;
    public float drag = 0f;

    [Header("Collision Tags (For Future Use)")]
    public bool isOneWayPlatform = false;
    public bool isWall = false;
    public bool isGround = false;

    [Header("Collider Override")]
    public bool overrideSize = false;
    public Vector2 manualSize = Vector2.one;

    [HideInInspector] public FP_Body Body;
    [HideInInspector] public FP_BoxCollider Collider;

    private void Awake()
    {
        TryGenerate();
    }

    private void TryGenerate()
    {
        if (generateCollider)
            TryCreateCollider();

        if (generateBody)
            TryCreateBody();

        // Hook up collider to body if both exist
        if (Collider != null && Body != null)
            Collider.Body = Body;

        if (Collider != null)
            FP_PhysicsSpace.Instance?.RegisterCollider(Collider);

        if (Body != null)
            Body.Register();
    }

    private void TryCreateCollider()
    {
        var box = GetComponent<BoxCollider2D>();
        if (box == null)
        {
            Debug.LogWarning($"[FP_Object] No BoxCollider2D found on '{name}' but generateCollider is enabled.");
            return;
        }

        Vector2 size = overrideSize ? manualSize : box.size;
        Vector2 center = (Vector2)transform.position + box.offset;

        var fixSize = new FixVec2(Fix64.FromFloat(size.x), Fix64.FromFloat(size.y));

        Collider = new FP_BoxCollider(null, fixSize);
    }

    private void TryCreateBody()
    {
        var fixPos = new FixVec2(
            Fix64.FromFloat(transform.position.x),
            Fix64.FromFloat(transform.position.y)
        );

        Body = new FP_Body
        {
            Position = fixPos,
            isStatic = isStatic,
            isKinematic = isKinematic,
            affectedByGravity = affectedByGravity,
            Mass = Fix64.FromFloat(mass),
            Drag = Fix64.FromFloat(drag),
            isOneWayPlatform = isOneWayPlatform,
            isWall = isWall,
            isGround = isGround
        };
    }

    private void LateUpdate()
    {
        if (Body != null && !isStatic)
        {
            transform.position = new Vector3(
                Body.Position.x.ToFloat(),
                Body.Position.y.ToFloat(),
                transform.position.z);
        }
    }
}
