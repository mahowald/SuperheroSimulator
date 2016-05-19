using UnityEngine;
using System.Collections;

public class ObjectTracker : MonoBehaviour {

	public Transform follow; // Follow this object

	float smooth = 5f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void LateUpdate () {
		this.transform.position = Vector3.Lerp (this.transform.position, follow.position, Time.deltaTime*smooth);
	}
}
