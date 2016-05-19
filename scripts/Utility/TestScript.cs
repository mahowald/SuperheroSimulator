using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	public SkinnedMeshRenderer skinnedMesh;
	public Transform parent;
	public SkinnedMeshRenderer target;

	private SkinnedMeshRenderer instance;

	// Use this for initialization
	void Start () 
	{
	
		instance = (SkinnedMeshRenderer) GameObject.Instantiate(skinnedMesh, target.transform.position, target.transform.rotation);
		instance.transform.parent = target.transform.parent;
		instance.bones = target.bones;


		// instance.updateWhenOffscreen = true; // because the bounds don't update right!

	}
	
	// Update is called once per frame
	void Update () {
	}
}
