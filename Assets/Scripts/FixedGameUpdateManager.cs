using System.Collections.Generic;
using UnityEngine;

public interface IGameUpdate
{
	void FixedFrameUpdate();
}

public class FixedGameUpdateManager
{
	private const float TargetDeltaTime = 1f / 60f; // 60Hz
	private float accumulatedTime = 0f;
	private static List<IGameUpdate> fixedUpdateObjects = new List<IGameUpdate>();




	public static void Register(IGameUpdate obj) 
	{
		fixedUpdateObjects.Add(obj);
		LogCore.Log("FixedGameUpdateManager", $"Registered a {obj.GetType().Name} to the FixedGameUpdateManager.");
	}
	
	

	public static void Unregister(IGameUpdate obj)
	{
		fixedUpdateObjects.Remove(obj);
		LogCore.Log("FixedGameUpdateManager", $"Unregistered a {obj.GetType().Name} from the FixedGameUpdateManager.");
	}
		


	void Update()
	{
		accumulatedTime += Time.deltaTime;

		while (accumulatedTime >= TargetDeltaTime)
		{
			accumulatedTime -= TargetDeltaTime;
			RunFixedGameUpdate();
		}
	}

	private void RunFixedGameUpdate()
	{
		AppManager.Instance.FixedGameUpdate();

		for (int i = 0; i < fixedUpdateObjects.Count; i++)
		{
			fixedUpdateObjects[i].FixedFrameUpdate();
		}
	}
}
