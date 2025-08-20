
public enum MatchState
{
	Null,
	Cinematic,
	Gameplay,

}

public class MatchAPS : AppState
{
	public override void OnEnter()
	{
		base.OnEnter();

		AppManager.LoadScene("HomeMenu");

		AppManager.Instance.SystemInputManager.SetState(SysInputManagerState.Debug);
	}

	public override void OnUpdate()
	{

	}
}
