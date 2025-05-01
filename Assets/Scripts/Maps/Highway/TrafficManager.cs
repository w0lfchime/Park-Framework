using UnityEngine;
using System.Collections;
using UnityEngine.Splines;

public class TrafficManager : MonoBehaviour
{
	public GameObject[] vehiclePrefabs;
	public float spawnInterval = 5f;
	public float baseVehicleSpeed = 10f;
	public SplineContainer splines; // Single container with multiple splines
	private Transform vehicleParent;

	void Start()
	{
		FindSplines();
		FindOrCreateVehicleParent();
		StartCoroutine(SpawnTraffic());
	}

	void FindSplines()
	{
		Transform splinesParent = transform.Find("Splines"); // Find child named "Splines"
		if (splinesParent != null)
		{
			splines = splinesParent.GetComponent<SplineContainer>(); // Get the single SplineContainer
		}
		else
		{
			Debug.LogError("TrafficManager: No 'Splines' child object found!");
		}
	}

	void FindOrCreateVehicleParent()
	{
		vehicleParent = transform.Find("Vehicles"); // Try to find an existing "Vehicles" object
		if (vehicleParent == null)
		{
			vehicleParent = new GameObject("Vehicles").transform; // Create one if missing
			vehicleParent.SetParent(transform); // Set it as a child of TrafficManager
		}
	}

	IEnumerator SpawnTraffic()
	{
		while (true)
		{
			yield return new WaitForSeconds(spawnInterval);
			if (splines != null && splines.Splines.Count > 0)
			{
				SpawnVehicle();
			}
		}
	}

	void SpawnVehicle()
	{
		// Randomly pick which spline (lane) to use
		int laneIndex = Random.Range(0, splines.Splines.Count);

		// Randomly pick a vehicle prefab
		GameObject vehiclePrefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];

		// Calculate speed offset for this lane
		float assignedSpeed = baseVehicleSpeed;
		switch (laneIndex)
		{
			case 0:
				assignedSpeed *= 1.2f;
				break;
			case 1:
				assignedSpeed *= 1.4f;
				break;
			case 2:
				assignedSpeed *= 1.3f;
				break;
			default:
				break;
		}

		// Get the spline and evaluate the start position/tangent
		Spline spline = splines.Splines[laneIndex];
		float startT = 0f;
		Vector3 startPosition = spline.EvaluatePosition(startT);
		Quaternion startRotation = Quaternion.LookRotation(spline.EvaluateTangent(startT));

		// Instantiate the vehicle at the proper world space position/rotation, with no parent
		GameObject newVehicle = Instantiate(vehiclePrefab, startPosition, startRotation, null);
		newVehicle.SetActive(false); // Ensure it won't render yet

		// Optionally re-parent it under our "Vehicles" object, preserving world-space transform
		newVehicle.transform.SetParent(vehicleParent, true);

		// Attach and configure our movement script
		VehicleMovement vehicleMovement = newVehicle.AddComponent<VehicleMovement>();
		vehicleMovement.SetUp(splines, laneIndex, assignedSpeed, laneIndex < 2);

		// Now that everything is set, activate the vehicle
		newVehicle.SetActive(true);
	}



}
