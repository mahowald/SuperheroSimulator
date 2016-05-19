using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrategicMaterialSetter : MonoBehaviour {

	public enum MaterialMode
	{
		Standard,
		Crime,
		Happiness,
		Ownership,
	};

	List<StrategicMapInteracter> myInteractors;

	// Use this for initialization
	void Start () {
        myInteractors = new List<StrategicMapInteracter>(this.gameObject.GetComponentsInChildren<StrategicMapInteracter>());
	}
	
	// Update is called once per frame
	void Update () {
	

	}

	public void SetMaterialMode(MaterialMode mode)
	{
		switch(mode)
		{
		case MaterialMode.Standard:
			SetStandardMode(true);
			break;
		default:
			SetClayMode(true);
			break;
		}
	}

	public void SetStandardMode(bool doChange)
	{
		if(!doChange)
			return;
        
        foreach(StrategicMapInteracter smi in myInteractors)
        {
            smi.SetMaterialMode(MaterialMode.Standard);
        }
	}

    public void SetOwnershipMode(bool doChange)
    {
        if (!doChange)
            return;

        foreach (StrategicMapInteracter smi in myInteractors)
        {
            smi.SetMaterialMode(MaterialMode.Ownership);
        }
    }

	public void SetClayMode(bool doChange)
	{
		if(!doChange)
			return;

        foreach (StrategicMapInteracter smi in myInteractors)
        {
            smi.SetMaterialMode(MaterialMode.Crime);
        }

    }
}
