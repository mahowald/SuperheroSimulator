using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterAppearanceModifier : MonoBehaviour {
	

	public enum ShapeKey
	{
		nose_up,
		nose_down,
		nose_wide,
		nose_out,
		mouth_wide,
		mouth_big,
		cheekbones_out,
		eyes_large,
		eyes_back,
		eyes_up,
		eyes_down,
		cheeks_wide,
		cheeks_narrow,
		blink,
	};

	public enum ColorType
	{
		skin,
		hair,
		primary,
		secondary,
		tertiary,
		cape,
		accessories,
		emblem,
	};

	public enum CustomizationSlot
	{
		hair,
		cape,
		equipment,
		emblem,
	};

	public Dictionary<ColorType, Color> colors;

	public Dictionary<ShapeKey, float> blendWeights;
	
	public GameObject blendedMesh;
	
	public List<Dictionary<string, int>> blendshapeLookupDict;
	
	public List<SkinnedMeshRenderer> renderers;

	public List<SkinnedMeshRenderer> hairMeshes;
	public List<SkinnedMeshRenderer> skinMeshes;
	public List<SkinnedMeshRenderer> suitMeshes;
	
	public List<SkinnedMeshRenderer> capeMeshes;
	public List<SkinnedMeshRenderer> accessoryMeshes;

	public List<Texture> emblems;

	public Dictionary<CustomizationSlot, int> customizationSlots;

    public bool loadOnStart = false;

	// Use this for initialization
	void Start () {
		SetupColors ();
		SetupBlendWeights();
		SetupRenderers();
		SetupCustomizationSlots();
	}
	
	// Update is called once per frame
	void Update () {
        if(loadOnStart)
        {
            LoadFromGlobalData();
            loadOnStart = false;
        }
	
	}

	public void SaveToGlobalData()
	{
		// colors
		GlobalData.playerData.appearance.hairTint = colors[ColorType.hair];
		GlobalData.playerData.appearance.skinTint = colors[ColorType.skin];
		GlobalData.playerData.appearance.primaryColor = colors[ColorType.primary];
		GlobalData.playerData.appearance.secondaryColor = colors[ColorType.secondary];
		GlobalData.playerData.appearance.tertiaryColor = colors[ColorType.tertiary];
		GlobalData.playerData.appearance.accessoriesColor = colors[ColorType.accessories];
		GlobalData.playerData.appearance.capeColor = colors[ColorType.cape];
		GlobalData.playerData.appearance.emblemColor = colors[ColorType.emblem];

		// shapekeys
		GlobalData.playerData.appearance.shape.nose_up = blendWeights[ShapeKey.nose_up];
		GlobalData.playerData.appearance.shape.nose_down = blendWeights[ShapeKey.nose_down];
		GlobalData.playerData.appearance.shape.nose_wide = blendWeights[ShapeKey.nose_wide];
		GlobalData.playerData.appearance.shape.nose_out = blendWeights[ShapeKey.nose_out];
		GlobalData.playerData.appearance.shape.mouth_wide = blendWeights[ShapeKey.mouth_wide];
		GlobalData.playerData.appearance.shape.mouth_big = blendWeights[ShapeKey.mouth_big];
		GlobalData.playerData.appearance.shape.cheekbones_out = blendWeights[ShapeKey.cheekbones_out];
		GlobalData.playerData.appearance.shape.eyes_large = blendWeights[ShapeKey.eyes_large];
		GlobalData.playerData.appearance.shape.eyes_back = blendWeights[ShapeKey.eyes_back];
		GlobalData.playerData.appearance.shape.eyes_up = blendWeights[ShapeKey.eyes_up];
		GlobalData.playerData.appearance.shape.eyes_down = blendWeights[ShapeKey.eyes_down];
		GlobalData.playerData.appearance.shape.cheeks_wide = blendWeights[ShapeKey.cheeks_wide];
		GlobalData.playerData.appearance.shape.cheeks_narrow = blendWeights[ShapeKey.cheeks_narrow];

        GlobalData.playerData.appearance.emblemIndex = customizationSlots[CustomizationSlot.emblem];
        GlobalData.playerData.appearance.hasCape = caped;
	}

	public void LoadFromGlobalData()
	{
		// colors
		colors[ColorType.hair] = GlobalData.playerData.appearance.hairTint;
		colors[ColorType.skin] = GlobalData.playerData.appearance.skinTint;
		colors[ColorType.primary] = GlobalData.playerData.appearance.primaryColor;
		colors[ColorType.secondary] = GlobalData.playerData.appearance.secondaryColor;
		colors[ColorType.tertiary] = GlobalData.playerData.appearance.tertiaryColor;
		colors[ColorType.accessories] = GlobalData.playerData.appearance.accessoriesColor;
		colors[ColorType.cape] = GlobalData.playerData.appearance.capeColor;
		colors[ColorType.emblem] = GlobalData.playerData.appearance.emblemColor;
		
		// shapekeys
		blendWeights[ShapeKey.nose_up] = GlobalData.playerData.appearance.shape.nose_up;
		blendWeights[ShapeKey.nose_down] = GlobalData.playerData.appearance.shape.nose_down;
		blendWeights[ShapeKey.nose_wide] = GlobalData.playerData.appearance.shape.nose_wide;
		blendWeights[ShapeKey.nose_out] = GlobalData.playerData.appearance.shape.nose_out;
		blendWeights[ShapeKey.mouth_wide] = GlobalData.playerData.appearance.shape.mouth_wide;
		blendWeights[ShapeKey.mouth_big] = GlobalData.playerData.appearance.shape.mouth_big;
		blendWeights[ShapeKey.cheekbones_out] = GlobalData.playerData.appearance.shape.cheekbones_out;
		blendWeights[ShapeKey.eyes_large] = GlobalData.playerData.appearance.shape.eyes_large;
		blendWeights[ShapeKey.eyes_back] = GlobalData.playerData.appearance.shape.eyes_back;
		blendWeights[ShapeKey.eyes_up] = GlobalData.playerData.appearance.shape.eyes_up;
		blendWeights[ShapeKey.eyes_down] = GlobalData.playerData.appearance.shape.eyes_down;
		blendWeights[ShapeKey.cheeks_wide] = GlobalData.playerData.appearance.shape.cheeks_wide;
		blendWeights[ShapeKey.cheeks_narrow] = GlobalData.playerData.appearance.shape.cheeks_narrow;

        // customization slots
        customizationSlots[CustomizationSlot.emblem] = GlobalData.playerData.appearance.emblemIndex;

        // cape
        SetCape(GlobalData.playerData.appearance.hasCape);

		UpdateAllColors();
		UpdateAllBlendWeights();
        UpdateAllCustomizationSlots();
	}

	private void SetupCustomizationSlots()
	{
		customizationSlots = new Dictionary<CustomizationSlot, int>()
		{
			{CustomizationSlot.hair, 0},
			{CustomizationSlot.cape, 0},
			{CustomizationSlot.emblem, 0},
			{CustomizationSlot.equipment, 0},
		};
	}

	private void SetupColors()
	{
		colors = new Dictionary<ColorType, Color>()
		{
			{ColorType.hair, Color.black},
			{ColorType.skin, Color.white},
			{ColorType.primary, Color.red},
			{ColorType.secondary, Color.blue},
			{ColorType.tertiary, Color.green},
			{ColorType.accessories, Color.green},
			{ColorType.cape, Color.green},
			{ColorType.emblem, Color.green},
		};
	}

	private void SetupBlendWeights()
	{
		blendWeights = new Dictionary<ShapeKey, float>()
		{
			{ ShapeKey.nose_up, 0f},
			{ ShapeKey.nose_down, 0f},
			{ ShapeKey.nose_wide, 0f},
			{ ShapeKey.nose_out, 0f},
			{ ShapeKey.mouth_wide,0f},
			{ ShapeKey.mouth_big,0f},
			{ ShapeKey.cheekbones_out,0f},
			{ ShapeKey.eyes_large,0f},
			{ ShapeKey.eyes_back,0f},
			{ ShapeKey.eyes_down,0f},
			{ ShapeKey.eyes_up,0f},
			{ ShapeKey.cheeks_wide,0f},
			{ ShapeKey.cheeks_narrow,0f},
		};
	}

	private void SetupRenderers()
	{
		
		SkinnedMeshRenderer[] srenderers = blendedMesh.GetComponentsInChildren<SkinnedMeshRenderer>();
		
		renderers = new List<SkinnedMeshRenderer>(srenderers);
		blendshapeLookupDict = new List<Dictionary<string, int>>();
		
		for(int i = 0; i < renderers.Count; i++)
		{
			Mesh m = renderers[i].sharedMesh;
			Dictionary<string, int> mdict = new Dictionary<string, int>();
			for(int j = 0; j < m.blendShapeCount; j++)
			{
				string bshapename = m.GetBlendShapeName(j);
				mdict.Add(bshapename, j);
			}
			blendshapeLookupDict.Add(mdict);
		}
	}
	
	public void ModifyBlendWeights(float delta, ShapeKey key)
	{

		float currentValue = blendWeights[key];
		float newValue = currentValue + delta;
		newValue = Mathf.Clamp(newValue, 0f, 100f);
		blendWeights[key] = newValue;
		SetBlendShape(blendWeights[key], key);
	}

    

	private void UpdateAllBlendWeights()
	{
		List<ShapeKey> bkeys = new List<ShapeKey>(blendWeights.Keys);
		for(int i = 0; i < bkeys.Count; i++)
		{
			SetBlendShape(blendWeights[bkeys[i]], bkeys[i]);
		}
	}

	private void SetBlendShape(float val, ShapeKey key)
	{
		string shapekey = key.ToString(); // We do this to force people to pick valid shapekeys.
		for(int i = 0; i < renderers.Count; i++)
		{
			if(blendshapeLookupDict[i].ContainsKey(shapekey))
			{
				renderers[i].SetBlendShapeWeight(blendshapeLookupDict[i][shapekey], val);
			}
		}
	}

	public void SetColor(ColorType colorType, Color color)
	{
		colors[colorType] = color;

		if(colorType == ColorType.skin)
		{
			for(int i = 0; i < skinMeshes.Count; i++)
			{
				skinMeshes[i].GetComponent<Renderer>().material.SetColor("_Color", color);
			}
			return;
		}

		if(colorType == ColorType.hair)
		{
			for(int i = 0; i < hairMeshes.Count; i++)
			{
				hairMeshes[i].GetComponent<Renderer>().material.SetColor("_Color", color);
			}
			return;
		}

		if(colorType == ColorType.cape)
		{
			for(int i = 0; i < capeMeshes.Count; i++)
			{
				List<Material> materials = new List<Material>(capeMeshes[i].materials);
				
				for(int j = 0; j < materials.Count; j++)
				{
					materials[j].SetColor("_SecColor", color);
				}
			}
			return;
		}
		
		
		if(colorType == ColorType.accessories)
		{
			for(int i = 0; i < accessoryMeshes.Count; i++)
			{
				List<Material> materials = new List<Material>(accessoryMeshes[i].materials);
				
				for(int j = 0; j < materials.Count; j++)
				{
					materials[j].SetColor("_Color", color);
				}
			}
			return;
		}

		for(int i = 0; i < suitMeshes.Count; i++)
		{

			List<Material> materials = new List<Material>(suitMeshes[i].materials);

			for(int j = 0; j < materials.Count; j++)
			{

				switch(colorType)
				{
				case ColorType.primary:
					materials[j].SetColor("_PriColor", color);
					break;
				case ColorType.secondary:
					materials[j].SetColor("_SecColor", color);
					break;
				case ColorType.tertiary:
					materials[j].SetColor("_TerColor", color);
					break;
				case ColorType.emblem:
					materials[j].SetColor("_DetailColor", color);
					break;
				default:
					break;
				}
			}
		}

	}

	private void UpdateAllColors()
	{
		List<ColorType> ckeys = new List<ColorType>(colors.Keys);
		for(int i = 0; i < ckeys.Count; i++)
		{
			SetColor(ckeys[i], colors[ckeys[i]]);
		}
	}

    private bool caped;

	public void SetCape(bool hasCape)
	{
        caped = hasCape;
		for(int i = 0; i < capeMeshes.Count; i++)
		{
			capeMeshes[i].gameObject.SetActive(hasCape);
		}
	}

    private void UpdateAllCustomizationSlots()
    {
        List<CustomizationSlot> slots = new List<CustomizationSlot>(customizationSlots.Keys);
        foreach(CustomizationSlot slot in slots)
        {
            SetCustomizationSlot(slot, customizationSlots[slot]);
        }
    }

    public void SetCustomizationSlot(CustomizationSlot slot, int index)
	{
		// update the dictionary
		customizationSlots[slot] = index;

		if(slot == CustomizationSlot.emblem)
		{
			if(index >= emblems.Count)
			{
				Debug.Log("Error: emblem index out of bounds.");
				return;
			}
			for(int i = 0; i < suitMeshes.Count; i++)
			{
				List<Material> materials = new List<Material>(suitMeshes[i].materials);
				
				for(int j = 0; j < materials.Count; j++)
				{
					materials[j].SetTexture("_DetailMask", emblems[index]);
					materials[j].SetTexture("_DetailAlbedoMap", emblems[index]);
				}
			}
			return;
		}
	}

}
