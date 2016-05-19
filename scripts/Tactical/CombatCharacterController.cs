using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICombatant
{
	
	void DoBasicAttack(); // performs attack
	void DoHit(Projectile projectile); // hit by projectile
	void DoHit(Vector3 hitDir, AttackData attack); // hit by melee
	void DoKO(); // knocked out / killed
	void FireProjectile(); // Shoot our gun
	void DoMelee(); // Do a melee attack
}

public struct AttackData
{
	public float dmg; // the raw damage of the attack.
	// public float armorDmg; // how much additional damage to do to armor. 
	public float force; // the amount of force the attack should apply.
	public bool ragdoll; // true = appropriate targets will ragdoll upon hit
	public bool breaksBlock; // true = if the character is blocking, the character will be forced out of block
}

public struct CombatantData
{
	public float health; // how much health we have. 0 = dead/knocked out!
	public float armor; // how much armor we have. armor reduces incoming damage. 
}


public enum CombatMode {Melee, Ranged};

public class CombatCharacterController : GenericCharacterController, ICombatant {

	public CombatMode combatMode = CombatMode.Melee;

	private ICombatant activeEnemy; 

	public bool block = false;

	protected Transform activeTarget;

	public GameObject projectilePrefab;

	protected AttackData currentAttack;

	public CombatantData combatantData;

	public CombatantData startCombatantData;

	virtual public Transform ActiveTarget // For the player, the active target will be determined based on the target mode.  
	{
		get
		{
			return activeTarget;
		}
		set
		{
			activeTarget = value;
		}
	}

	override public void Awake()
	{
		base.Awake();
		
		currentAttack.dmg = 20f;
		currentAttack.force = 5f;
		currentAttack.ragdoll = false;
		currentAttack.breaksBlock = false;
		
		
		combatantData.health = 100f;
		combatantData.armor = 0f;
		
		startCombatantData = combatantData;

	}

	override public void Update()
	{
		base.Update();

		if(characterState != CharacterState.Attacking && Attacking())
		{
			characterState = CharacterState.Attacking;
		}
		else if(characterState == CharacterState.Attacking && !Attacking())
		{
				characterState = CharacterState.Idle;
		}

		// blocking:

		animator.SetBool(hash.bBlocking, block);

		if(characterState != CharacterState.Blocking && Blocking())
		{
			characterState = CharacterState.Blocking;
		}
		else if(characterState == CharacterState.Blocking && !Blocking())
		{
				characterState = CharacterState.Idle;
		}

		if(characterState == CharacterState.Damaged && NotBeingDamaged())
		{
			characterState = CharacterState.Idle;

		}

		// Manage HitPoints

		if(combatantData.health <= 0f)
		{
			DoKO();
		}


	}

	protected void HandleAttacking()
	{
		if(!Attacking())
		 	characterState = CharacterState.Idle;
	}
	
	protected void HandleDamaged()
	{
		if(NotBeingDamaged())
			characterState = CharacterState.Idle;
	}
	
	protected void HandleDead()
	{
	}

	protected void HandleSpecialAbility()
	{
	}


	protected bool Attacking()
	{
		if(!animator.IsInTransition(0))
		{
			if(currentStateInfo.fullPathHash == hash.sBT_BasicAttack1)
			{
				return true;
			}
			if(currentStateInfo.fullPathHash == hash.sBT_BasicAttack2)
			{
				return true;
			}
			if(currentStateInfo.fullPathHash == hash.sBT_BasicAttack3)
			{
				return true;
			}
		}
		else
		{
			AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);
			if(nextStateInfo.fullPathHash == hash.sBT_BasicAttack1)
			{
				return true;
			}
			if(nextStateInfo.fullPathHash == hash.sBT_BasicAttack2)
			{
				return true;
			}
			if(nextStateInfo.fullPathHash == hash.sBT_BasicAttack3)
			{
				return true;
			}
		}

		return false;
	}

	protected bool Blocking()
	{
		if(!animator.IsInTransition(0))
		{
			if(currentStateInfo.fullPathHash == hash.sBT_Blocking)
				return true;
		}

		return false;
	}

	protected bool NotBeingDamaged()
	{
		return true;
	}


	virtual public void DoBasicAttack()
	{
		characterState = CharacterState.Attacking;

		animator.SetTrigger(hash.tBasicAttack);

		Transform enemy = this.ActiveTarget;

		if(enemy != null)
		{
			activeEnemy = enemy.GetComponentInParent<CombatCharacterController>();

			if(!animator.IsInTransition(0))
			{
				Vector3 targetDir = enemy.position - characterTransform.position;
				Vector3 qtargetDir = targetDir - new Vector3(0, targetDir.y, 0);

				Quaternion rotation = Quaternion.FromToRotation(characterTransform.forward, qtargetDir);
				if(combatMode == CombatMode.Melee) // For melee, we want to end up connecting with the target. 
				{
					//TODO: Match targets to make things animate clearly. For now:
					if(targetDir.magnitude <= Constants.MaxAttackMeleeDist)
					{
						StartCoroutine(MatchCombatTarget(enemy.position - Constants.AttackMeleeDist*targetDir.normalized, rotation*characterTransform.rotation));

						// animator.MatchTarget(enemy.position - Constants.AttackMeleeDist*targetDir.normalized, rotation*characterTransform.rotation, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 1, 1), 1f), 0.0f, 1f);
					}
				}
				else if(combatMode == CombatMode.Ranged) // For ranged, we don't change the end position of our animation.
				{
					animator.MatchTarget(characterTransform.position, rotation*characterTransform.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1f),0.0f, 1f);
				}

			}

		}
		else
		{
			activeEnemy = null;
			HandleNullTarget();
		}

	}

	protected IEnumerator MatchCombatTarget(Vector3 characterTargetPosition, Quaternion characterTargetRotation)
	{
		yield return new WaitForSeconds(0.1f);
		animator.MatchTarget(characterTargetPosition, characterTargetRotation, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 1, 1), 1f), 0.0f, 0.75f); // stop a little early.
	}

	virtual protected void HandleNullTarget() 
		// This gives a "hook" to add in any special behavior if the combatant can't find a nearby enemy when it attacks.
		// For example, the player character should still turn to face the target. 
	{
		return;
	}

	virtual public void DoHit(Projectile projectile)
	{
		Vector3 projectileDir = projectile.transform.forward;
		DoHit (projectileDir, projectile.attackData);
	}

	virtual public void DoHit(Vector3 hitDirection, AttackData attack)
	{
		characterState = CharacterState.Damaged;
        
		float dmg = attack.dmg;

		if(Blocking ())
		{
			dmg = 0f;
		}

        // Apply damage
        combatantData.health = Mathf.Clamp(combatantData.health + combatantData.armor - dmg, 0f, startCombatantData.health);


		//TODO: Play damage sound

		// Hit animation
		if(!CanAnimateHit())
		{
			return;
		}

		animator.SetTrigger(hash.tDoHit);

		
		float hitAngle = Vector3.Angle(-1f*characterTransform.forward, hitDirection)/90f;
		float sign = Vector3.Dot (characterTransform.right, hitDirection);
		if(sign >= 0)
		 	hitAngle = -1f*hitAngle;

		animator.SetFloat(hash.fHitDirection, hitAngle);

		Vector3 targetDirection = hitDirection;
		if(hitAngle < 1)
		{
			targetDirection = -1f*targetDirection;
		}

		targetDirection = targetDirection - new Vector3(0f, targetDirection.y, 0f);
		Quaternion rotation = Quaternion.FromToRotation(characterTransform.forward, targetDirection);

		StartCoroutine(MatchHitTarget(rotation*characterTransform.rotation));
	}



	protected IEnumerator MatchHitTarget(Quaternion characterTargetRotation)
	{
		yield return new WaitForSeconds(0.15f);
		animator.MatchTarget(Vector3.zero, characterTargetRotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1f), 0.0f, 0.2f);
	}

	virtual public void DoKO()
	{
		Ragdoll ragdoll = this.GetComponent<Ragdoll>();
		if(ragdoll != null)
		{
			ragdoll.Ragdolled = true;
		}
		else
		{
			this.gameObject.SetActive(false);
		}
	}

	virtual public void FireProjectile()
	{
		GameObject proj = (GameObject) Instantiate(projectilePrefab, characterTransform.position + characterTransform.forward + Vector3.up, characterTransform.rotation);
		proj.GetComponent<Projectile>().creator = this.GetComponentInParent<CombatCharacterController>();
	}

	virtual public void DoMelee()
	{
		if(activeEnemy != null)
		{
			activeEnemy.DoHit(characterTransform.forward, currentAttack);
		}
	}

	bool CanAnimateHit()
	{
		int stateNameHash = currentStateInfo.fullPathHash;

		if(stateNameHash == hash.sIdle || stateNameHash == hash.sLocomotion || stateNameHash == hash.sBT_Blocking)
		{
			return true;
		}

		return false;
	}

}
