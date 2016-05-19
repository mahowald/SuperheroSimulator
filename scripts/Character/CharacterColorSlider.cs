using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterColorSlider : MonoBehaviour {

    public Slider slider;
    public List<Color> colorOptions;

    public CharacterAppearanceModifier appearanceModifier;
    public CharacterAppearanceModifier.ColorType colorType;

    void Start()
    {
        slider.minValue = 0;
        slider.maxValue = colorOptions.Count - 1;
        slider.wholeNumbers = true;
    }

    public void AssignValue()
    {
        int ind = (int)Mathf.Round(slider.value);
        Color selectedColor = colorOptions[ind];
        appearanceModifier.SetColor(colorType, selectedColor);
    }
}
