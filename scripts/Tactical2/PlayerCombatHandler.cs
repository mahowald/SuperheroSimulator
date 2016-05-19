using UnityEngine;
using System.Collections;

public class PlayerCombatHandler : CombatHandler 
{
	// The combat handler takes commands from the locomotor
	// and performs combat-related actions:
	// - animates attacks
	// - fires projectiles
	// - animates damage

	// makes sure we're in the right position for the attack.

	public Animator animator;
	private HashIDs hash;
	private GameController gameController;
	private Transform playerTransform;
	private Camera mainCamera;

	private AnimatorStateInfo stateInfo;

	public GameObject projectileTest;





	void Start()
	{
		// animator = this.GetComponent<CharacterAppearanceHandler>().GetAnimator(); set by appearance handler
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		playerTransform = gameController.GetHeroTransform();
		hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>(); 
		mainCamera = gameController.mainCamera;
	}

	// Update is called once per frame
	void Update () 
	{
	}

	// This is called by the locomotor
	override public void DoBasicAttack()
	{
		animator.SetTrigger (hash.tBasicAttack);



		// Find nearby enemy:
		AIEnemy enemy = this.FindNearestEnemy();
		if(enemy != null)
		{
			if(!animator.IsInTransition(0))
			{
				Vector3 targetDir = enemy.transform.position - playerTransform.position;
				Vector3 qtargetDir = targetDir - new Vector3(0, targetDir.y, 0);
				Quaternion rotation = Quaternion.FromToRotation(playerTransform.forward, qtargetDir);
				animator.MatchTarget(enemy.transform.position - Constants.AttackMeleeDist*targetDir.normalized, rotation*playerTransform.rotation, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 1, 1), 1f), 0.0f, 1f);
			}
		}
		else
		{
			if(!animator.IsInTransition(0))
			{
				Vector3 targetHeading = mainCamera.transform.forward - new Vector3(0, mainCamera.transform.forward.y, 0);
				Quaternion rotation = Quaternion.FromToRotation(playerTransform.forward, targetHeading);
				animator.MatchTarget(playerTransform.position, rotation*playerTransform.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1f),0.0f, 1f);
			}
		}



	}

	override public void DoHit(Projectile projectile) // get hit by the given projectile
	{
		Debug.Log ("Got hit!");
	}

	override public void DoKO()
	{
	}


	override public void FireProjectile()
	{
		// GameObject proj = (GameObject) Instantiate(projectileTest, playerTransform.position + playerTransform.forward + Vector3.up, playerTransform.rotation);
		// proj.GetComponent<Projectile>().creator = this.GetComponentInParent<CombatHandler>();
	}

	private AIEnemy FindNearestEnemy()
	{
		AIEnemy closestEnemy = null;
		float attackDist = this.GetAttackDist();
		float closestDist = attackDist + 5;

		// New way
		float toBeat = 0f;
		foreach(AIEnemy enemy in gameController.enemies)
		{
			// rank the enemy
			Vector3 d = enemy.transform.position - playerTransform.position;
			Vector3 dir = new Vector3(mainCamera.transform.forward.x, 0f, mainCamera.transform.forward.z).normalized;
			float dot = Vector3.Dot(dir, d.normalized);
			float score = -1f; 

			if(d.magnitude < attackDist)
			{
				score = dot;
			}

			if(score > toBeat)
			{
				toBeat = score;
				closestEnemy = enemy;
			}
		}

		/** old way
		foreach(AIEnemy enemy in gameController.enemies)
		{
			float dist = Vector3.Distance(playerTransform.position, enemy.transform.position);

			if(dist > attackDist)
				continue;

			if(dist > closestDist)
				continue;
			else
				closestDist = dist;
			
			Vector3 cameraDirection = enemy.transform.position - mainCamera.transform.position;
			Vector3 playerDirection = enemy.transform.position - playerTransform.position;

			float camangle = Vector3.Angle(cameraDirection, mainCamera.transform.forward);
			float playerangle = Vector3.Angle(playerDirection, playerTransform.forward);

			if(camangle <= Constants.AttackAngle)
			{
				closestEnemy = enemy;
			}

			else if(playerangle <= Constants.AttackAngle)
			{
				closestEnemy = enemy;
			}
			else
			{
				continue;
			}

		}

		**/

		return closestEnemy;
	}

	private float GetAttackDist()
	{
		return Constants.AttackDist;
	}
}
