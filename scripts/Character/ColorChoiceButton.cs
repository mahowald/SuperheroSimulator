using UnityEngine;
using System.Collections;

public class ColorChoiceButton : MonoBehaviour {

	public CharacterAppearanceModifier.ColorType colorType;
	public ColorPanelSelection colorPane;


	public void ShowChoice()
	{
		colorPane.gameObject.SetActive(true);
		colorPane.colorType = colorType;
	}

}
