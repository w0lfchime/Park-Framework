using NUnit.Framework;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class Match : AppState
{
    public GameMode gameMode;
    public Map map;

    public List<Character> playerCharacters;

    Match(GameMode gm, Map map)
    {
        this.gameMode = gm;
        this.map = map;

    }


    public void SetupMap()
    {

    }

    public void SpawnAllCharacters()
    {

    }

    //Standard
    public void Enter()
    {
       

    }


	public override void Update()
	{
		base.Update();

        LogCore.Log("TEST AMTCH");
	}
}
