using UnityEngine;
using System.Collections;

public class CharacterCameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float rotate = Input.GetAxis ("RotateObject");
		this.transform.RotateAround(Vector3.zero, Vector3.up, 2*rotate*Constants.CameraSpeed*Time.deltaTime);
	
	}
}
