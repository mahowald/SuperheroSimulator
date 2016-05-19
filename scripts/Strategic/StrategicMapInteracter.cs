using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrategicMapInteracter : MonoBehaviour {

    protected Dictionary<Renderer, Material> startMaterials;
    protected Material clayMaterial;

    protected List<Renderer> myRenderers;


    // Use this for initialization
    virtual public void Start ()
    {
        myRenderers = new List<Renderer>(this.gameObject.GetComponentsInChildren<Renderer>()); // doesn't include inactive ones

        startMaterials = new Dictionary<Renderer, Material>();

        foreach (Renderer r in myRenderers)
        {
            startMaterials.Add(r, r.sharedMaterial);
        }

        clayMaterial = Resources.Load<Material>("meshes/city/Materials/ClayMaterial");
    }
	
	// Update is called once per frame
	virtual public void Update () {
	
	}

    public virtual void SetMaterialMode(StrategicMaterialSetter.MaterialMode mode)
    {
        switch (mode)
        {
            case StrategicMaterialSetter.MaterialMode.Standard:
                SetStandardMode();
                break;
            default:
                SetClayMode();
                break;
        }
    }

    protected void SetStandardMode()
    {
        foreach (Renderer r in myRenderers)
        {
            r.material = startMaterials[r];
        }
    }

    protected void SetClayMode()
    {
        foreach (Renderer r in myRenderers)
        {
            r.material = clayMaterial;
        }
    }

}
