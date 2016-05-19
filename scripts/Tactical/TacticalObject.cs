using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TacticalObject : MonoBehaviour 
{
	public enum TacticalObjectType {Ally, Enemy, Neutral, Prop};

	public TacticalObjectType tacticalType = TacticalObjectType.Prop; 

	public float cashValue = 0f; // how valuable are we, in units of cashmoney?
	public Rigidbody rb; // the rigidbody we pay attention to. 

	
	private Dictionary<Renderer, Material[]> originals; 
	private bool highlighted = false;
	private float rimPower = 1f;
	private Color highlightColor;

	private Material highlightMaterial; 
	

	public bool Highlighted
	{
		get
		{
			return highlighted;
		}
		set
		{
			highlighted = value;
			if(value)
			{
				SetHighlight();
			}
			else
			{
				UnsetHighlight();
			}
		}
	}
	// Use this for initialization
	void Start () {
		SetupHighlighting();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void SetupHighlighting()
	{
		switch(tacticalType)
		{
		case TacticalObjectType.Ally:
			highlightColor = Constants.allyHighlightColor;
			break;
		case TacticalObjectType.Enemy:
			highlightColor = Constants.enemyHighlightColor;
			break;
		case TacticalObjectType.Neutral:
			highlightColor = Constants.neutralHighlightColor;
			break;
		case TacticalObjectType.Prop:
			highlightColor = Constants.neutralHighlightColor;
			break;
		}
		
		highlightMaterial = new Material(Resources.Load("gfx/materials/HighlightMaterial", typeof(Material)) as Material);
		highlightMaterial.SetColor("_Color", highlightColor);

		PopulateDictionary();
	}

	// Damage calculations. 
	void OnCollisionEnter(Collision col)
	{
		try
		{
			// force:
			float f = Vector3.Dot(col.contacts[0].normal, col.relativeVelocity)*rb.mass;
		}
		catch(System.IndexOutOfRangeException e)
		{
			// Debug.Log (this.gameObject.name);
		}

	}

	void PopulateDictionary()
	{
		originals = new Dictionary<Renderer, Material[]>();
		Renderer[] renderers = this.GetComponentsInChildren<Renderer>(false);
		
		foreach(Renderer renderer in renderers)
		{
			originals.Add(renderer, renderer.sharedMaterials);
		}
	}
	
	void SetHighlight()
	{
		foreach(Renderer r in originals.Keys)
		{
			// because unity doesn't use List objects...
			Material[] highlightedMats = new Material[originals[r].Length + 1];
			originals[r].CopyTo(highlightedMats, 0);
			highlightedMats[highlightedMats.Length - 1] = highlightMaterial;
			r.materials = highlightedMats;

			// r.material = highlightMaterial;
			
		}
		/** 
		foreach(Renderer r in originals.Keys)
		{
			r.material.SetColor ("_RimColor",highlightColor);
			r.material.SetColor("_OutlineColor",highlightColor);
			r.material.SetFloat("_RimPower", rimPower);
		}
		**/
	}
	
	void UnsetHighlight()
	{
		foreach(Renderer r in originals.Keys)
		{
			r.sharedMaterials = originals[r];
		}
	}
}
