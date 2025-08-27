
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

public class Ric : Character
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



	//======// /==/==/==/=||[UPDATES & MONO]||==/==/==/==/==/==/==/==/==/ //======//
	#region update_calls
	public void FixedPhysicsUpdate()
	{
		base.FixedPhysicsUpdate();
		//..
	}
	public void FixedFrameUpdate()
	{
		base.FixedFrameUpdate();
		//...
	}
	#endregion update_calls
	//----------------------------------------
	#region mono_virtuals
	#endregion mono_virtuals 
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
		//HACK: SEE BELOW
		for (int i = 3; i < CStateIDs.GenericStateCount(); i++)
		{
			StateBlacklist.Add(i);
		}
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
	protected override void UpdateCharacterData() //TODO: better name 
	{
		base.UpdateCharacterData();
		//...
	}
	#endregion data
	//=//------------------------------------------------------------------------//=//
	#endregion base
	/////////////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[DEBUG]||==/==/==/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region debug
	//=//------------------------------------------------------------------------//=//
	#endregion debug 
	/////////////////////////////////////////////////////////////////////////////////////
}
