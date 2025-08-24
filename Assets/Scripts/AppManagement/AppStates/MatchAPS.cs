
public enum MatchState
{
	Null,
	Cinematic,
	Gameplay,

}

public class MatchAPS : AppState
{
	private string MatchScene;

	public MatchAPS(string matchScene)
	{
		this.MatchScene = matchScene;


	}


	public override void OnEnter()
	{
		base.OnEnter();

		AppManager.LoadScene(MatchScene);

		
	}

	public override void OnUpdate()
	{

	}
}
