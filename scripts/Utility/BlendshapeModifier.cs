using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlendshapeModifier : MonoBehaviour {

	public enum ShapeFeature
	{
		nose,
		cheekbone,
		mouth,
		eyes,
		cheeks,
	};

	public ShapeFeature feature;

	public Transform cameraLocation;

    public RectTransform translateIcon;

	private bool tracking = false;

	private bool mouseStartOnRight; // the mouse started on the right side of the screen

	public CharacterAppearanceModifier charModifier;

	// Use this for initialization
	void Start () {
	
	}

	void Awake()
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(tracking)
		{
			SetBlendShape();
            translateIcon.position = Input.mousePosition;
        }
	}

	public void OnMouseDown()
	{
		mouseStartOnRight = (Input.mousePosition.x > Screen.width/2f);
		tracking = true;
	}

	public void OnMouseUp()
	{
		tracking = false;
        translateIcon.position = new Vector2(-10f, -10f);
        Cursor.visible = true;
    }
	
    public void OnMouseEnter()
    {
        Cursor.visible = false;
    }

    public void OnMouseOver()
    {
        translateIcon.position = Input.mousePosition;
    }

    public void OnMouseExit()
    {
        if (!tracking)
        {
            Cursor.visible = true;
            translateIcon.position = new Vector2(-10f, -10f);
        }
    }

	public Vector2 Delta
		// Returns normalized difference along the current path. 
	{
		get
		{
			return new Vector2(-1f*Input.GetAxis("LookHorizontal"), -1f*Input.GetAxis("LookVertical"));
		}
	}

	private void SetBlendShape()
	{
		switch(feature)
		{
		case ShapeFeature.cheekbone:
			SetCheekboneBlendShape();
			break;
		case ShapeFeature.cheeks:
			SetCheeksBlendShape();
			break;
		case ShapeFeature.eyes:
			SetEyesBlendShape();
			break;
		case ShapeFeature.mouth:
			SetMouthBlendShape();
			break;
		case ShapeFeature.nose:
			SetNoseBlendShape();
			break;
		default:
			break;
		}
	}

	private void SetNoseBlendShape()
	{
		float noseUpWeight = charModifier.blendWeights[CharacterAppearanceModifier.ShapeKey.nose_up];
		float noseDownWeight = charModifier.blendWeights[CharacterAppearanceModifier.ShapeKey.nose_down];

		// float noseOutWeight = charModifier.blendWeights[CharacterAppearanceModifier.ShapeKey.nose_out];
		// float noseWideWeight = charModifier.blendWeights[CharacterAppearanceModifier.ShapeKey.nose_wide];

		Vector2 delt = Delta;

		if(delt.y > 0)
		{
			if(noseDownWeight > 0f)
			{
				charModifier.ModifyBlendWeights(-1f*delt.y, CharacterAppearanceModifier.ShapeKey.nose_down);
			}
			else
			{
				charModifier.ModifyBlendWeights(delt.y, CharacterAppearanceModifier.ShapeKey.nose_up);
			}
		}

		if(delt.y < 0)
		{
			if(noseUpWeight > 0f)
			{
				charModifier.ModifyBlendWeights(delt.y, CharacterAppearanceModifier.ShapeKey.nose_up);
			}
			else
			{
				charModifier.ModifyBlendWeights(-1f*delt.y, CharacterAppearanceModifier.ShapeKey.nose_down);
			}
		}


		float dotProd = -1f*cameraLocation.forward.z;

		float deltax = delt.x;

		if(mouseStartOnRight)
		{
			deltax = -1f*deltax;
		}

		if(dotProd > 0.707) // then we are facing the front of the face. 0.707 ~ 1/sqrt(2)
		{
			charModifier.ModifyBlendWeights(deltax, CharacterAppearanceModifier.ShapeKey.nose_wide);
		}
		else
		{
			charModifier.ModifyBlendWeights(deltax, CharacterAppearanceModifier.ShapeKey.nose_out);
		}
	}

	private void SetCheekboneBlendShape()
	{
		float delt = Delta.x;
		
		if(mouseStartOnRight)
		{
			delt = -1f*delt;
		}

		charModifier.ModifyBlendWeights(delt, CharacterAppearanceModifier.ShapeKey.cheekbones_out);

	}

	private void SetCheeksBlendShape()
	{
		float cheekwideweight = charModifier.blendWeights[CharacterAppearanceModifier.ShapeKey.cheeks_wide];
		float cheeknarrowweight = charModifier.blendWeights[CharacterAppearanceModifier.ShapeKey.cheeks_narrow];

		float delt = Delta.x;

		if(mouseStartOnRight)
		{
			delt = -1f*delt;
		}

		if(delt < 0f)
		{
			if(cheekwideweight > 0f)
			{
				charModifier.ModifyBlendWeights(delt, CharacterAppearanceModifier.ShapeKey.cheeks_wide);
			}
			else
			{
				charModifier.ModifyBlendWeights(-1f*delt, CharacterAppearanceModifier.ShapeKey.cheeks_narrow);
			}
		}
		if(delt > 0f)
		{
			if(cheeknarrowweight > 0f)
			{
				charModifier.ModifyBlendWeights(-1f*delt, CharacterAppearanceModifier.ShapeKey.cheeks_narrow);
			}
			else
			{
				charModifier.ModifyBlendWeights(delt, CharacterAppearanceModifier.ShapeKey.cheeks_wide);
			}
		}
	}

	private void SetEyesBlendShape()
	{
		Vector2 delt = Delta;

		float dotProd = -1f*cameraLocation.forward.z;
		
		float deltax = delt.x;
		
		if(mouseStartOnRight)
		{
			deltax = -1f*deltax;
		}
		
		if(dotProd > 0.707) // then we are facing the front of the face. 0.707 ~ 1/sqrt(2)
		{
			charModifier.ModifyBlendWeights(deltax, CharacterAppearanceModifier.ShapeKey.eyes_large);
		}
		else
		{
			charModifier.ModifyBlendWeights(-1f*deltax, CharacterAppearanceModifier.ShapeKey.eyes_back);
		}

		float eyesUpWeight = charModifier.blendWeights[CharacterAppearanceModifier.ShapeKey.eyes_up];
		float eyesDownWeight = charModifier.blendWeights[CharacterAppearanceModifier.ShapeKey.eyes_down];
		
		if(delt.y > 0)
		{
			if(eyesDownWeight > 0f)
			{
				charModifier.ModifyBlendWeights(-1f*delt.y, CharacterAppearanceModifier.ShapeKey.eyes_down);
			}
			else
			{
				charModifier.ModifyBlendWeights(delt.y, CharacterAppearanceModifier.ShapeKey.eyes_up);
			}
		}
		
		if(delt.y < 0)
		{
			if(eyesUpWeight > 0f)
			{
				charModifier.ModifyBlendWeights(delt.y, CharacterAppearanceModifier.ShapeKey.eyes_up);
			}
			else
			{
				charModifier.ModifyBlendWeights(-1f*delt.y, CharacterAppearanceModifier.ShapeKey.eyes_down);
			}
		}


	}

	private void SetMouthBlendShape()
	{
		Vector2 delt = Delta;
		
		if(mouseStartOnRight)
		{
			delt = new Vector2(-1f*delt.x, delt.y);
		}

		charModifier.ModifyBlendWeights(delt.x, CharacterAppearanceModifier.ShapeKey.mouth_wide);
		charModifier.ModifyBlendWeights(delt.y, CharacterAppearanceModifier.ShapeKey.mouth_big);
	}



}
