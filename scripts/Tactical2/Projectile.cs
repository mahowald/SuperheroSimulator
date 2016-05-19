using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ProjectileEffect { None, TKEffect };

public class Projectile : MonoBehaviour {

	public float speed; // how fast do we go
	public float range; // how far do we go before deleting ourselves?

	public bool destroyOnHit = true; // do we destroy on hit?

	public GameObject spawnOnHit = null; // what do we spawn upon collision?

	public CombatCharacterController creator = null; // who fired this projectile?

	public ProjectileEffect effect;

	public AttackData attackData; // the attack data about the projectile.

	private float distanceCovered = 0f;

	public bool ragdoll = false;

	public Vector3 force = Vector3.zero;

	private List<Transform> hits; // the number of targets we've already hit

	// Use this for initialization
	void Awake () {
		hits = new List<Transform>();
	}



	// Update is called once per frame
	void FixedUpdate () 
	{
		this.transform.position = this.transform.position + speed*Time.deltaTime*this.transform.forward;
		distanceCovered += speed*Time.deltaTime;

		if(distanceCovered >= range)
		{
			Object.Destroy(this.gameObject);
		}
	}


	void OnTriggerEnter(Collider other)
	{
		CombatCharacterController handler = other.GetComponentInParent<CombatCharacterController>();

		if(handler == creator)
		{
			return;
		}

		if(handler != null)
		{
			if(hits.Contains(handler.transform)) // this way, we only fire once.
			{
				return;
			}
			handler.DoHit(this);
			hits.Add(handler.transform);

			Ragdoll r = handler.GetComponent<Ragdoll>();
			if(r != null)
			{
				r.Ragdolled = this.ragdoll;
				r.ApplyForce(this.GetAbsoluteForce());
			}

			if(this.effect == ProjectileEffect.TKEffect && r != null)
			{
				r.Ragdolled = true;
				TKEffect tkeffect = handler.gameObject.AddComponent<TKEffect>();
				tkeffect.BeginTKEffect(r.root);

			}
		}
		else
		{
			Rigidbody rb = other.GetComponent<Rigidbody>();
			if(rb != null && this.effect != ProjectileEffect.TKEffect)
			{
				rb.AddForce(this.transform.rotation*force);
			}

			if(this.effect == ProjectileEffect.TKEffect && rb != null)
			{
				TKEffect tkeffect = rb.gameObject.AddComponent<TKEffect>();
				tkeffect.BeginTKEffect(rb);
			}

		}

		if(spawnOnHit != null)
		{
			GameObject.Instantiate(spawnOnHit, this.transform.position, this.transform.rotation);
		}

		if(destroyOnHit)
		{
			Object.Destroy(this.gameObject);
		}
	}

	public Vector3 GetAbsoluteForce()
	{
		return this.transform.rotation*force;
	}

}
