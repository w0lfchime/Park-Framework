


using UnityEngine;

public class TestBody : MonoBehaviour
{
    private FP_Body body;

    void Start()
    {
        body = new FP_Body
        {
            Position = new FixVec2(transform.position),
            Velocity = FixVec2.zero,
            affectedByGravity = true,
            Drag = Fix64.FromFloat(0.1f)
        };
        body.Register();
    }

    void Update()
    {
        if (body != null)
            transform.position = new Vector3(body.Position.x.ToFloat(), body.Position.y.ToFloat(), 0f);

    }
}
