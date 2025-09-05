using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class FixedGameUpdateDriver
{
	public const float FPS = 60.0f;
	private const float TargetDeltaTime = 1f / FPS; // 60Hz
	private float accumulatedTime = 0f;

	public int Clock; 
	public bool ClockEnabled = false;
	private int pauseDuration;

	public FixedGameUpdateDriver()
	{

	}

	public void Update() //called by monobehaviour AppManager
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
		AppManager.Instance.FixedGameUpdate(); //circular calling, dont worry about it, just feels the best for me

		ClockUpdateLogic();
	}

	private void ClockUpdateLogic()
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

	public void PauseClock()
	{
		ClockEnabled = false;
	}
	public void PauseClock(int pauseFrameCount)
	{
		pauseDuration = pauseFrameCount;
	}


	public void UnpauseClock()
	{
		ClockEnabled = true;
		pauseDuration = 0;
	}
}
