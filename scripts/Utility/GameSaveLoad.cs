using UnityEngine;
using System.Collections;

public class GameSaveLoad : MonoBehaviour {

	public CharacterAppearanceModifier charModifier;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SaveGame()
	{
		charModifier.SaveToGlobalData();
		GlobalData.SaveGameState(@"Data\Saves\save.xml");
	}

	public void LoadGame()
	{
		GlobalData.LoadGameState(@"Data\Saves\save.xml");
		charModifier.LoadFromGlobalData();
	}
}
