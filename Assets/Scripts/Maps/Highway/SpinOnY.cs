using UnityEngine;

public class SpinOnY : MonoBehaviour
{
	public float rotationSpeed = 100f; // Degrees per second

	void Update()
	{
		transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
	}
}
