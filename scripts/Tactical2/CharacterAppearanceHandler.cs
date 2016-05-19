using UnityEngine;
using System.Collections;

public class CharacterAppearanceHandler : MonoBehaviour {

	public GameObject maleHero;
	public GameObject femaleHero;

	// The scripts we have to set

	// On the player:
	public InputHandler inputHandler;
	public PlayerCharacterController locomotor;
	private Animator femaleAnimator;
	private Animator maleAnimator;

	// For the camera:
	public CameraHandler cameraHandler;
	public CameraBase cameraTracker;

	public bool characterSheet = false;


	void Start()
	{

		femaleAnimator = femaleHero.GetComponent<Animator>();
		maleAnimator = maleHero.GetComponent<Animator>();
		
		UpdateAppearance();
	}

	public void UpdateAppearance()
	{
		SetGender ();
		SetShape ();
		SetColors ();
		SetGear ();
	}

	void SetGender()
	{
		if(GlobalData.playerData.appearance.isFemale)
		{
			maleHero.SetActive(false);
			femaleHero.SetActive(true);

			if(characterSheet == false)
			{
				inputHandler.player = femaleHero;
				locomotor.animator = femaleAnimator;
				femaleAnimator.SetFloat ("Gender", 0f); 
				cameraHandler.player = femaleHero.transform;
				cameraTracker.player = femaleHero.transform;
			}
		}
		else
		{
			femaleHero.SetActive(false);
			maleHero.SetActive(true);
			
			if(characterSheet == false)
			{
				inputHandler.player = maleHero;
				locomotor.animator = maleAnimator;
				maleAnimator.SetFloat("Gender", 1f);
				cameraHandler.player = maleHero.transform;
				cameraTracker.player = maleHero.transform;
			}
		}
	}

	public Animator GetAnimator()
	{
		if(maleHero)
		{
			return maleAnimator;
		}
		if(femaleHero)
		{
			return femaleAnimator;
		}
		return null;
	}

	void SetColors()
	{
		foreach(SkinnedMeshRenderer part in this.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			// Names are from the 3 Color Toon Shader code. 
			part.GetComponent<Renderer>().material.SetColor ("_PriColor", GlobalData.playerData.appearance.primaryColor);
			part.GetComponent<Renderer>().material.SetColor ("_SecColor", GlobalData.playerData.appearance.secondaryColor);
			part.GetComponent<Renderer>().material.SetColor ("_TerColor", GlobalData.playerData.appearance.tertiaryColor);
		}
	}

	void SetShape()
	{
		/** 
		float shape = GlobalData.playerData.appearance.shape;
		if(shape == 0f) // the default setting.
		{
			return; 
		}

		foreach(SkinnedMeshRenderer part in this.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			if(part.sharedMesh.blendShapeCount < 2)
			{
				continue;
			}
			if(shape < 0) // slim
			{
				part.SetBlendShapeWeight(0, -1f*shape);
				part.SetBlendShapeWeight(1, 0f);
			}
			if(shape > 0) // muscular
			{
				part.SetBlendShapeWeight(0, 0f);
				part.SetBlendShapeWeight(1, shape);
			}
		}
		**/
	}

	void SetGear()
	{
	}

}
