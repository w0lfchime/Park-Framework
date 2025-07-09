using System.Collections.Generic;
using UnityEngine;

public interface IGameUpdate
{
	void FixedFrameUpdate();
}

public static class FixedGameUpdateDriver
{
	private const float TargetDeltaTime = 1f / 60f; // 60Hz
	private static float accumulatedTime = 0f;
	private static List<IGameUpdate> fixedUpdateObjects = new List<IGameUpdate>();

	public static void Register(IGameUpdate obj)
	{
		fixedUpdateObjects.Add(obj);
		LogCore.Log("FixedGameUpdateDriver", $"Registered a {obj.GetType().Name} to the FixedGameUpdateDriver.");
	}

	public static void Unregister(IGameUpdate obj)
	{
		fixedUpdateObjects.Remove(obj);
		LogCore.Log("FixedGameUpdateDriver", $"Unregistered a {obj.GetType().Name} from the FixedGameUpdateDriver.");
	}

	public static void Update()
	{
		accumulatedTime += Time.deltaTime;

		while (accumulatedTime >= TargetDeltaTime)
		{
			accumulatedTime -= TargetDeltaTime;
			RunFixedGameUpdate();
		}
	}

	private static void RunFixedGameUpdate()
	{
		AppManager.Instance.FixedGameUpdate();

		for (int i = 0; i < fixedUpdateObjects.Count; i++)
		{
			fixedUpdateObjects[i].FixedFrameUpdate();
		}
	}
}
