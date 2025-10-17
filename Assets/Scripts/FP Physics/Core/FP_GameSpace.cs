using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FP_GameSpace : MonoBehaviour
{
    public static FP_GameSpace Instance { get; private set; }

	public List<Character> CharacterList = new List<Character>();

	// === Physics system state 
	private readonly List<FP_Body2D> bodies = new();
    private readonly List<FP_BoxCollider2D> colliders = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;
        // Uncomment if you want it to survive scene changes
        // DontDestroyOnLoad(gameObject);

        CommandHandler.RegisterCommand("listcharacters", args =>
        {
            ListCharacters();
        });
    }

    private void Start()
    {
        GenerateGameSpace();

    }

    private void GenerateGameSpace()
    {

        LogCore.Log(LogType.GameSpace, "Generating gamespace...");
        AssignBodiesFromLayer("FP_Character");
        AssignBodiesFromLayer("FP_Ground");
        AssignBodiesFromLayer("FP_Platform");
        AssignBodiesFromLayer("FP_Wall");

        foreach (var body in bodies)
            body.InitializeBody(); // Let each one complete its setup

        LogCore.Log(LogType.GameSpace, "Gamespace setup complete.");
        AppManager.Instance.SystemInputManager.ListPlayers();
    }

    private void AssignBodiesFromLayer(string layerName)
    {
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
            if (go.layer != layer || go.GetComponent<Rigidbody2D>() == null)
                continue;

            // Only assign if it doesn't already have one
            var body = go.GetComponent<FP_Body2D>();
            if (body == null)
                body = go.AddComponent<FP_Body2D>();



            // Add to bodies list (only if not already tracked)
            if (!bodies.Contains(body))
                count++;
            bodies.Add(body);

            if (layerName == "FP_Character")
            {
                Character c = go.GetComponent<Character>();
                if (c == null)
                {
                    LogCore.Log(LogType.PhysicsSetup, "There are objects within the FP_Character layer without a valid character compnent.");
                    DebugCore.StopGame();
                }
                else
                {
                    AddCharacter(c);
                }
            }
        }
        LogCore.Log(LogType.GameSpace, $"Found {count} objects in layer {layerName}");



        switch (layerName) //HACK: random as switch statement
        {
            case "None":
                break;
            default:
                break;
        }

    }

    public void AddCharacter(Character c)
    {
        LogCore.Log(LogType.GameSpace, $"Adding character {c.InstanceName} to the gamespace.");
        CharacterList.Add(c);
        c.PlayerID = CharacterList.Count; //HACK: super hack adding method 
        c.EnterGameSpace();
    }

    public void FixedGameSpaceUpdate()
    {
        foreach (Character c in CharacterList)
        {
            c.FixedFrameUpdate();
        }
    }

    //main driver, called by match appstate
    public void FixedPhysicsSpaceUpdate()
    {
        StepPhysics();
		foreach (Character c in CharacterList)
		{
			c.FixedFrameUpdate();
		}
	}

    // === Step functions ===
    private void StepPhysics()
    {
        // 1. Apply forces only to dynamics
        foreach (var body in bodies)
            body.ApplyForces();

        // 2. Integrate only dynamics
        foreach (var body in bodies)
            body.Integrate();

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



	public void ListCharacters()
	{
		foreach (var character in CharacterList)
		{
			LogCore.Log(LogType.Response, $"{character.InstanceName}");
		}
	}
}
