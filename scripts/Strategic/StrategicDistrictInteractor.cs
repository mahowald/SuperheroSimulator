using UnityEngine;
using System.Collections;

public class StrategicDistrictInteractor : StrategicMapInteracter {

    public GameObject selectorMesh; // show this mesh when the district is selected



	// Use this for initialization
	override public void Start ()
    {
        base.Start();
	}
	
	// Update is called once per frame
	override public void Update () {
	
	}

    public void OnMouseEnter()
    {
        selectorMesh.SetActive(true);
    }

    public void OnMouseExit()
    {
        selectorMesh.SetActive(false);
    }

}
