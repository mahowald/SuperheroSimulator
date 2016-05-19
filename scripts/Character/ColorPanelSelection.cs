using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorPanelSelection : MonoBehaviour {

	public CharacterAppearanceModifier.ColorType colorType;

	public CharacterAppearanceModifier charmodifier;

	public void ApplyColor(Color color)
	{
		charmodifier.SetColor(colorType, color);
	}

	public void ApplyColor(Button button)
	{
		ApplyColor(button.colors.normalColor);
	}
}
