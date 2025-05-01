using UnityEngine;

public abstract class SubState
{
    protected AppState _appState { get; private set; }

    public SubState(AppState parentState)
    {
        _appState = parentState;
    }

    public virtual void Enter()
    {
        LogCore.Log("SubState", "Entering SubState: {this.GetType().Name}");
    }

    public virtual void Exit()
    {
        LogCore.Log("SubState", "Exiting SubState: {this.GetType().Name}");
    }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void Pause() { }

    public virtual void Resume() { }

}
