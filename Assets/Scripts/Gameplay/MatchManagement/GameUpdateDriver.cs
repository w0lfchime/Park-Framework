using System.Collections.Generic;
using UnityEngine;

public interface IGameUpdate
{
	void FixedFrameUpdate();
}

public class GameUpdateDriver : MonoBehaviour
{
	private const float TargetDeltaTime = 1f / 60f; // 60Hz
	private float accumulatedTime = 0f;
	private static List<IGameUpdate> fixedUpdateObjects = new List<IGameUpdate>();

	public static void Register(IGameUpdate obj) 
	{
		fixedUpdateObjects.Add(obj);
		LogCore.Log("GameUpdateDriver", $"Registered a {obj.GetType().Name} to the GameUpdateDriver.");
	}
	
	

	public static void Unregister(IGameUpdate obj)
	{
		fixedUpdateObjects.Remove(obj);
		LogCore.Log("GameUpdateDriver", $"Unregistered a {obj.GetType().Name} from the GameUpdateDriver.");
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
		for (int i = 0; i < fixedUpdateObjects.Count; i++)
		{
			fixedUpdateObjects[i].FixedFrameUpdate();
		}
	}
}
