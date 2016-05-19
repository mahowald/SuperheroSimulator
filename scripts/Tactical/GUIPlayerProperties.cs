using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIPlayerProperties : MonoBehaviour {

	public PlayerCharacterController player;
	public Slider healthbarSlider;

	private CombatantData startCombatantData;

	// Use this for initialization
	void Start () {
		startCombatantData = player.combatantData;
	}
	
	// Update is called once per frame
	void Update () {
	
		// Update healthbar:
		healthbarSlider.value = player.combatantData.health/startCombatantData.health;

	}
}
