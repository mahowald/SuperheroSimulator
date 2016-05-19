using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DistrictHandler : StrategicMapInteracter {

    public District district;

    public Transform selectorMesh; // show this mesh when the district is selected

    private StrategicMaterialSetter.MaterialMode materialMode;

    bool selected = false;

    List<DistrictHandler> otherHandlers;

    StrategicGameController gameController;
    
    // Use this for initialization
    override public void Start()
    {
        base.Start();
        otherHandlers = new List<DistrictHandler>(this.transform.parent.GetComponentsInChildren<DistrictHandler>());
        otherHandlers.Remove(this);

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<StrategicGameController>();
    }

    // Update is called once per frame
    override public void Update()
    {

    }

    public override void SetMaterialMode(StrategicMaterialSetter.MaterialMode mode)
    {
        materialMode = mode;
        switch (mode)
        {
            case StrategicMaterialSetter.MaterialMode.Standard:
                SetStandardMode();
                break;
            case StrategicMaterialSetter.MaterialMode.Ownership:
                SetOwnershipMode();
                break;
            default:
                SetClayMode();
                break;
        }
    }

    public void UpdateDistrictHandler() // called on end of turn. 
    {
        UpdateMaterialMode();
    }
    
    private void UpdateMaterialMode()
    {
        SetMaterialMode(materialMode);
    }

    protected void SetOwnershipMode()
    {
        foreach (Renderer r in myRenderers)
        {
            r.material = clayMaterial;
            if(district.Controller == null)
            {
                r.material.color = new Color(180f/255f, 180f/255f, 180f/255f);
            }
            else
            {
                r.material.color = district.Controller.primaryColor;
            }
        }
    }

    public void OnMouseEnter()
    {
        if (selectorMesh == null)
            return;

        if (selected == true)
            return;

        selectorMesh.gameObject.SetActive(true);
        StartCoroutine(UnfadeSelector());
    }

    public void OnMouseExit()
    {
        if (selectorMesh == null)
            return;

        if (selected == true)
            return;
        
        StartCoroutine(FadeSelector());
    }

    public void OnMouseDown()
    {
        // make sure we're not clicking on a GUI element

        bool overGO = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        if (overGO)
            return;



        selected = !selected;
        if(selected)
        {
            foreach(DistrictHandler handler in otherHandlers)
            {
                if(handler.selected)
                {
                    handler.StartCoroutine(handler.FadeSelector());
                }
                handler.selected = false;
            }
            gameController.SelectDistrict(this.district);
        }
        else
        {
            gameController.SelectDistrict(null);
        }
    }

    IEnumerator FadeSelector()
    {
        Renderer r = selectorMesh.GetComponent<Renderer>();
        for(float f = 1f; f >= 0; f -= 0.05f)
        {
            Color c = r.material.color;
            c.a = f;
            r.material.color = c;
            yield return null;
        }
        selectorMesh.gameObject.SetActive(false);
        
    }

    IEnumerator UnfadeSelector()
    {
        Renderer r = selectorMesh.GetComponent<Renderer>();
        for (float f = 0f; f <= 1f; f += 0.05f)
        {
            Color c = r.material.color;
            c.a = f;
            r.material.color = c;
            yield return null;
        }
    }
}
