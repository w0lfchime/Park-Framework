using UnityEngine;

public class AppCameraController : MonoBehaviour
{
	public enum CameraMode
	{
		Disabled,
		FreeCam
	}

	[Header("Mode Settings")]
	public CameraMode currentMode = CameraMode.Disabled;


	//-------------------------------------
	[Header("FreeCam Settings")]
	public float moveSpeed = 15f;
	public float fastMoveMultiplier = 5f;
	public float lookSensitivity = 5f;

	private float yaw;
	private float pitch;

	// Store original transform for restoring after FreeCam
	private Vector3 savedPosition;
	private Quaternion savedRotation;
	//------------------------------------

	void Start()
	{
		Vector3 euler = transform.rotation.eulerAngles;
		yaw = euler.y;
		pitch = euler.x;
	}

	void Update()
	{
		switch (currentMode)
		{
			case CameraMode.Disabled:
				// Do nothing
				break;

			case CameraMode.FreeCam:
				UpdateFreeCam();
				break;
		}
	}

	private void UpdateFreeCam()
	{
		// --- Mouse Look (hold right-click) ---
		if (Input.GetMouseButton(1))
		{
			float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
			float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

			yaw += mouseX;
			pitch -= mouseY;
			pitch = Mathf.Clamp(pitch, -89f, 89f);

			transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
		}

		// --- Movement ---
		float speed = moveSpeed;
		if (Input.GetKey(KeyCode.LeftShift))
			speed *= fastMoveMultiplier;

		Vector3 move = Vector3.zero;
		if (Input.GetKey(KeyCode.W)) move += transform.forward;
		if (Input.GetKey(KeyCode.S)) move -= transform.forward;
		if (Input.GetKey(KeyCode.A)) move -= transform.right;
		if (Input.GetKey(KeyCode.D)) move += transform.right;
		if (Input.GetKey(KeyCode.E)) move += transform.up;
		if (Input.GetKey(KeyCode.Q)) move -= transform.up;

		transform.position += move * speed * Time.deltaTime;
	}

	public void SetMode(CameraMode newMode)
	{
		if (newMode == currentMode) return;

		// --- Handle exiting current mode ---
		switch (currentMode)
		{
			case CameraMode.FreeCam:
				// restore original transform when leaving FreeCam
				transform.position = savedPosition;
				transform.rotation = savedRotation;
				break;
		}

		// --- Handle entering new mode ---
		switch (newMode)
		{
			case CameraMode.FreeCam:
				// save current transform for later restore
				savedPosition = transform.position;
				savedRotation = transform.rotation;

				// initialize yaw/pitch to match current rotation
				Vector3 euler = transform.rotation.eulerAngles;
				yaw = euler.y;
				pitch = euler.x;
				break;
		}

		currentMode = newMode;
	}
}
