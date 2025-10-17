using UnityEngine;


public class CharSelectAPS : AppState
{
	public override void OnEnter()
	{
		base.OnEnter();

		AppManager.LoadScene("CharSelect");


	}




	public override void OnMonoUpdate()
	{

	}
}
