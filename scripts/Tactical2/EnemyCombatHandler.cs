using UnityEngine;
using System.Collections;

public class EnemyCombatHandler : CombatHandler {

	private int hitCounter = 0;

	private Animator animator;
	private HashIDs hash;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag("GameController").GetComponent<HashIDs>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	override public void DoBasicAttack() // perform attack
	{
	}
	override public void DoHit(Projectile projectile) // hit by projectile
	{
		hitCounter++;
		Debug.Log ("hit " + hitCounter.ToString() + " times");
		animator.SetTrigger(hash.tDoHit);

		if(projectile.ragdoll)
		{
			Ragdoll r = this.GetComponent<Ragdoll>();
			r.Ragdolled = true;
			r.ApplyForce(projectile.GetAbsoluteForce());
		}

	}
	override public void DoKO() // knocked out / killed
	{
	}
	override public void FireProjectile()
	{
	}
}
