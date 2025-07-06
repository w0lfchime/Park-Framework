using System.Collections.Generic;
using UnityEngine;

public class FP_GameSpace : MonoBehaviour
{
    public static FP_GameSpace Instance { get; private set; }

    public const float FrameRate = 60f;
    public static readonly Fix64 DeltaTime = Fix64.FromFloat(1f / FrameRate);

    private Fix64 accumulatedTime = new(1);

    // === Physics system state ===
    private readonly List<FP_Body2D> bodies = new();
    private readonly List<FP_BoxCollider2D> colliders = new();

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
        GenerateGameSpace();
    }

    private void GenerateGameSpace()
    {

        LogCore.Log("GameSetup", "Generating gamespace...");
        AssignBodiesFromLayer("FP_Player");
        AssignBodiesFromLayer("FP_Ground");
        AssignBodiesFromLayer("FP_Platform");
        AssignBodiesFromLayer("FP_Wall");

        foreach (var body in bodies)
            body.InitializeBody(); // Let each one complete its setup
    }

    private void AssignBodiesFromLayer(string layerName)
    {
        switch (layerName)
        {
            case "None":
                break;
            default:
                int count = 0;
                int layer = LayerMask.NameToLayer(layerName);
                if (layer == -1)
                {
                    Debug.LogWarning($"Layer '{layerName}' not found.");
                    return;
                }

                // Find all GameObjects in the scene
                foreach (var go in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
                {
                    if (go.layer != layer)
                        continue;

                    // Only assign if it doesn't already have one
                    var body = go.GetComponent<FP_Body2D>();
                    if (body == null)
                        body = go.AddComponent<FP_Body2D>();

                    // Add to bodies list (only if not already tracked)
                    if (!bodies.Contains(body))
                        count++;
                        bodies.Add(body);
                }
                LogCore.Log("PhysicsSetup", $"Found {count} objects in layer {layerName}");
                break;
        }

    }


    private void Update()
    {
        accumulatedTime += Fix64.FromFloat(Time.deltaTime);

        while (accumulatedTime >= DeltaTime)
        {
            StepInput();
            StepGameLogic();
            StepPhysics();
            accumulatedTime -= DeltaTime;
        }
    }

    // === Step functions ===
    private void StepInput()
    {
        // Deterministic input collection goes here (from pre-buffered input or input queue)
    }

    private void StepGameLogic()
    {
        // Game state logic (e.g., timers, FSMs) before physics
    }

    private void StepPhysics()
    {
        // 1. Apply forces only to dynamics
        foreach (var body in bodies)
            body.ApplyForces(DeltaTime);

        // 2. Integrate only dynamics
        foreach (var body in bodies)
            body.Integrate(DeltaTime);

		// 3. Collision detection (include all bodies)
	    int solverIterations = 4;
		for (int k = 0; k < solverIterations; k++)
		{
			for (int i = 0; i < colliders.Count; i++)
			{
				for (int j = i + 1; j < colliders.Count; j++)
				{
					var a = colliders[i];
					var b = colliders[j];

					if (a.IsActive && b.IsActive)
						FP_CollisionSolver.Instance.CheckAndResolve(a, b);
				}
			}
		}


		// 4. Sync all bodies for rendering
		foreach (var body in bodies)
            body.SyncTransform();
    }


    // === Registration ===
    public void RegisterBody(FP_Body2D body)
    {
        if (!bodies.Contains(body))
            bodies.Add(body);
    }

    public void UnregisterBody(FP_Body2D body)
    {
        bodies.Remove(body);
    }

    public void RegisterCollider(FP_BoxCollider2D collider)
    {
        if (!colliders.Contains(collider))
            colliders.Add(collider);
    }

    public void UnregisterCollider(FP_BoxCollider2D collider)
    {
        colliders.Remove(collider);
    }

    // === Optional accessors ===
    public IReadOnlyList<FP_Body2D> Bodies => bodies;
    public IReadOnlyList<FP_BoxCollider2D> Colliders => colliders;
}
