using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class FixedGameUpdateDriver
{
	public const float FPS = 60.0f;
	private const float TargetDeltaTime = 1f / FPS; // 60Hz
	private float accumulatedTime = 0f;

	public int Clock; 
	public bool ClockEnabled = true;

	public FixedGameUpdateDriver()
	{

	}

	public void MonoUpdate() //called by monobehaviour AppManager
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

		UpdateClock();
	}

	private void UpdateClock()
	{
		if (ClockEnabled)
		{
			Clock++;
		}
	}

	public void PauseClock()
	{
		ClockEnabled = false;
	}

	public void UnpauseClock()
	{
		ClockEnabled = true;
	}
}
