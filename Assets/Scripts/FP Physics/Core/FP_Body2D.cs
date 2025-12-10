using System.Collections.Generic;
using UnityEngine;

public enum FP_BodyType
{
    Static,
    Dynamic,
    Kinematic
}


public class FP_Body2D : MonoBehaviour
{

    [SerializeField]
    private FP_BodyType bodyType;

    public bool IsStatic => bodyType == FP_BodyType.Static;
    public bool IsKinematic => bodyType == FP_BodyType.Kinematic;
    public bool IsDynamic => bodyType == FP_BodyType.Dynamic;


    // === Body State ===
    public FixVec2 Position { get; set; }
    public FixVec2 Velocity { get; set; }
    public Fix64 Mass => mass;
    public Fix64 InverseMass => mass.RawValue == 0 ? Fix64.Zero : Fix64.FromFloat(1f) / mass;

    [SerializeField] private Fix64 mass = Fix64.FromFloat(1f);
    [SerializeField] private bool UseGravity = true;

    private FixVec2 accumulatedForces = FixVec2.zero;

    public List<FP_BoxCollider2D> colliders = new();


    //Debug
    

    public void InitializeBody()
    {
        Position = new FixVec2(transform.position);
        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            Velocity = FixVec2.zero;
            mass = Fix64.FromFloat(rb2d.mass);
            UseGravity = rb2d.gravityScale != 0;

            // Convert Rigidbody2D body type
            switch (rb2d.bodyType)
            {
                case RigidbodyType2D.Static:
                    bodyType = FP_BodyType.Static;
                    mass = Fix64.Zero;
                    break;

                case RigidbodyType2D.Kinematic:
                    bodyType = FP_BodyType.Kinematic;
                    break;

                case RigidbodyType2D.Dynamic:
                    bodyType = FP_BodyType.Dynamic;
                    break;
            }

            Destroy(rb2d);
        }
        // Convert Unity BoxCollider2Ds to FP_BoxCollider2Ds
        var unityColliders = GetComponents<BoxCollider2D>();
        foreach (var box in unityColliders)
        {
            // Add FP_BoxCollider2D
            var newCollider = gameObject.AddComponent<FP_BoxCollider2D>();
            newCollider.Initialize(this, box);
            colliders.Add(newCollider);

            // Disable PhysX collider
            Destroy(box);
        }
    }


    public void ApplyForces()
    {


        if (!IsDynamic) return;

        if (UseGravity)
        {
            FixVec2 gravity = new(0.0f, -9.81f);
            accumulatedForces += gravity * mass;
        }

        FixVec2 acceleration = accumulatedForces * InverseMass;
        Velocity += acceleration;

        ClearForces();
    }



    public void Integrate()
    {
        if (!IsDynamic) return;
        Position += Velocity; //* deltaTime;
    }




    public void Move(FixVec2 delta)
    {
        if (IsStatic) return;
        Position += delta;
    }



    public void AddForce(FixVec2 force)
    {
        accumulatedForces += force;
    }



    public void ClearForces()
    {
        accumulatedForces = FixVec2.zero;
    }



    public void SetVelocity(FixVec2 newVelocity)
    {
        if (IsKinematic)
            Velocity = newVelocity;
    }




    public void SyncTransform()
    {
        transform.position = Position.ToVector3();
    }




    //debug
    private void OnDrawGizmosSelected()
    {
        if (colliders == null || colliders.Count == 0) return;

        Gizmos.color = Color.yellow;

        foreach (var collider in colliders)
        {

            if (collider == null || !collider.IsActive) continue;
            
            var worldPos = collider.GetWorldPosition();
            var size = collider.Size;

            Vector3 center = new Vector3(worldPos.x.ToFloat(), worldPos.y.ToFloat(), 0f);
            Vector3 extents = new Vector3(size.x.ToFloat(), size.y.ToFloat(), 0f);

            Gizmos.DrawWireCube(center, extents);
        }
    }

}
