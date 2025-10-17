using System;

public abstract class AppState
{
	protected AppManager App => AppManager.Instance;

	public virtual void OnEnter() 
	{
		LogCore.Log(LogType.AppState, $"Entering AppState: {GetType().Name}");
	}
	public virtual void OnExit()
	{
		LogCore.Log(LogType.AppState, $"Exiting AppState: {GetType().Name}");
	}


	public virtual void FixedGameUpdate()
	{

	}

	public virtual void FixedPhysicsUpdate()
	{

	}


	public virtual void OnMonoUpdate()
	{
		
	}

	// Optional: override for pause/resume, or frame-step updates
	public virtual void OnMonoFixedUpdate() { }
	public virtual void OnMonoLateUpdate() { }
}
