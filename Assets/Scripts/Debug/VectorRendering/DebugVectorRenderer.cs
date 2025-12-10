using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour singleton for rendering named debug vectors.
/// Call RenderVector(name, origin, vector, color) each frame you want a vector shown/updated.
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("Debug/Debug Vector Renderer")]
public sealed class DebugVectorRenderer : MonoBehaviour
{
	// --- Singleton ---
	private static DebugVectorRenderer _instance;
	public static DebugVectorRenderer Instance
	{
		get
		{
			if (_instance == null)
			{
				// Try find in scene
				_instance = FindFirstObjectByType<DebugVectorRenderer>();
				if (_instance == null)
				{
					var go = new GameObject("[DebugVectorRenderer]");
					_instance = go.AddComponent<DebugVectorRenderer>();
				}
			}
			return _instance;
		}
	}

	[Header("Prefab & Scaling")]
	[Tooltip("Prefab with an EMPTY parent at (0,0,0) and an ARROW child pointing +X. Scaling parent X should stretch length.")]
	[SerializeField] private GameObject vectorPrefab;

	[Tooltip("World units per Fix64 unit of vector magnitude (purely proportional).")]
	[SerializeField] private float lengthScale = 1.0f;

	[Tooltip("Optional global thickness/size multiplier applied to the prefab's Y/Z scale of the parent.")]
	[SerializeField] private float thicknessScale = 1.0f;

	[Header("Parenting")]
	[Tooltip("If true, vectors become children of their origin Transform. If false, they live under this manager and follow origin via FP_Position/rotation.")]
	[SerializeField] private bool parentToOrigin = true;

	// name -> DebugVector
	private readonly Dictionary<string, DebugVector> _vectors = new Dictionary<string, DebugVector>();

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this;
		// Optional: keep across scenes if you want
		// DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// Main API: create or update a named vector.
	/// </summary>
	/// <param name="vectorName">Unique key. New creates, existing updates.</param>
	/// <param name="origin">Where to anchor the vector (can be null for world origin).</param>
	/// <param name="vector">Direction+length in FixVec2 (XY plane). Magnitude drives X scale.</param>
	/// <param name="color">Color to apply to the arrow mesh renderer.</param>
	public void RenderVector(string vectorName, Transform origin, FixVec2 vector, Color color)
	{
		if (vectorPrefab == null)
		{
			Debug.LogWarning("[DebugVectorRenderer] No vectorPrefab assigned.");
			return;
		}

		if (!_vectors.TryGetValue(vectorName, out var dv) || dv == null)
		{
			dv = new DebugVector(vectorName, vectorPrefab, parentToOrigin ? origin : transform, thicknessScale);

			_vectors[vectorName] = dv;
		}

		// If our parenting policy is "parent to origin", ensure reparent if origin changed.
		if (parentToOrigin && dv.CurrentParent != origin)
		{
			dv.Reparent(origin);
		}

		// Update transform/visuals
		dv.UpdateVisual(origin, vector, color, lengthScale, parentToOrigin);
	}

	/// <summary>Remove and destroy a named vector.</summary>
	public void RemoveVector(string vectorName)
	{
		if (_vectors.TryGetValue(vectorName, out var dv) && dv != null)
		{
			dv.Destroy();
		}
		_vectors.Remove(vectorName);
	}

	/// <summary>Clear all debug vectors.</summary>
	public void ClearAll()
	{
		foreach (var kvp in _vectors)
			kvp.Value?.Destroy();
		_vectors.Clear();
	}

	// Optional convenience overloads:
	public void RenderVector(string vectorName, Transform origin, Vector2 vec, Color color)
		=> RenderVector(vectorName, origin, FixVec2.FromVector2(vec), color);

	public void RenderVector(string vectorName, Transform origin, Vector3 vec, Color color)
		=> RenderVector(vectorName, origin, FixVec2.FromVector3(vec), color);
}
