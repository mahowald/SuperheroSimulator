using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterHairColorSelection : MonoBehaviour {

    public Slider slider;
    public List<Color> colorOptions;

    public CharacterAppearanceModifier appearanceModifier;

    void Start()
    {
        slider.minValue = 0;
        slider.maxValue = colorOptions.Count - 1;
        slider.wholeNumbers = true;
    }

    public void AssignValue()
    {
        int ind = (int) Mathf.Round(slider.value);
        Color selectedColor = colorOptions[ind];
        appearanceModifier.SetColor(CharacterAppearanceModifier.ColorType.hair, selectedColor);
    }
    
	
}
