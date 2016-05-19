using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipmentSelection : MonoBehaviour {

	public enum SelectorType
	{
		Slider,
		Toggle
	}

	public SelectorType selectorType;
	public CharacterAppearanceModifier.CustomizationSlot category;

	public Toggle toggle;
	public Slider slider;

	public CharacterAppearanceModifier appearanceModifier;

	void Start()
	{
		if(selectorType == SelectorType.Slider)
		{
			if(category == CharacterAppearanceModifier.CustomizationSlot.emblem)
			{
				slider.minValue = 0;
				slider.maxValue = appearanceModifier.emblems.Count - 1;
			}
		}
	}

	public void AssignValue()
	{
		if(selectorType == SelectorType.Slider)
		{
			appearanceModifier.SetCustomizationSlot(category, (int)slider.value);
		}

		if(selectorType == SelectorType.Toggle)
		{
			if(category == CharacterAppearanceModifier.CustomizationSlot.cape)
			{
				appearanceModifier.SetCape(toggle.isOn);
			}
		}
	}



}
