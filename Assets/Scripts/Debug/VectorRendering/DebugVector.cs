using UnityEngine;

public sealed class DebugVector
{
	public string Name { get; private set; }
	public Transform Root { get; private set; }
	public Transform ArrowChild { get; private set; }
	public Transform CurrentParent { get; private set; }

	// NEW: all renderers under the prefab (body + tip, etc.)
	private Renderer[] _renderers;
	private MaterialPropertyBlock _mpb;

	private Vector3 _baseLocalScale;
	private float _thicknessScaleApplied;

	public DebugVector(string name, GameObject prefab, Transform parent, float thicknessScale)
	{
		Name = name;

		var go = Object.Instantiate(prefab, parent);
		go.name = $"[DBG] {name}";
		Root = go.transform;
		CurrentParent = parent;

		// Find ALL renderers (handles body + tip)
		_renderers = go.GetComponentsInChildren<Renderer>(true);

		// Keep ArrowChild for convenience (use first renderer if present, else first child)
		if (_renderers != null && _renderers.Length > 0)
			ArrowChild = _renderers[0].transform;
		if (ArrowChild == null && go.transform.childCount > 0)
			ArrowChild = go.transform.GetChild(0);

		_mpb = new MaterialPropertyBlock();

		_baseLocalScale = Root.localScale;
		_thicknessScaleApplied = Mathf.Max(0.0001f, thicknessScale);

		Root.localScale = new Vector3(
			_baseLocalScale.x,
			_baseLocalScale.y * _thicknessScaleApplied,
			_baseLocalScale.z * _thicknessScaleApplied
		);
	}

	public void Reparent(Transform newParent)
	{
		var worldPos = Root.position;
		var worldRot = Root.rotation;

		Root.SetParent(newParent, worldPositionStays: true);
		Root.position = worldPos;
		Root.rotation = worldRot;

		CurrentParent = newParent;
	}

	public void UpdateVisual(Transform origin, FixVec2 v, Color color, float lengthScale, bool parentToOrigin)
	{
		if (!parentToOrigin)
		{
			if (origin != null)
			{
				Root.position = origin.position;
				Root.rotation = origin.rotation;
			}
			else
			{
				Root.position = Vector3.zero;
				Root.rotation = Quaternion.identity;
			}
		}
		else
		{
			Root.localPosition = Vector3.zero;
			Root.localRotation = Quaternion.identity;
		}

		float vx = v.x.ToFloat();
		float vy = v.y.ToFloat();
		float angleDeg = Mathf.Atan2(vy, vx) * Mathf.Rad2Deg;

		Root.rotation = Root.rotation * Quaternion.AngleAxis(angleDeg, Vector3.forward);

		float mag = Mathf.Sqrt(vx * vx + vy * vy);
		float xScale = Mathf.Max(0f, mag * lengthScale);

		var ls = Root.localScale;
		Root.localScale = new Vector3(xScale, ls.y, ls.z);

		// Apply color to ALL renderers via MPB (no material instantiation)
		if (_renderers != null && _renderers.Length > 0)
		{
			_mpb.Clear();
			_mpb.SetColor("_Color", color);      // Standard/Built-in
			_mpb.SetColor("_BaseColor", color);  // URP/HDRP-compatible
			foreach (var r in _renderers)
			{
				if (r != null) r.SetPropertyBlock(_mpb);
			}
		}
	}

	public void Destroy()
	{
		if (Root != null)
			Object.Destroy(Root.gameObject);
		Root = null;
		ArrowChild = null;
		_renderers = null;
		_mpb = null;
		CurrentParent = null;
	}
}
