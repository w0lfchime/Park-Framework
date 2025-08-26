using system;
using system.security.cryptography;
using unity.visualscripting.fullserializer;
using unityengine;

public class physicalstate : characterstateold
{
	//level 2 state 
	//======// /==/==/==/=||[local fields]||==/==/==/==/==/==/==/==/==/ //======//
	#region local_fields
	private int? ongroundingholdframes = 5;
	private int? onungroundingholdframes = 5;
	//=//----------------------------------------------------------------//=//
	#endregion local_fields
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[local]||=/==/==/==/==/==/==/==/==/==/==/==/ //======//
	#region local
	//=//----------------------------------------------------------------//=//
	#endregion local
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/=||[base]||=/==/==/==/==/==/==/==/==/==/==/==/ //======// performance state
	//overrides of the base class, performance state.
	#region base
	//=//-----|setup|----------------------------------------------------//=//
	#region setup
	public physicalstate(performancecsm sm, character character) : base(sm, character)
	{
		//...
	}
	protected override void setstatereferences()
	{
		base.setstatereferences();
		//...
	}
	public override void setstatemembers()
	{
		base.setstatemembers();
		//...
	}
	#endregion setup
	//=//-----|data management|------------------------------------------//=//
	#region data_management
	protected override void pollinput()
	{
		base.pollinput();
		//...
	}
	protected override void setonentry()
	{
		base.setonentry();
		//...
		physicaldataupdates(); //call here to prevent nulls or whatever
	}
	protected override void perframe()
	{
		base.perframe();
		//...
	}
	#endregion data_management
	//=//-----|routing|--------------------------------------------------//=//
	#region routings
	protected override void routestate()
	{
		//...
		handlegrounding();
		handlejump();
		base.routestate();
	}
	protected override void routestatefixed()
	{
		//...
		base.routestatefixed();
	}
	#endregion routing
	//=//-----|flow|-----------------------------------------------------//=//
	#region flow
	public override void enter()
	{
		base.enter();
		//...
	}
	public override void exit()
	{
		base.exit();
		//...
	}
	#endregion flow
	//=//-----|mono|-----------------------------------------------------//=//
	#region mono
	public override void update()
	{
		base.update();
		//...
		physicaldataupdates();
		handlenaturalrotation();
	}
	public override void fixedframeupdate()
	{
		setgrounding();
		//...
		base.fixedframeupdate();
	}
	public override void fixedphysicsupdate()
	{
		watchgrounding();
		applygravity();
		//...
		base.fixedphysicsupdate();
	}
	public override void lateupdate()
	{
		//...
		base.lateupdate();
	}
	#endregion mono
	//=//-----|debug|----------------------------------------------------//=//
	#region debug
	public override bool verifystate()
	{
		return base.verifystate();
	}
	#endregion debug
	//=//----------------------------------------------------------------//=//
	#endregion base
	/////////////////////////////////////////////////////////////////////////////




	//======// /==/==/==/==||[level 1]||==/==/==/==/==/==/==/==/==/==/ //======// character state

	#region level_1
	//=//----------------------------------------------------------------//=//
	#endregion level_1
	/////////////////////////////////////////////////////////////////////////////





	//======// /==/==/==/==||[level 2]||==/==/==/==/==/==/==/==/==/==/ //======// physical state
	#region level_2
	//=//-----|data management|-----------------------------------------//=//
	#region data_management
	protected virtual void physicaldataupdates()
	{
		ch.position = ch.transform.position;

		vector3 lv = rb.linearvelocity;
		ch.velocity = lv;
		ch.velocityx = lv.x;
		ch.velocityy = lv.y;
		ch.characterspeed = lv.magnitude;

		//debug 
		ch.updatedebugvector("velocity", lv, color.green);

	}
	#endregion data_management
	//=//-----|force|---------------------------------------------------//=//
	#region force
	public virtual void addforce(string forcename, vector3 force)
	{
		ch.updatedebugvector(forcename, force, color.yellow);

		ch.appliedforce += force;
	}
	public virtual void addimpulseforce(string forcename, vector3 impulseforce)
	{
		ch.stampdebugvector(forcename, impulseforce, color.red);
		ch.appliedimpulseforce += impulseforce;
	}
	protected virtual void addforcebytargetvelocity(string forcename, vector3 targetvelocity, float forcefactor)
	{
		//debug
		string tvname = $"{forcename}_targetvelocity";
		ch.updatedebugvector(tvname, targetvelocity, color.white);

		//force
		vector3 forcebytargetvelocity = vector3.zero;
		forcebytargetvelocity += targetvelocity - ch.velocity;
		forcebytargetvelocity *= forcefactor;
		addforce(forcename, forcebytargetvelocity);
	}
	protected virtual void applygravity()
	{
		vector3 gravforcevector = vector3.up * ch.acs.gravityterminalvelocity;
		addforce("gravity", gravforcevector);
	}
	#endregion force
	//=//-----|grounding|-----------------------------------------------//=//
	#region grounding
	protected virtual void watchgrounding()
	{
		float sphereradius = cc.radius;
		vector3 capsuleraycaststart = ch.transform.position + new vector3(0, sphereradius + 0.1f, 0);

		unityengine.debug.drawray(capsuleraycaststart, vector3.down * ch.acs.groundcheckingdistance, color.red);
		unityengine.debug.drawray(capsuleraycaststart + new vector3(0.1f, 0, 0), vector3.down * ch.acs.isgroundeddistance, color.blue);

		raycasthit hit;

		if (physics.spherecast(capsuleraycaststart, sphereradius, vector3.down, out hit, ch.acs.groundcheckingdistance, ch.groundlayer))
		{
			ch.distancetoground = hit.distance - sphereradius;
		}
		else
		{
			ch.distancetoground = ch.acs.groundcheckingdistance;
		}


	}
	public void setgrounding()
	{
		bool groundedbydistance = ch.distancetoground < ch.acs.isgroundeddistance;

		if (groundedbydistance != ch.isgrounded)
		{
			if (time.time - ch.lastgroundedchecktime >= ch.acs.groundedswitchcooldown)
			{
				ch.isgrounded = groundedbydistance;
				ch.lastgroundedchecktime = time.time;

				//reset jumps on grounded
				if (ch.isgrounded)
				{
					ch.timesincelastgrounding = time.time;

					ch.ongrounding = true;

					ch.scheduleaction((int)ongroundingholdframes, () => ch.ongrounding = false);
				}
				else
				{
					ch.onungrounding = true;

					ch.scheduleaction((int)onungroundingholdframes, () => ch.onungrounding = false);
				}
			}
		}
	}
	#endregion grounding
	//=//-----|rotation|------------------------------------------------//=//
	#region rotation
	public void handlenaturalrotation()
	{
		if (ch.isgrounded)
		{
			//ch.facingright = ch.velocityx > 0;
		}


		if (ch.inputmovedirection != vector3.zero)
		{
			ch.facingright = ch.inputmovedirection.x > 0;
		}

		bool clockwiserotation = ch.flipcoin();

		vector3 directionfacing = ch.facingright ? vector3.right : vector3.left;

		// calculate the target rotation
		quaternion targetrotation = quaternion.lookrotation(directionfacing, vector3.up);

		// smoothly interpolate the rotation using slerp
		ch.rigandmeshtransform.rotation = quaternion.slerp(
			ch.rigandmeshtransform.rotation,
			targetrotation,
			time.deltatime * ch.acs.rotationspeed
		);
	}


	#endregion rotation
	//=//-----|routes|--------------------------------------------------//=//
	#region routes
	protected virtual void handlegrounding()
	{
		if (ch.ongrounding)
		{
			ch.jumpcount = 0;
			if (!ch.isgroundedbystate)
			{
				statepushstate(cstateid.groundedidle, (int)priority + 1, 2);
			}
		}
		if (ch.onungrounding)
		{
			if (ch.isgroundedbystate)
			{
				statepushstate(cstateid.idleairborne, (int)priority + 1, 2);
			}
		}
	}
	protected virtual void handlejump()
	{
		//assess
		bool jumpallowed = true;
		if (ch.jumpcount > ch.acs.maxjumps)
		{
			jumpallowed = false;
		}
		if (ch.inputenabled == false)
		{
			jumpallowed = false;
		}
		//route 
		if (cih.getbuttondown("jump") && jumpallowed)
		{
			ch.jumpcount++;
			statepushstate(cstateid.jump, 4, 4);
		}
	}
	#endregion routes
	//=//----------------------------------------------------------------//=//
	#endregion level_2
	/////////////////////////////////////////////////////////////////////////////
}
