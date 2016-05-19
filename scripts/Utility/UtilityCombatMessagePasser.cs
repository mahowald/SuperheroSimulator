using UnityEngine;
using System.Collections;

public class UtilityCombatMessagePasser : MonoBehaviour {
	// This exists solely to pass the "fire projectile" method to the actual combat handler.

	public CombatCharacterController combatHandler;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DoMelee()
	{
		combatHandler.DoMelee();
	}

	public void FireProjectile()
	{
		combatHandler.FireProjectile();
	}
}
