using UnityEngine;
using System.Collections;

public class StrategicCameraControls : MonoBehaviour {

	public float cameraSpeed;

	private Transform myTransform;

	private float moveInX;
	private float moveInY;
	private float strength = 0f;

	private Vector3 xDir; 
	private Vector3 yDir;

	// Use this for initialization
	void Start () {

		myTransform = this.transform;

		xDir = myTransform.right - new Vector3(0f, myTransform.right.y, 0f);
		yDir = myTransform.forward - new Vector3(0f, myTransform.forward.y, 0f);

	
	}
	
	// Update is called once per frame
	void Update () {
		
		moveInX = Input.GetAxis("Horizontal");
		moveInY = Input.GetAxis("Vertical");

		strength = Mathf.Clamp (Mathf.Sqrt (moveInX*moveInX + moveInY*moveInY), 0, 1f);

		myTransform.position += Time.deltaTime*cameraSpeed*(moveInX*xDir + moveInY*yDir);

	}


}
