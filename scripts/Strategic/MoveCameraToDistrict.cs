using UnityEngine;
using System.Collections;

public class MoveCameraToDistrict : MonoBehaviour {

	public Transform viewpoint;
	public CameraLocationHandler handler;

	private Color trueBaseTint;
	private Color baseTint;

	private Color hoverTint;

	void Start () {
		hoverTint =  new Color(245f/255f, 245f/255f, 245f/255f);
		baseTint = this.GetComponent<Renderer>().material.GetColor("_Tint");
		trueBaseTint = baseTint;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void OnMouseEnter()
	{
		this.GetComponent<Renderer>().material.SetColor ("_Tint", hoverTint);
	}

	void OnMouseExit()
	{
		this.GetComponent<Renderer>().material.SetColor ("_Tint", baseTint);
	}

	void OnMouseUp()
	{
		handler.SetTarget(viewpoint);
	}

	public Color BaseTint
	{
		get
		{
			return baseTint;
		}
		set
		{
			baseTint = value;
			this.GetComponent<Renderer>().material.SetColor ("_Tint", baseTint);
		}
	}

	public void ResetTint()
	{
		this.BaseTint = trueBaseTint;
	}

}
