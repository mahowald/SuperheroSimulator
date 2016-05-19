using UnityEngine;
using System.Collections;

public class PlayerLocomotor : MonoBehaviour {

	public Animator animator;

	public Vector3 targetHeading; // What heading do we want to have?
	public float speed; // strength of movement input
	public float direction; // between -1 and 1. dot product of desired heading with transform.right
	public float moveFwd; // between -1 and 1. dot product of desired heading with transform.forward
	public float moveRight; // between -1 and 1. dot product of desired heading with transform.right
	public bool jump; // should we trigger a jump?
	public bool basicAttack; // did the player press the attack button?

	private bool airborne; // are our feet touching the ground?

//	private Vector3 currentHeading;
//	private Vector3 lastHeading;
//	private Vector3 lastPosition;

	private HashIDs hash;

	public Transform playerTransform;

	private Quaternion targetRotation;

	private AnimatorStateInfo currentStateInfo;

	private bool limitSlope;

	private PlayerCombatHandler combatHandler;

	private float attackTimer;
	private int attackNum;
	private bool attackWindow;

	// Use this for initialization
	void LateStart () 
		/*
		 * We start out completely stopped and idle.
		 * 
		 */
	{
		speed = 0f;
		direction = 0f;
		moveFwd = 0f;
		jump = false;
		targetHeading = transform.forward;
		limitSlope = false;
		airborne = false;
	}

	void Awake()
	{
		// Set up the HashIDs thing so we don't have to do string comparisons. It's a little faster?
		hash = GameObject.FindGameObjectWithTag ("GameController").GetComponent<HashIDs>(); 

		combatHandler = this.GetComponent<PlayerCombatHandler>();
		
		currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

		// lastPosition = playerTransform.position;
	}


	/********************************
	 * 								*
	 * 		The Update Loop! 		*
	 * 								*
	 ********************************/

	void Update () 
		/** anything that it is important the character immediately respond to
		 * e.g. attacking or jumps
		 * should go in the update loop. 
		 */
	{
		// Update the state of the animator.
		currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		// Grab the time we're at in the current animation from the above. 
		// (This is used for, among other things, transition handling in the Animator state machine).
		SetNormalizedTime ();
		// Check if we're doing a jump.
		HandleJump ();
		// Check if we're doing an attack.
		HandleBasicAttack();
		// If we're not doing anything special, then update the animator speed
		// so that it matches the strength of the movement input.
		HandleTargetSpeed ();
		// This function turns us in the correct direction, or, if we're doing a 180,
		// determines when to do that.
		HandleTargetDirection();

		// Update the animator based on whether or not we're airborne.
		animator.SetBool (hash.bAirborne, UpdateAirborne());
		// Play an animation (pushing off the wall) when we run into a wall.
		animator.SetBool (hash.bWallCollide, DetectFrontCollision());
		// The last position is the place we're currently at. (Not Used!)
		// lastPosition = playerTransform.position;

		// update the attack count
		if(attackWindow)
		{
			attackTimer += Time.deltaTime;
			if(attackTimer >= Constants.AttackTime)
			{
				attackNum = 0;
				attackTimer = 0f;
				attackWindow = false;
				animator.SetInteger(hash.iAttackCount, 0);
			}
		}
	}

	void FixedUpdate()
		/** anything directly affecting the physics engine (i.e. rigidbody)
		 * needs to be done here! **/
	{
		// Here we determine whether we're running up something super steep or not.
		// Since we do raycasts, and we're deciding whether or not to switch off root motion
		// and let the rigidbody stuff handle motion, we do it in the physics loop.
		HandleInclination ();

		if(limitSlope || airborne)
		{
			animator.applyRootMotion = false;
		}
		else
		{
			animator.applyRootMotion = true;
		}
		if(limitSlope)
		{
			LimitVerticalClimb(); // Stops the player from being able to run up everything.
		}
	}

	// Right now, this just sets the trigger based on input.
	// TODO: control jump height via player input (e.g. holding down the trigger, movement)
	void HandleJump()
	{
		if(jump)
		{
			animator.SetTrigger(hash.tJump);
		}
	}

	// Tells the combat handler to take over things.
	void HandleBasicAttack()
	{
		if(basicAttack)
		{
			combatHandler.DoBasicAttack();

			if(currentStateInfo.fullPathHash == hash.sBT_BasicAttack1 || currentStateInfo.fullPathHash == hash.sBT_BasicAttack2 || currentStateInfo.fullPathHash == hash.sBT_BasicAttack3)
			{
				// we are already attacking
			}
			else
			{
				attackWindow = true;
				attackTimer = 0f;
				animator.SetInteger(hash.iAttackCount, attackNum);
				attackNum++;
				attackNum = attackNum % Constants.AttackVariations;
			}
		}
	}

	// Update the speed based on the InputHandler.
	void HandleTargetSpeed()
	{		
		animator.SetFloat (hash.fSpeed, speed);
	}

	// Get the normalized time from the animator's current state info, and then feed it back
	// to the animator. 
	void SetNormalizedTime()
	{
		animator.SetFloat (hash.fNormalizedTime, currentStateInfo.normalizedTime);
	}

	// Sets up our movement handling.
	void HandleTargetDirection()
	{
		animator.SetFloat (hash.fDirection, direction);
		animator.SetFloat (hash.fMoveFwd, moveFwd);
		animator.SetFloat (hash.fMoveRight, moveRight);
//		currentHeading = this.transform.forward;
		RotateToTargetHeading();
		// lastHeading = currentHeading;
	}

	// This is the tricky one.
	// Does 180s, makes sure the player can rotate to target, etc.
	void RotateToTargetHeading()
	{
		if(targetHeading.magnitude == 0) // if we have no target heading, do nothing.
		{
			return;
		}

		if(Pivoting ()) // if we're in a pivoting state (e.g., turning while idle, or doing a quick 180 while running)
		{
			if(animator.IsInTransition(0)) // if we're already animating, ignore
			{
				return;
			}
			if(currentStateInfo.normalizedTime == 0.0f) // if it's the start of our pivot clip, we set up the correct angle to end at:
			{
				if(currentStateInfo.fullPathHash == hash.sIdle180 || currentStateInfo.fullPathHash == hash.sSlide180) // for 180s, we do 180 degrees
				{
					targetRotation = playerTransform.rotation*Quaternion.Euler (0, 180, 0);
				}
				if(currentStateInfo.fullPathHash == hash.sIdle90L)
				{
					targetRotation = playerTransform.rotation*Quaternion.Euler (0, -90, 0); // 90 to the left
				}
				if(currentStateInfo.fullPathHash == hash.sIdle90R)
				{
					targetRotation = playerTransform.rotation*Quaternion.Euler (0, 90, 0); // 90 to the right
				}
			}
			animator.MatchTarget (animator.rootPosition, targetRotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1f), 0.2f, 1f); // at the end of the clip, this is where we should end up
			return;
		}
		else
		{
			targetRotation = playerTransform.rotation;
		}

		if(currentStateInfo.fullPathHash == hash.sCollideWithWall) // can't turn when colliding with a wall
		{
			return;
		}

		if(Sliding ()) // or while sliding
		{
			return;
		}

		playerTransform.Rotate (Vector3.up, Constants.HeroRunTurnSpeed*direction*Time.deltaTime); // otherwise we turn at the hero turning speed

	}

	// This is used in rotation when we check to make sure we can't turn while we slide to a stop.
	bool Sliding()
	{
		int stateNameHash = currentStateInfo.fullPathHash; 
		if(stateNameHash == hash.sSlideToStop)
		{
		 	return true;
		}
		return false;
	}

	bool Pivoting()
		// Check if we're currently in a pivoting animation
	{
		int stateNameHash = currentStateInfo.fullPathHash;

		// if(stateNameHash == hash.sSlideToStop)
		// {
		// 	return true;
		// }
		if(stateNameHash == hash.sSlide180)
		{
			return true;
		}

		if(stateNameHash == hash.sIdle180)
		{
			return true;
		}
		
		if(stateNameHash == hash.sIdle90R)
		{
			return true;
		}
		
		if(stateNameHash == hash.sIdle90L)
		{
			return true;
		}
		return false;
	}

	// This has a lot of code but basically it just
	// stops the player from running up steep stuff,
	// and slows down movement when going uphill.
	void HandleInclination()
	{
		
		int layerMask = 1 << Constants.PlayerLayer;
		
		layerMask = ~layerMask;

		limitSlope = false;
		RaycastHit hit;
		Vector3 fwdDir = playerTransform.forward; // where we are facing.
		Vector3 downDir = -1f*playerTransform.up;
		Vector3 heightOffset = Vector3.up*0.1f;
		float angle = 0f;

//		bool collide = false;

		if(currentStateInfo.fullPathHash == hash.sStandardJump) // don't bother when we're jumping.
		{
			return;
		}
		if(Physics.Raycast(playerTransform.position + heightOffset, downDir, out hit, 2.5f, layerMask)) // if there's something in front of us
		{
			if(!hit.rigidbody)
			{
				angle = Vector3.Angle (Vector3.up, hit.normal);
			}
		}
		if(Physics.Raycast (playerTransform.position + heightOffset, fwdDir, out hit, 0.15f, layerMask))
		{
			if(!hit.rigidbody)
			{
				angle = Vector3.Angle (Vector3.up, hit.normal);
			}
		}

		float scaledAngle = -1f*angle*Vector3.Dot (playerTransform.forward, new Vector3(hit.normal.x, 0f, hit.normal.z).normalized);
		animator.SetFloat (hash.fInclination, scaledAngle);


		if(angle > Constants.SlopeLimit)
		{
			Vector3 normaldir = new Vector3(0f, -10f, 0f);
			playerTransform.GetComponent<Rigidbody>().AddForce (normaldir, ForceMode.VelocityChange);
			limitSlope = true;
		}
	}

	// Checks whether or not we're in the air.
	bool UpdateAirborne()
	{
		RaycastHit hit;
		Vector3 downDir = -1f*playerTransform.up;

		Vector3 heightOffset = 0.1f*playerTransform.up;

		if(currentStateInfo.fullPathHash == hash.sStandardJump && currentStateInfo.normalizedTime < 0.9f)
		{
			airborne = false;
			return airborne;
		}

		if(Physics.Raycast (playerTransform.position + heightOffset, downDir, out hit, 1.5f))
		{
			if(hit.transform.tag != "Player")
			{
				airborne = false;
			}
		}
		else
		{
			airborne = true;
		}

		return airborne;
	}

	// Did we hit something in front of us?
	bool DetectFrontCollision()
	{
		int layerMask = 1 << Constants.PlayerLayer;

		layerMask = ~layerMask;

		RaycastHit hit;

		Vector3 heightOffset = 1.3f*playerTransform.up;
		if(Physics.Raycast (playerTransform.position + heightOffset, playerTransform.forward, out hit, 0.65f, layerMask))
		{
			if(!hit.rigidbody)
			{
				Vector3 newnormal = new Vector3(hit.normal.x, 0f, hit.normal.z).normalized;
				if(Vector3.Dot (newnormal, playerTransform.forward) < -0.9f)
				{
					return true;
				}
			}
		}

		return false;
	}

	// Doesn't do anything at the moment.
	void LimitVerticalClimb()
	{
		// not sure what to do here!
	}



}
