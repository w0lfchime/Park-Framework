using System;

public abstract class AppState
{
	protected AppManager App => AppManager.Instance;

	public virtual void OnEnter() 
	{
		LogCore.Log("AppFlow", $"Entering AppState: {GetType().Name}");
	}
	public virtual void OnExit()
	{
		LogCore.Log("AppFlow", $"Exiting AppState: {GetType().Name}");
	}



	public virtual void OnUpdate()
	{
		
	}

	// Optional: override for pause/resume, or frame-step updates
	public virtual void OnFixedUpdate() { }
	public virtual void OnLateUpdate() { }
}
