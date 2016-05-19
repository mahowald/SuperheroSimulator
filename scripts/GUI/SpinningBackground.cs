using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpinningBackground : MonoBehaviour {

	private Transform mytransform;

	public float speed;

	public Vector3 axis;

	// Use this for initialization
	void Start () {

		mytransform = this.transform;
	
	}
	
	// Update is called once per frame
	void Update () {
		mytransform.Rotate(speed*Time.deltaTime*axis, Space.Self);
	}
}
