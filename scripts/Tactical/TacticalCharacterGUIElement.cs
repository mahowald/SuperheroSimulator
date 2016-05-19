using UnityEngine;
using System.Collections;

// This is a class intended for use for stuff like health bars
// or comments/exclaimations by a tactical character. 
// Basically anything "GUI" that should follow a tactical character around
// and be visible to the player. 
public class TacticalCharacterGUIElement : MonoBehaviour {

	public Transform cameraTransform;
	private Transform myParent;

	private RectTransform rect;

	// Use this for initialization
	virtual public void Start () {
		rect = this.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	virtual public void Update () {
		RotateToFacePlayer();
	}

	void RotateToFacePlayer()
	{
		rect.LookAt(cameraTransform.position);
	}


}
