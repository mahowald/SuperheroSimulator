using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombatantHealthbar :  TacticalCharacterGUIElement {

	// These should be set in the Editor.
	public Slider healthbarSlider;
	public CombatCharacterController combatant;
	

	// Update is called once per frame
	override public void Update () {
		healthbarSlider.value = combatant.combatantData.health/combatant.startCombatantData.health;
		base.Update();

		if(combatant.combatantData.health <= 0f)
		{
			healthbarSlider.gameObject.SetActive(false);
		}
	}
}
