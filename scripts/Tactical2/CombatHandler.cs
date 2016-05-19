using UnityEngine;
using System.Collections;

public abstract class CombatHandler : MonoBehaviour {

	abstract public void DoBasicAttack(); // performs attack
	abstract public void DoHit(Projectile projectile); // hit by opposing attack
	abstract public void DoKO(); // knocked out / killed
	abstract public void FireProjectile();

}
