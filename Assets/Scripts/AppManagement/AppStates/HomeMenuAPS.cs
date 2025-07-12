


public class HomeMenuAPS : AppState
{
	public override void OnEnter()
	{
		base.OnEnter();

		AppManager.LoadScene("HomeMenu");

		AppManager.Instance.systemInputManager.SetState(SysInputManagerState.Debug);
	}

	public override void OnUpdate()
	{

	}
}
