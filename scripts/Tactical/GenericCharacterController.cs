using UnityEngine;
using System.Collections;

public enum CharacterState { Idle, Combat, Moving, Falling, Attacking, Blocking, Damaged, SpecialAbility, Dead };

public class GenericCharacterController : MonoBehaviour 

{

	public Animator animator;
	protected Transform characterTransform; 
	protected Rigidbody rigidBody; 
	protected Collider characterCollider;

	public CharacterState characterState = CharacterState.Idle;
	
	public Vector3 heading; // What heading do we want to have?
	public bool lockHeading = false; 

	public float speed; // strength of movement input
	public float direction; // between -2 and 2. approximate angle between desired heading and transform.forward. (-2 = -180, 2 = 180). 
	public float moveFwd; // between -1 and 1. dot product of desired heading with transform.forward
	public float moveRight; // between -1 and 1. dot product of desired heading with transform.right

        public GroundedHelper groundedHelper;
	
	protected bool airborne; // are our feet touching the ground?
	protected float airtime = 0f; // how long have we been falling?

	protected HashIDs hash;
	protected Quaternion targetRotation;
	protected AnimatorStateInfo currentStateInfo;
	private bool limitSlope;

	protected int myLayer;

	// Start is called next. 
	virtual public void Start () 
		/*
		 * We start out completely stopped and idle.
		 * 
		 */
	{

		speed = 0f;
		direction = 0f;
		moveFwd = 0f;
		moveRight = 0f;
		// targetHeading = transform.forward;
		limitSlope = false;
		airborne = false;
		characterTransform = animator.transform;
		heading = characterTransform.forward;

		myLayer = LayerMask.NameToLayer("Player"); // TODO: Update this for NPCs. 
	}

	// Awake is called first.
	virtual public void Awake()
	{
		// Set up the HashIDs thing so we don't have to do string comparisons. It's a little faster?
		hash = GameObject.FindGameObjectWithTag ("GameController").GetComponent<HashIDs>(); 
		
		currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		characterTransform = animator.GetComponent<Transform>();
		rigidBody = animator.GetComponent<Rigidbody>();
		characterCollider = animator.GetComponent<Collider>();
	}

	
	/********************************
	 * 								*
	 * 		The Update Loop! 		*
	 * 								*
	 ********************************/
	
	virtual public void Update () 
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

		switch(characterState)
		{
		case CharacterState.Falling:
			HandleFalling();
			break;
		case CharacterState.Combat:
		case CharacterState.Moving:
		case CharacterState.Idle:
			// If we're not doing anything special, then update the animator speed
			// so that it matches the strength of the movement input.
			HandleTargetSpeed ();
			// This function turns us in the correct direction, or, if we're doing a 180,
			// determines when to do that.
			HandleTargetDirection();
			break;
		default:
			break;
		}

		// Play an animation (pushing off the wall) when we run into a wall.
		animator.SetBool (hash.bWallCollide, DetectFrontCollision());
	}

	virtual protected void HandleFalling()
	{
		animator.SetFloat(hash.fVerticalSpeed, rigidBody.velocity.y/5f);
		airtime += Time.deltaTime;
		animator.SetFloat(hash.fAirtime, airtime);

		Vector3 translationDir = moveFwd*characterTransform.forward + moveRight*characterTransform.right;

		rigidBody.AddForce(3f*translationDir);
	}


    float ypos = 0f;
    float VerticalVelocity()
    {
        float yout = characterTransform.position.y;

        float v = (yout - ypos)/Time.deltaTime;

        ypos = yout;
        
        return v;
    }

    float v0 = 0f;
    float VerticalAcceleration()
    {
        float v1 = VerticalVelocity();

        float a = (v1 - v0) / Time.deltaTime;

        v0 = v1;

        return a;
    }

	void FixedUpdate()
		/** anything directly affecting the physics engine (i.e. rigidbody)
		 * needs to be done here! **/
	{
		// Here we determine whether we're running up something super steep or not.
		// Since we do raycasts, and we're deciding whether or not to switch off root motion
		// and let the rigidbody stuff handle motion, we do it in the physics loop.
		// TODO: Fix this!
		HandleInclination ();

        // Update the animator based on whether or not we're airborne.
        UpdateAirborne();

        if (limitSlope || airborne)
		{
			animator.applyRootMotion = false;
		}
		else
		{
			animator.applyRootMotion = true;
		}
	}
	
	// Update the speed based on the InputHandler.
	void HandleTargetSpeed()
	{		
		animator.SetFloat (hash.fSpeed, speed);
		if(speed > 0f)
			characterState = CharacterState.Moving;
		else
			characterState = CharacterState.Idle;
	}
	
	// Get the normalized time from the animator's current state info, and then feed it back
	// to the animator. 
	void SetNormalizedTime()
	{
		animator.SetFloat (hash.fNormalizedTime, currentStateInfo.normalizedTime);
	}

	private float smoothDirV = 0f;
	// Sets up our movement handling.
	void HandleTargetDirection()
	{
		animator.SetFloat (hash.fDirection, direction);
		animator.SetFloat (hash.fMoveFwd, moveFwd);
		animator.SetFloat (hash.fMoveRight, moveRight);

		RotateToTargetHeading();
	}
	
	// This is the tricky one.
	// Does 180s, makes sure the player can rotate to target, etc.

	void RotateToTargetHeading()
	{
		if(moveFwd == 0 && moveRight == 0) // if we have no target heading, do nothing.
		{
			return;
		}

		Vector3 targetDirection = new Vector3();

		if(lockHeading)
		{
			targetDirection = heading - new Vector3(0, heading.y, 0);
		}
		else
		{
			targetDirection = moveFwd*characterTransform.forward + moveRight*characterTransform.right;
		}

		if(Pivoting ()) // if we're in a pivoting state (e.g., turning while idle, or doing a quick 180 while running)
		{
			if(animator.IsInTransition(0)) // if we're already animating, ignore
			{
				return;
			}
			if(currentStateInfo.normalizedTime == 0.0f) // if it's the start of our pivot clip, we set up the correct angle to end at:
			{
				targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
			}
			animator.MatchTarget (animator.rootPosition, targetRotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1f), 0.1f, 1f); // at the end of the clip, this is where we should end up
			return;
		}
		else
		{
			// ...
		}

		if(currentStateInfo.fullPathHash == hash.sCollideWithWall) // can't turn when colliding with a wall
		{
			return;
		}
		
		if(Sliding ()) // or while sliding
		{
			return;
		}
		if(lockHeading)
		{
			characterTransform.LookAt(characterTransform.position + targetDirection);
		}
		else
		{
			characterTransform.Rotate (Vector3.up, Constants.HeroRunTurnSpeed*moveRight*Time.deltaTime); // otherwise we turn at the hero turning speed
		}
		
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
		Vector3 fwdDir = characterTransform.forward; // where we are facing.
		Vector3 downDir = -1f*characterTransform.up;
		Vector3 heightOffset = Vector3.up*0.1f;
		float angle = 0f;
		
		//		bool collide = false;
		
		if(currentStateInfo.fullPathHash == hash.sStandardJump) // don't bother when we're jumping.
		{
			return;
		}
		if(Physics.Raycast(characterTransform.position + heightOffset, downDir, out hit, 2.5f, layerMask)) // if there's something in front of us
		{
			if(!hit.rigidbody)
			{
				angle = Vector3.Angle (Vector3.up, hit.normal);
			}
		}
		if(Physics.Raycast (characterTransform.position + heightOffset, fwdDir, out hit, 0.15f, layerMask))
		{
			if(!hit.rigidbody)
			{
				angle = Vector3.Angle (Vector3.up, hit.normal);
			}
		}
		
		float scaledAngle = -1f*angle*Vector3.Dot (characterTransform.forward, new Vector3(hit.normal.x, 0f, hit.normal.z).normalized);
		animator.SetFloat (hash.fInclination, scaledAngle);
		
		/**
		if(angle > Constants.SlopeLimit)
		{
			Vector3 normaldir = new Vector3(0f, -10f, 0f);
			characterTransform.GetComponent<Rigidbody>().AddForce (normaldir, ForceMode.VelocityChange);
			limitSlope = true;
		}
		**/
	}

    
	// Checks whether or not we're in the air.
	void UpdateAirborne()
	{
		bool oldAirborne = airborne;

        airborne = !IsGrounded();

		animator.SetBool(hash.bAirborne, airborne);

		if(airborne == true)
		{
			characterState = CharacterState.Falling;
			animator.applyRootMotion = false;
			if(oldAirborne == false)
			{
				airtime = 0.0f;
			}
		}

		// Landing
		if(airborne == false)
		{
            // if (oldAirborne == true && airtime > 0.1f) // doing a landing
			// {
				// animator.SetTrigger(hash.tDoLanding);
				characterState = CharacterState.Idle;
                animator.applyRootMotion = true;
                airtime = 0.0f;
			// }

		}


	}



    
    protected bool IsGrounded()
    {
        return groundedHelper.grounded;

        /** 
        float accel = VerticalAcceleration();

        float velocity = VerticalVelocity();

        if(Mathf.Abs(accel) >= 8f)
        {
            return false;
        }

        else
        {
            return true;
        }
        **/

        /** 
        if(jumpHelper != null)
        {
            return jumpHelper.Grounded;
        }

        RaycastHit hit;
        Vector3 downDir = -1f * characterTransform.up;

        if (Physics.Raycast(characterCollider.bounds.center, downDir, out hit, characterCollider.bounds.extents.y + 0.15f))
        {
            if (!hit.transform.IsChildOf(this.transform)) // TODO: Update this for NPCs. 
            {
                return true;
            }
        }


        return false;

         **/ 
    }
	
	// Did we hit something in front of us?
	bool DetectFrontCollision()
	{
		int layerMask = 1 << Constants.PlayerLayer;
		
		layerMask = ~layerMask;
		
		RaycastHit hit;
		
		Vector3 heightOffset = 1.3f*characterTransform.up;
		if(Physics.Raycast (characterTransform.position + heightOffset, characterTransform.forward, out hit, 0.65f, layerMask))
		{
			if(!hit.rigidbody)
			{
				Vector3 newnormal = new Vector3(hit.normal.x, 0f, hit.normal.z).normalized;
				if(Vector3.Dot (newnormal, characterTransform.forward) < -0.9f)
				{
					return true;
				}
			}
		}
		
		return false;
	}

}
