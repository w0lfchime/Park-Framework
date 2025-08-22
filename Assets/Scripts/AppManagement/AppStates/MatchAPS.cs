
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

		
	}

	public override void OnUpdate()
	{

	}
}
