//using UnityEngine;

//public class GenericCharacter : Character
//{
//    //This is a character 

//    public override void SetMemberVariables()
//    {
//        base.SetMemberVariables();
//		//To avoid using the class name "Generic", generic character is the only character with "Character" in the class name.
//		//Thus, the character's member variable class name should be set as just "Generic" 
//		this.characterClassName = "Generic";

//    }

//    protected override void CharacterStart()
//	{
//		//HACK
//		//string playerName = "3 pushups, fucked by a black guy";

//		//AppManager.Instance.AddPlayer(playerName);

//		//this.player = AppManager.Instance.players[playerName];

//		//player.character = this;
//	}
//	protected override void CharacterUpdate()
//	{


//	}

//	protected override void CharacterFixedFrameUpdate()
//	{
//		// Implement character-specific physics update logic here
//	}

//    protected override void CharacterLateUpdate()
//    {
        
//    }

//    public override void CharacterSetup()
//	{
//		base.CharacterSetup();


//		//Addtional code pretaining to character
//	}

//	public override void UpdateActiveCharacterData()
//	{
//		base.UpdateActiveCharacterData();

//		this.name = "Generic";
//	}
//	protected override void RegisterCharacterStates()
//	{
//		base.RegisterCharacterStates();

//		LogCore.Log("GenericCharacterHighDetail", "Registering character states...");

//		//common 
//		stateDict.Add("IdleAirborne", new GenericIdleAirborne(this));
//		stateDict.Add("IdleGrounded", new GenericIdleGrounded(this));
//		stateDict.Add("Walk", new GenericWalk(this));
//		stateDict.Add("Run", new GenericRun(this));
//		stateDict.Add("Jump", new GenericJump(this));
//		//(dev)
//		stateDict.Add("Flight", new GenericFlight(this));
		

//	}

//}