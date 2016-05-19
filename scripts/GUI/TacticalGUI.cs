using UnityEngine;
using System.Collections;

public class TacticalGUI : MonoBehaviour {

	public int menuWidth;
	public int menuHeight;
	public int buttonHeight;

// 	private Rect pauseBackground; 
//	private string pauseTitle = "Menu";


	private bool paused = false;
	
	public InputHandler inputHandler;

	public Canvas pauseCanvas;


	// Use this for initialization
	void Start () {
		Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
	//	pauseBackground = new Rect(Screen.width/2 - menuWidth/2f, Screen.height/2 - menuHeight/2f, menuWidth, menuHeight); // centers things
		
		pauseCanvas.enabled = false;
		StartCoroutine(PauseCoroutine());


	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnGUI()
	{

		/** 
		if(paused)
		{
			GUI.BeginGroup (pauseBackground);

			GUI.Box (new Rect(0, 0, menuWidth, menuHeight), pauseTitle);
			GUI.Button (MakeButtonRect(0), "Controls");
			GUI.Button (MakeButtonRect(1), "Equipment");
			GUI.Button (MakeButtonRect(2), "Leave Area");
			GUI.Button (MakeButtonRect(3), "Quit Game");

			GUI.EndGroup ();
		}

		else
		{

		}

**/
	}

	private Rect MakeButtonRect(int index)
	{
		return new Rect(5, 30 + index*(buttonHeight + 5f), menuWidth - 10f, buttonHeight);
	}

	IEnumerator PauseCoroutine()
	{
		int timeSinceChanged = 0;
		bool recentChange = false;
		while(true)
		{
			if(!recentChange)
			{
				if(Input.GetButton("Pause"))
				{
					if(Time.timeScale == 0)
					{
						paused = false;
						Time.timeScale = 1;
						Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        pauseCanvas.enabled = false;
					}
					else
					{
						paused = true;
						Time.timeScale = 0;
						Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        pauseCanvas.enabled = true;
					}
					inputHandler.paused = paused;
					recentChange = true;
				}
			}

			if(recentChange == true)
			{
				timeSinceChanged++;
				if(timeSinceChanged >= 20) // just enough so that we don't "flicker" the menu on fast machines
				{
					timeSinceChanged = 0;
					recentChange = false;
				}
			}

			yield return null;

		}
	}

	public void Resume()
	{
		paused = false;
		Time.timeScale = 1;
		Cursor.visible = false;
		pauseCanvas.enabled = false;
		inputHandler.paused = paused;

	}

	public void ExitGame()
	{
		Application.Quit();
	}


}
