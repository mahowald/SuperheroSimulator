using UnityEngine;
using System.Collections;

public class SplashControllerScript : MonoBehaviour {
    
    public void NewGame()
    {
        Application.LoadLevel("character_creation");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
