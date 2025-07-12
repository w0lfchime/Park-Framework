using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IGameUpdate
{
	void FixedFrameUpdate();
}

public static class FixedGameUpdateDriver
{
	private const float TargetDeltaTime = 1f / 60f; // 60Hz
	private static float accumulatedTime = 0f;

	public static int Clock; 
	public static bool ClockEnabled = false;
	private static int pauseDuration;

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

		ClockUpdateLogic();
		
	}

	private static void ClockUpdateLogic()
	{
		if (ClockEnabled && pauseDuration == 0)
		{
			Clock++;
		}
		else if (pauseDuration > 0)
		{
			pauseDuration--;
		}
		else
		{
			pauseDuration = 0;
		}
	}

	public static void PauseClock()
	{
		ClockEnabled = false;
	}
	public static void PauseClock(int pauseFrameCount)
	{
		pauseDuration = pauseFrameCount;
	}


	public static void UnpauseClock()
	{
		ClockEnabled = true;
		pauseDuration = 0;
	}
}
