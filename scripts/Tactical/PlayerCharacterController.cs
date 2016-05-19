using UnityEngine;
using System.Collections;

public class PlayerCharacterController : CombatCharacterController {

	/** These are used to give sequenced attack animations **/
	private float attackTimer;
	private int attackNum;
	private bool attackWindow;

	/** targeting **/
	public enum TargetMode {Free, Locked};
	public TargetMode targetMode = TargetMode.Free;

	/** Game Controller & Main Camera **/
	
	private GameController gameController;
	private Camera mainCamera;

	/** jumping **/

	private float jumpTimer = 0f;
	private bool chargingJump = false;


    public override void Awake()
    {
        base.Awake();

        // make the player a little tougher
        combatantData.health = 120f;
        combatantData.armor = 10f;
        startCombatantData = combatantData;
    }

    override public void Start()
	{
		base.Start();

		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		mainCamera = gameController.mainCamera;

		heading = characterTransform.forward;
	}

	override public void Update()
	{

		base.Update();

		// update the attack count
		if(attackWindow && characterState != CharacterState.Attacking)
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

		// update heading to target:
		if(targetMode == TargetMode.Locked)
		{
			heading = this.activeTarget.position - this.characterTransform.position;
		}

		// jumping

		if(chargingJump)
		{
			jumpTimer += Time.deltaTime;
		}

        Debug.Log(IsGrounded());

	}

	public void ClearActiveTarget()
	{
		targetMode = TargetMode.Free;
	}
	
	override public Transform ActiveTarget // For the player, the active target will be determined based on the target mode.  
	{
		get
		{
			if(targetMode == TargetMode.Locked)
			{
				return activeTarget;
			}
			else
			{
				return GetNearestEnemy();
			}
		}
		set
		{
			activeTarget = value;
			if(value != null)
			{
				targetMode = TargetMode.Locked;
			}
		}
	}

	override public void DoBasicAttack()
	{
		if(characterState == CharacterState.SpecialAbility)
			return;

		if(!Attacking() || currentStateInfo.normalizedTime >= 0.95f)
		{
			attackWindow = true;
			attackTimer = 0f;
			animator.SetInteger(hash.iAttackCount, attackNum);
			attackNum++;
			attackNum = attackNum % Constants.AttackVariations;
		}
		Debug.Log (ActiveTarget);
		base.DoBasicAttack();
	}

	override protected void HandleNullTarget()
	{
		if(!animator.IsInTransition(0))
		{
			Vector3 targetHeading = mainCamera.transform.forward - new Vector3(0, mainCamera.transform.forward.y, 0);
			Quaternion rotation = Quaternion.FromToRotation(characterTransform.forward, targetHeading);
			animator.MatchTarget(characterTransform.position, rotation*characterTransform.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1f),0.0f, 1f);
		}
	}

	/** 
	private float priorCombatMoveFwd = 0f;
	private float priorCombatMoveRight = 0f;
	private float combatMoveFwdV = 0f;
	private float combatMoveRightV = 0f;
	
	override protected void HandleCombatMovement()
	{
		
		float inputMoveFwd = 0f;
		float inputMoveRight = 0f;
		
		// smoothing:
		inputMoveFwd = Mathf.SmoothDamp(priorCombatMoveFwd, moveFwd, ref combatMoveFwdV, 0.2f);
		inputMoveRight = Mathf.SmoothDamp(priorCombatMoveRight, moveRight, ref combatMoveRightV, 0.2f);

		priorCombatMoveFwd = inputMoveFwd;
		priorCombatMoveRight = inputMoveRight;

		animator.SetFloat (hash.fMoveFwd, inputMoveFwd);
		animator.SetFloat (hash.fMoveRight, inputMoveRight);
		
		if(this.ActiveTarget != null)
		{
			Vector3 dir = ActiveTarget.position - this.characterTransform.position;
			dir = dir - new Vector3(0f, dir.y, 0f);
			Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
			this.characterTransform.rotation = rot;
		}
	}
	**/


	public void ChargeJump()
	{
		chargingJump = true;
	}
	
	public void LaunchJump()
	{
		float chargeTime = jumpTimer;
		jumpTimer = 0f;
		chargingJump = false;

		animator.applyRootMotion = false;
		animator.SetBool(hash.bAirborne, true);
		airborne = true;
		Vector3 jumpDirection = moveFwd*characterTransform.forward + moveRight*characterTransform.right;

		float jumpStrength = Mathf.Clamp(15f*chargeTime, 4f, 10f);

		rigidBody.AddForce(jumpStrength*Vector3.up + 3f*jumpDirection, ForceMode.VelocityChange);

		characterState = CharacterState.Falling;
	}

	public Transform GetNearestEnemy()
	{
		AIEnemy closestEnemy = null;
		float attackDist = this.GetAttackDist();
		
		// New way: First we identify the closest enemy based on what the camera's looking at.
		float toBeat = 0f;
		foreach(AIEnemy enemy in gameController.enemies)
		{
			if(enemy.gameObject.activeSelf == false)
			{
				continue;
			}
			// rank the enemy
			Vector3 d = enemy.transform.position - characterTransform.position;
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

		if(closestEnemy != null)
		{
			return closestEnemy.transform;
		}

		else // next we try to find the closest enemy based on what the player character is looking at. 
		{
			toBeat = -1f;
			foreach(AIEnemy enemy in gameController.enemies)
			{
				if(enemy.gameObject.activeSelf == false)
				{
					continue;
				}
				// rank the enemy
				Vector3 d = enemy.transform.position - characterTransform.position;
				Vector3 dir = characterTransform.forward;
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
			if(closestEnemy != null)
			{
				return closestEnemy.transform;
			}
		}
		return null;
	}
	
	private float GetAttackDist()
	{
		// TODO: Update this based on the active power. 
		return Constants.MaxAttackMeleeDist;
	}

	override public void DoMelee()
	{
		base.DoMelee();
	}

	override public void DoKO()
	{
		Debug.Log ("You died!");
	}
}
