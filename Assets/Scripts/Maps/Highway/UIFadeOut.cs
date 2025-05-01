using System.Collections;
using UnityEngine;

public class UIFader : MonoBehaviour
{
	[Header("References")]
	public CanvasGroup[] uiElements; // UI elements to fade out
	public Renderer[] renderers; // 3D objects to fade out
	public Character character; // Reference to the character

	[Header("Fade Settings")]
	public float fadeDuration = 1.0f; // Time to fade out
	public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); // Default easing

	private bool isFading = false;

	private void Update()
	{
		if (!isFading && character != null && character.velocity != Vector3.zero)
		{
			FadeOut();
		}
	}

	public void FadeOut()
	{
		if (!isFading)
			StartCoroutine(FadeOutRoutine());
	}

	private IEnumerator FadeOutRoutine()
	{
		isFading = true;
		float elapsed = 0f;

		// Get initial transparency for 3D objects
		MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
		float[] initialAlphas = new float[renderers.Length];

		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].GetPropertyBlock(propBlock);
			if (renderers[i].material.HasProperty("_Color"))
				initialAlphas[i] = renderers[i].material.color.a;
		}

		while (elapsed < fadeDuration)
		{
			elapsed += Time.deltaTime;
			float alpha = fadeCurve.Evaluate(elapsed / fadeDuration);

			// Fade UI elements
			foreach (CanvasGroup cg in uiElements)
				cg.alpha = alpha;

			// Fade 3D objects
			for (int i = 0; i < renderers.Length; i++)
			{
				if (renderers[i] == null) continue;
				renderers[i].GetPropertyBlock(propBlock);
				if (renderers[i].material.HasProperty("_Color"))
				{
					Color color = renderers[i].material.color;
					color.a = initialAlphas[i] * alpha;
					propBlock.SetColor("_Color", color);
					renderers[i].SetPropertyBlock(propBlock);
				}
			}

			yield return null;
		}

		// Disable UI and 3D objects after fade
		foreach (CanvasGroup cg in uiElements)
			cg.gameObject.SetActive(false);
		foreach (Renderer r in renderers)
			r.gameObject.SetActive(false);

		isFading = false;
	}
}
