
//----------------------------------------------------------------------------------
// Copyright (2025) NITER
//
// This code is part of the PARK-v6 Unity Framework. It is proprietary software.  
// Unauthorized use, modification, or distribution is not permitted without  
// explicit permission from the owner.  
//----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class Storm : Character
{
	//======// /==/==/==/=||[FIELDS]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region fields
	//=//-------------------------------------------------------------------------//=//
	#endregion fields
	/////////////////////////////////////////////////////////////////////////////////////





	//======// /==/==/==/=||[LOCAL]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//=//------------------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[BASE]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region base 
	//Base methods and their helpers 
	//=//-----|Setup|------------------------------------------------------------//=//
	#region setup
	protected override void CharacterSetup()
	{
		base.CharacterSetup();
		//...
	}
	protected override void RegisterCommands()
	{
		base.RegisterCommands();
		//...
	}
	protected override void SetMemberVariables()
	{
		base.SetMemberVariables();
		//...
	}
	protected override void SetReferences()
	{
		base.SetReferences();
		//...
	}
	#endregion setup
	//=//-----|Data|-------------------------------------------------------------//=//
	#region data
	protected override void ProcessInput()
	{
		base.ProcessInput();
		//...
	}
	protected override void UpdateACD()
	{
		base.UpdateACD();
		//...
	}
	protected override void UpdateCharacterData() //TODO: better name 
	{
		base.UpdateCharacterData();
		//...
	}
	#endregion data
	//=//-----|Mono|------------------------------------------------------------//=//
	#region mono
	protected override void CharacterAwake()
	{
		//...
	}
	protected override void CharacterStart()
	{
		//...
	}
	protected override void CharacterUpdate()
	{
		//...
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			inputEnabled = !inputEnabled;
		}
	}
	protected override void CharacterFixedFrameUpdate()
	{
		//...
	}
	protected override void CharacterFixedPhysicsUpdate()
	{
		//...
	}
	protected override void CharacterLateUpdate()
	{
		//...
	}
	#endregion mono
	//=//------------------------------------------------------------------------//=//
	#endregion base
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[DEBUG]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region debug
	//=//------------------------------------------------------------------------//=//
	#endregion debug 
	/////////////////////////////////////////////////////////////////////////////////////
}
