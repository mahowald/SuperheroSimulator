using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighlightObject : MonoBehaviour {

	private Dictionary<Renderer, Material[]> originals; 

	private bool highlighted = false;

	public Color highlightColor;

	public static Material highlightMaterial = Resources.Load<Material>("resources/gfx/materials/HighlightMaterial");

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
		PopulateDictionary();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Keypad2))
		{
			Highlighted = !Highlighted;
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
			Material hmaterial = Material.Instantiate<Material>(highlightMaterial);
			hmaterial.SetColor("_Color", highlightColor);
			r.materials = highlightedMats;

		}
	}

	void UnsetHighlight()
	{
		foreach(Renderer r in originals.Keys)
		{
			r.materials = originals[r];
		}
	}
}
