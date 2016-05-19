using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum AIMode {
	Idle,
	GotoLocation,
	FollowPlayer,
	FollowRoute,
	FleePlayer,
	FleeLocation
};

public class AIAgent : MonoBehaviour {

	public NavMeshAgent navAgent;

	public GameController gameController;

	public AIMode startingMode;

	private AIMode mode; 

	public Vector3 target; // This is where we are trying to get to! (a location in 3-space)

	protected Animator animator;

	protected GenericCharacterController characterController;

	public List<Transform> route;

	protected int waypoint; // The waypoint we're at in the route.
	private bool loopRoute; // Do we loop around the route?

	public float waypointWaitTime = 0f; // How long do we stop before going to the next waypoint? 
	private float waypointTimer = 0f;

	public float speed = 1f;

	protected Transform player;

	void Awake () {
		waypoint = 0;
		loopRoute = true;
	}
	
	// Use this for initialization
	protected virtual void Start()
	{
		Mode = startingMode;
		animator = this.GetComponent<Animator>();
		player = gameController.GetHeroTransform();

		characterController = this.GetComponent<GenericCharacterController>();

		switch(Mode)
		{
		case AIMode.FollowPlayer:
			target = player.position;
			break;
		case AIMode.FollowRoute:
			target = route[0].position;
			break;
		default:
			target = this.transform.position;
			break;
		}

		navAgent.SetDestination(target);
	}

	protected virtual void Update()
	{
	}

	protected virtual void FixedUpdate () 
	{
		switch(Mode)
		{
		case AIMode.FleePlayer:
			if((navAgent.destination - this.transform.position).magnitude < navAgent.stoppingDistance)
			{
				if((player.position - this.transform.position).magnitude < Constants.FleeDistance)
				{
					target = CalculateFleeToSpot();
				}
			}
			else
			{
			}
			break;
		case AIMode.FollowPlayer:
			target = player.position; // update the new location each time.
			break;
		case AIMode.FollowRoute:
			if(ArrivedAtDestination())
			{
				waypointTimer += Time.deltaTime;
				if(waypointTimer >= waypointWaitTime)
				{
					target = GetNextWaypoint();
					waypointTimer = 0f;
				}
			}
			break;
		case AIMode.GotoLocation:
			// We just go straight to the target. 
			break;
		case AIMode.Idle:
			// shouldn't do anything
			break;
		}

		navAgent.SetDestination(target);

		// UpdateAnimator();

		UpdateController();
		// DoRotation();
	}

	private float dampedMoveFwd = 0f;
	private float dampedMoveRight = 0f;
	private float dampedMoveFwdV = 0f;
	private float dampedMoveRightV = 0f;

	// AI controls just like the player now.
	void UpdateController()
	{

		Vector3 targetMovement = navAgent.desiredVelocity/navAgent.speed;
		// float moveStrength = navAgent.speed/Constants.AIRunSpeed;

		float targetMoveFwd = speed*Vector3.Dot (targetMovement, this.transform.forward);
		float targetMoveRight = speed*Vector3.Dot (targetMovement, this.transform.right);
		// damping

		dampedMoveFwd = Mathf.SmoothDamp(dampedMoveFwd, targetMoveFwd, ref dampedMoveFwdV, 0.3f);
		dampedMoveRight = Mathf.SmoothDamp(dampedMoveRight, targetMoveRight, ref dampedMoveRightV, 0.3f);


		Vector3 targetHeading = targetMovement;

		if(Vector3.Magnitude(targetMovement) <= 0.1f)
			targetHeading = Vector3.zero;

		// characterController.targetHeading = targetHeading;

		characterController.speed = Mathf.Clamp (Mathf.Sqrt (dampedMoveFwd*dampedMoveFwd + dampedMoveRight*dampedMoveRight), 0, 1f);

		// characterController.direction = moveRight;
		characterController.direction = 0f;

		characterController.moveFwd = dampedMoveFwd;
		characterController.moveRight = dampedMoveRight;
	}

	// Set the navAgent velocity correctly... 
	void OnAnimatorMove()
	{
		navAgent.velocity = animator.deltaPosition/Time.deltaTime;
	}

	// deprecated
	void DoRotation()
	{
		Vector3 desiredV = navAgent.desiredVelocity;
		float rotate = Vector3.Dot (desiredV, this.transform.right);

		this.transform.Rotate (Vector3.up, Constants.NPCTurnSpeed*rotate*Time.deltaTime);
	}

	protected bool ArrivedAtDestination()
	{
		if(Vector3.Magnitude(this.transform.position - target) <= navAgent.stoppingDistance)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	Vector3 GetNextWaypoint()
	{
		if((waypoint == route.Count - 1) && loopRoute == false)
		{
			return route[waypoint].position;
		}

		if(waypoint < route.Count - 1)
		{
			waypoint = waypoint + 1;
		}
		else
		{
			waypoint = 0;
		}

		return route[waypoint].position;
	}

	public AIMode Mode
	{
		get
		{
			return mode;
		}
		set
		{
			if(mode == value)
				return;

			mode = value;
			switch(mode)
			{
			case AIMode.FollowRoute:
				navAgent.speed = Constants.AIWalkSpeed;
				break;
			case AIMode.FleeLocation:
			case AIMode.FleePlayer:
			case AIMode.FollowPlayer:
			case AIMode.GotoLocation:
			default:
				navAgent.speed = Constants.AIRunSpeed;
				break;
			}
		}
	}

	Vector3 CalculateFleeToSpot()
	{
		Vector3 targetPos;

		switch(Mode)
		{
		case AIMode.FleePlayer:
			targetPos = player.position;
			break;
		default:
			targetPos = target;
			break;
		}

		Vector3 direction = (Constants.FleeDistance + 2.5f)*(this.transform.position - targetPos).normalized;

		Vector3 outPos;

		NavMeshHit hit;

		if(NavMesh.SamplePosition(this.transform.position + direction, out hit, 5f, ~0)) // ~0 should test for all layers
		{
			outPos = hit.position;
		}
		else
		{
			outPos = this.transform.position;
		}

		return outPos;



	}

}
