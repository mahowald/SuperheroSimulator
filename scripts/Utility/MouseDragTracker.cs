using UnityEngine;
using System.Collections;

public class MouseDragTracker : MonoBehaviour {

	private bool tracking = false;

	private Vector2 startPosition = Vector2.zero; 
	private Vector2 deltaPosition = Vector2.zero;

	public bool Tracking
	{
		get
		{
			return tracking;
		}
		set
		{
			if(value && tracking != value)
			{
				startPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			}
			tracking = value;
		}
	}

	public Vector2 Delta
		// Returns normalized difference along the current path. 
	{
		get
		{
			return new Vector2(deltaPosition.x/Screen.width, deltaPosition.y/Screen.height);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if(tracking)
		{
			Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			deltaPosition = mousePosition - startPosition;
		}
	}

	void OnMouseDown()
	{
		Tracking = true;
	}

	void OnMouseUp()
	{
		Tracking = false;
	}
}
