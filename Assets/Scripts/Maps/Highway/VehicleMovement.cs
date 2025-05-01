using UnityEngine;
using UnityEngine.Splines;

public class VehicleMovement : MonoBehaviour
{
	private SplineContainer splinePath;
	private int splineIndex; // Index of the specific spline to follow
	private float speed;
	private float progress = 0f;
	private bool reverse;

	public void SetUp(SplineContainer splineContainer, int laneIndex, float moveSpeed, bool moveReverse)
	{
		splinePath = splineContainer;
		splineIndex = laneIndex;
		speed = moveSpeed;
		reverse = moveReverse;
		progress = reverse ? 0.99f : 0.01f;  // Start at the correct end
	}

	private bool initialized = false;

	void Update()
	{
		if (!initialized)
		{
			initialized = true; // Skip movement for first frame
			return;
		}

		if (splinePath == null || splinePath.Splines.Count <= splineIndex) return;

		float direction = reverse ? -1f : 1f;
		progress += (speed / splinePath.Splines[splineIndex].GetLength()) * Time.deltaTime * direction;

		if ((reverse && progress <= 0f) || (!reverse && progress >= 1f))
		{
			Destroy(gameObject);
			return;
		}

		transform.position = splinePath.EvaluatePosition(splineIndex, progress);
		Quaternion forwardRotation = Quaternion.LookRotation(splinePath.EvaluateTangent(splineIndex, progress));

		if (reverse)
		{
			forwardRotation *= Quaternion.Euler(0, 180, 0);
		}

		transform.rotation = forwardRotation;
	}

}
