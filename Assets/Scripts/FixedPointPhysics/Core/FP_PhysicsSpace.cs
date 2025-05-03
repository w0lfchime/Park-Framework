using System.Collections.Generic;
using UnityEngine;

public class FP_PhysicsSpace : MonoBehaviour
{
    public static FP_PhysicsSpace Instance { get; private set; }

    // Constants
    // Changes in PhysicsSpace.cs
    public const float TickRate = 60f;
    public static readonly Fix64 DeltaTime = Fix64.FromFloat(1f / TickRate);

    private Fix64 accumulatedTime = new Fix64(1);

    private readonly List<FP_Body> bodies = new List<FP_Body>();
    private readonly List<FP_BoxCollider> colliders = new List<FP_BoxCollider>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        GeneratePhysicsSpace();
    }


    private void GeneratePhysicsSpace()
    {
        //in my scene, check every object that is a lower child of this one for:

        //
    }


    private void Update()
    {
        accumulatedTime += Fix64.FromFloat(Time.deltaTime);


        while (accumulatedTime >= DeltaTime)
        {
            StepPhysics();
            accumulatedTime -= DeltaTime;
        }
    }

    private void StepPhysics()
    {
        foreach (var body in bodies)
            body.Step(DeltaTime);

        FP_CollisionSystem.ProcessCollisions(colliders);

        // (Later) Collision detection + resolution here
    }

    public void RegisterBody(FP_Body body)
    {
        if (!bodies.Contains(body))
            bodies.Add(body);
    }
    public void RegisterCollider(FP_BoxCollider collider)
    {
        if (!colliders.Contains(collider))
            colliders.Add(collider);
    }
    public void UnregisterBody(FP_Body body) => bodies.Remove(body);
    public void UnregisterCollider(FP_BoxCollider collider) => colliders.Remove(collider);
    public void ClearAll() { bodies.Clear(); colliders.Clear(); }
}
