using UnityEngine;
using System.Collections;

public class CharacterCreatorController : MonoBehaviour {

    public CharacterAppearanceModifier charModifier;

	// Use this for initialization
	void Start ()
    {
	}

    bool firstUpdate = true;
	// Update is called once per frame
	void Update ()
    {
        if(firstUpdate)
        {
            LoadGame();
            firstUpdate = false;
        }
	
	}

    void LoadGame()
    {
        Debug.Log("loading game");
        GlobalData.LoadGameState(@"Data\Saves\init.xml");
        charModifier.LoadFromGlobalData();
    }

    public void StartGame()
    {
        charModifier.SaveToGlobalData();
        GlobalData.SaveGameState(@"Data\Saves\save.xml");
        Application.LoadLevel("strategy_map");
    }
}
