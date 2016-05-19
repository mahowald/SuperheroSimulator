using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIEnemy : AIAgent {

	public enum AIStance {
		Neutral, // just patrol or stand idle
		Alert, // if we see the player, seek her out and engage
		Hostile, // we are actively trying to find the player
	};
	
	public enum AIGoal {
		Idle, // hold position
		PatrolRoute, // follow our existing route
		AttackTarget, // close to target and attack.
		HoldPostAttackPosition, // identical behavior to HoldRelativePosition, but should never show up randomly! Only after executing an attack. 
		Block, // block incoming attacks
		MoveToTarget, // move to a given distance from the target
		MoveAwayFromTarget, // increase distance from target
		HoldRelativePosition, // maintain the current distance from the target. 
		Strafe, // Move left or right relative to target
		FleeTarget, // flee to a safe location
	};
	
	protected CombatCharacterController combatCharacterController;
	protected CombatCharacterController enemyCombatController;

	public AIStance stance;

	protected AIGoal currentGoal;

	public AIGoal CurrentGoal
	{
		get
		{
			return currentGoal;
		}
	}

	private List<AIEnemy> nearbyAllies;

	protected Dictionary<AIGoal, int> goalWeights;

	protected Vector3 targetOffset = Vector3.zero; 

	public Transform enemy;

	protected override void Start ()
	{
		base.Start ();
		
		combatCharacterController = this.gameObject.GetComponent<CombatCharacterController>();

		SetupGoalWeights();

		// TODO: Pick enemies dynamically...
		combatCharacterController.ActiveTarget = enemy;

		enemyCombatController = enemy.GetComponentInParent<CombatCharacterController>();

		nearbyAllies = new List<AIEnemy>();


		// Start the heartbeat. (Do this last!)
		StartCoroutine(Heartbeat());
	}



	protected override void FixedUpdate ()
	{

		if(enemy != null)
		{
			combatCharacterController.lockHeading = true;
			combatCharacterController.heading = enemy.position - this.transform.position;
		}


		if(currentGoal == AIGoal.AttackTarget)
		{
			Mode = AIMode.GotoLocation;
			target = enemy.position;
			if(EnemyInRange() && enemyCombatController.characterState != CharacterState.Attacking) // we're polite and don't attack if the player is already busy...
			{
				combatCharacterController.DoBasicAttack();
				currentGoal = AIGoal.HoldPostAttackPosition;
				targetOffset = -2f*this.transform.forward;
			}
		}

		if(currentGoal == AIGoal.MoveToTarget && Vector3.Distance(this.transform.position, target) < 0.5f)
		{
			targetOffset = this.transform.position - enemy.position;
			Mode = AIMode.GotoLocation;
		}

		if(currentGoal == AIGoal.HoldRelativePosition || currentGoal == AIGoal.HoldPostAttackPosition)
		{
			Mode = AIMode.GotoLocation;
			target = enemy.position + targetOffset; 
		}

		if(currentGoal == AIGoal.Strafe)
		{
		}
		
		base.FixedUpdate();
	}

	IEnumerator Heartbeat()
	{
		// Each 'heartbeat' we evaluate our position and update our goals accordingly. 

		while(true)
		{

			// Update speed:

			if(enemy != null)
			{
				float dist = Vector3.Distance(this.transform.position, enemy.position);
				if(dist <= 4f)
				{
					speed = 0.25f;
				}
				else
				{
					speed = 1f;
				}
			}

			// Update nearby allies:
			FindNearbyAllies();


			// Choose a Goal:
			UpdateGoalWeights();
			currentGoal = WeightedRandomChooser.ChooseRandom(goalWeights, GlobalData.rand);


			// Debug.Log ("Time: " + Time.time + ", chose goal " + currentGoal.ToString());


			if(currentGoal != AIGoal.Block)
				combatCharacterController.block = false;

			switch(currentGoal)
			{
			case AIGoal.MoveToTarget:
				Vector3 dir1 = (this.transform.position - enemy.position).normalized;
				target = enemy.position + 2f*dir1;
				yield return new WaitForSeconds(2f);
				break;

			case AIGoal.HoldRelativePosition:
				targetOffset = this.transform.position - enemy.position;
				yield return new WaitForSeconds(6f);
				break;

			case AIGoal.Strafe:
				int lor = GlobalData.rand.Next(0, 2);
				if(lor == 0)
				{
					target = this.transform.position + 4f*this.transform.right;
				}
				if(lor == 1)
				{
					target = this.transform.position - 4f*this.transform.right;
				}
				Mode = AIMode.GotoLocation;
				yield return new WaitForSeconds(4f);
				break;

			case AIGoal.MoveAwayFromTarget:
				Vector3 dir2 = (this.transform.position - enemy.position).normalized;
				target = this.transform.position + 2f*dir2;
				yield return new WaitForSeconds(3f);
				break;

			case AIGoal.AttackTarget:
				target = enemy.position;
				combatCharacterController.ActiveTarget = enemy;
				yield return new WaitForSeconds(6f);
				break;

			case AIGoal.Block:
				target = this.transform.position;
				combatCharacterController.block = true;
				// TODO: Look at target...
				/** 
				Vector3 head = enemy.position - this.transform.position;
				head = (head - new Vector3(0, head.y, 0)).normalized;
				Quaternion rot = Quaternion.FromToRotation(this.transform.forward, head)*this.transform.rotation;
				combatCharacterController.animator.MatchTarget(this.transform.position, rot, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1f), 0f, 1f);
				**/
				yield return new WaitForSeconds(3f);
				break;

			case AIGoal.PatrolRoute:
				Mode = AIMode.FollowRoute;
				target = route[waypoint].position;
				yield return new WaitForSeconds(3f);
				break;

			default:
				Mode = AIMode.Idle;
				target = this.transform.position;
				yield return new WaitForSeconds(5f);
				break;
			}
		}
	}

	bool EnemyInRange()
	{
		if(Vector3.Magnitude(enemy.position - this.transform.position) < 2f)
		{
			return true;
		}

		return false;
	}

	bool LookingAtEnemy()
	{
		Vector3 enemyDir = enemy.position - this.transform.position;

		return (Vector3.Dot (enemyDir, this.transform.forward) >= 0);
	}

	int GetAttackingAllies()
	{
		int attackingAllies = 0;
		foreach(AIEnemy ally in nearbyAllies)
		{
			if((ally.currentGoal == AIGoal.AttackTarget || ally.currentGoal == AIGoal.HoldPostAttackPosition) 
			   && ally.enemy == this.enemy && ally != this)
			{
				attackingAllies += 1;
			}
		}

		return attackingAllies;
	}

	void UpdateGoalWeights()
	{
		// Set all the goal weights to zero. 
		ZeroGoalWeights();

		// Here we assign weights based on analysis of our current situation. 
		if(stance == AIStance.Neutral)
		{
			if(Mode == AIMode.FollowRoute)
			{
				goalWeights[AIGoal.PatrolRoute] = 10;
			}
			else
			{
				goalWeights[AIGoal.Idle]  = 10; 
			}

			return;
		}

		if(stance == AIStance.Alert)
		{
			goalWeights[AIGoal.MoveToTarget] = 10;
			return;
		}

		if(stance == AIStance.Hostile)
		{
			float distToEnemy = Vector3.Distance(enemy.position, this.transform.position);

			if(distToEnemy >= 4f)
			{
				goalWeights[AIGoal.MoveToTarget] = (int)(distToEnemy*distToEnemy);
			}

			if(distToEnemy <= 1f)
			{
				goalWeights[AIGoal.MoveAwayFromTarget] = 50;
			}

			if(distToEnemy > 2f && distToEnemy < 3f)
			{
				goalWeights[AIGoal.HoldRelativePosition] = 5;
				goalWeights[AIGoal.Strafe] = 30;
			}

			if(distToEnemy <= 5f)
			{
				if(LookingAtEnemy())
				{
					if(IsEnemyLookingAtMe())
					{
						goalWeights[AIGoal.Block] = 20;
					}
					else
					{
						goalWeights[AIGoal.Block] = 10;
					}
				}

				int attackingAllies = this.GetAttackingAllies();
				if(attackingAllies == 0)
				{
					goalWeights[AIGoal.AttackTarget] = 25;
				}
				if(attackingAllies == 1)
				{
					goalWeights[AIGoal.AttackTarget] = 10;
				}

			}

			goalWeights[AIGoal.FleeTarget] = 0;


			return;
		}

		// So that we at least pick something. 
		goalWeights[AIGoal.Idle] = 10;

		return;

	}

	void SetupGoalWeights()
	{
		goalWeights = new Dictionary<AIGoal, int>();
		goalWeights.Add(AIGoal.Idle, 1);
		goalWeights.Add(AIGoal.PatrolRoute, 0);
		goalWeights.Add(AIGoal.AttackTarget, 0);
		goalWeights.Add(AIGoal.Block, 0);
		goalWeights.Add(AIGoal.MoveToTarget, 0);
		goalWeights.Add(AIGoal.MoveAwayFromTarget, 0);
		goalWeights.Add(AIGoal.HoldRelativePosition, 0);
		goalWeights.Add(AIGoal.Strafe, 0);
		goalWeights.Add(AIGoal.FleeTarget, 0);
	}

	void ZeroGoalWeights()
	{
		goalWeights[AIGoal.Idle] =  0;
		goalWeights[AIGoal.PatrolRoute] = 0;
		goalWeights[AIGoal.AttackTarget] = 0;
		goalWeights[AIGoal.Block] = 0;
		goalWeights[AIGoal.MoveToTarget] = 0;
		goalWeights[AIGoal.MoveAwayFromTarget] = 0;
		goalWeights[AIGoal.FleeTarget] = 0;
		goalWeights[AIGoal.HoldRelativePosition] = 0;
		goalWeights[AIGoal.Strafe] = 0;

	}

	void FindNearbyAllies()
	{
		nearbyAllies.Clear();

		GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");

		foreach(GameObject go in gos)
		{
			AIEnemy ally = go.GetComponent<AIEnemy>();

			if(ally != null)
			{
				if(Vector3.Distance(ally.transform.position, this.transform.position) < 10f)
				{
					nearbyAllies.Add(ally);
				}
			}
		}

		return;
	}

	bool IsEnemyLookingAtMe()
	{
		Vector3 dir = (this.transform.position - enemy.position).normalized;
		if(Vector3.Dot (enemy.forward, dir) >= 0.7)
		{
			return true;
		}
		return false;
	}


	/** OLD WAY
	 * 
	public AIEnemyStance stance;
	public Transform enemy;
	public float attackRange;

	private Vector3 lastKnownLocation;

	private float waitTimer; // how long do we wait at a destination?
	private float heartbeatTimer; // execute combat actions on every beat

	protected CombatCharacterController combatCharacterController;

	protected override void Start ()
	{
		base.Start ();

		combatCharacterController = this.gameObject.GetComponent<CombatCharacterController>();

		if(!enemy)
		{
			enemy = gameController.GetHeroTransform(); // by default, we are hostile to the hero.
		}
		waitTimer = 0f;
	}

	protected override void FixedUpdate()
	{
		switch(stance)
		{
		case AIEnemyStance.Neutral:
			break;
		case AIEnemyStance.Alert:
			if(IsEnemyInSight())
			{
				waitTimer = 0f;
				stance = AIEnemyStance.Hostile;
				lastKnownLocation = player.position;
			}
			else
			{
				waitTimer += Time.deltaTime;
				if(waitTimer > Constants.AIWaitTime)
				{
					Mode = startingMode; // revert to whatever we were doing.
				}
			}
			break;
		case AIEnemyStance.Hostile:
			
			Mode = AIMode.GotoLocation;
			lastKnownLocation = player.position;
			if(IsEnemyTooClose())
			{
				stance = AIEnemyStance.FallBack;
				target = (attackRange + 1f)*((this.transform.position - player.position).normalized) + this.transform.position;
				break;
			}
			else 
			{
				if(IsEnemyInSight())
				{
					lastKnownLocation = player.position;
					if(IsEnemyInRange())
					{
						transform.LookAt(lastKnownLocation);
						target = lastKnownLocation;

						if(heartbeatTimer == 0f)
						{
							combatCharacterController.DoBasicAttack();
						}

					}
					else
					{
						target = lastKnownLocation;
					}

				}
				else
				{
					stance = AIEnemyStance.Alert;
					// animator.SetBool ("Fire", false);
					target = lastKnownLocation;
				}
			}
			break;
		case AIEnemyStance.FallBack:
			Mode = AIMode.GotoLocation;
			if(!IsEnemyTooClose())
			{
				if(this.ArrivedAtDestination())
				{
					transform.LookAt(lastKnownLocation);
					stance = AIEnemyStance.Alert;
				}
			}
			else
			{
				target = (attackRange)*((this.transform.position - player.position).normalized) + this.transform.position;
			}
			break;


		}
		Heartbeat();

		base.FixedUpdate(); // AIAgent should handle all of the actual pathfinding stuff. 
	}

	bool IsEnemyInSight() // Do we know where our target is located? 
	{
		float dist = (this.transform.position - enemy.position).magnitude;

		if(dist < Constants.AIHearDistance)
			return true;

		if(dist > Constants.AIViewDistance)
			return false;

		float angle = Vector3.Angle(enemy.position - this.transform.position, this.transform.forward);

		if(angle > Constants.AIViewAngle)
			return false;
		RaycastHit hit;

		int layerMask = ~LayerMask.GetMask ("Player", "Enemies");

		if(Physics.Raycast(this.transform.position + 1.9f*Vector3.up, enemy.position - this.transform.position, out hit, dist, layerMask))
			return false;

		return true;
	}

	private void Heartbeat()
	{
		heartbeatTimer += Time.deltaTime;

		if(heartbeatTimer >= Constants.AIHeartbeat)
		{
			heartbeatTimer = 0f;
		}

	}

	public bool IsEnemyInRange()
	{
		return Vector3.Magnitude(this.transform.position - enemy.position) < attackRange;
	}

	bool IsEnemyTooClose()
	{
		return false; 
		// return Vector3.Magnitude(this.transform.position - enemy.position) < (attackRange - 3f);
	}

**/

}
