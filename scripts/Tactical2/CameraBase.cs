using UnityEngine;
using System.Collections;

public class CameraBase : MonoBehaviour {

	public Transform player; // Follow this object
	public Camera cameraObj; // The camera
	
	private float cameraSpeed;

	private float lastRotateX = 0f;
	private float lastRotateY = 0f;
	private float rotateXVelocity = 0f;
	private float rotateYVelocity = 0f;

//	float smooth = 10f;

	private CameraHandler cameraHandler;

	private Vector3 freeOffset = new Vector3(0f, 1.5f, 0f); // default offset at starting zoom
	private Vector3 zoomedOffset = new Vector3(0f, 2.5f, 0f); // offset at max zoom. 
	private Vector3 offset = new Vector3(0f, 1.5f, 0f); // default offset


	
	// Use this for initialization
	void Start () {
		cameraSpeed = Constants.CameraSpeed;
	}

	void Awake()
	{
		cameraHandler = cameraObj.GetComponent<CameraHandler>();
		offset = this.transform.position - player.position;
	}

	void LateUpdate () {
		// Update the position: 
		this.transform.position = player.position + offset; // = Vector3.Lerp (this.transform.position, follow.position, Time.deltaTime*smooth);

		// Update rotation:

		switch(cameraHandler.mode)
		{
		case CameraMode.Target:
			UpdateTargetRotation();
			break;
		case CameraMode.Locked:
			UpdateLockedRotation();
			break;
		case CameraMode.Free:
		default:
			UpdateFreeRotation ();
			UpdateFreeDistance();
			break;
		}
	}

	void UpdateLockedRotation()
	{
		this.transform.rotation = player.rotation;
	}

	void UpdateTargetRotation()
	{
		// this.transform.LookAt(cameraHandler.target, Vector3.up);
		Vector3 dir = cameraHandler.target.position - this.transform.position;
		dir = dir - new Vector3(0f, dir.y, 0f);
		Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
		this.transform.rotation = rot;
	}

	void UpdateFreeDistance()
	{
		float scale = -1f*(cameraObj.transform.localPosition.z + 1f)/4f;

		offset = Vector3.Lerp(freeOffset, zoomedOffset, scale);
	}

	void UpdateFreeRotation()
	{
		
		float rotateY = 0.5f*Input.GetAxis("LookVertical")*cameraSpeed*Time.deltaTime; // + looks up, - looks down
		float rotateX = Input.GetAxis("LookHorizontal")*cameraSpeed*Time.deltaTime;
		
		float lastx = transform.rotation.eulerAngles.x;

		// smooth the values:

		rotateX = Mathf.SmoothDamp(lastRotateX, rotateX, ref rotateXVelocity, 0.1f);
		rotateY = Mathf.SmoothDamp(lastRotateY, rotateY, ref rotateYVelocity, 0.1f);
		
		// clamp the angles
		if(lastx > 65 && lastx < 85)
		{
			if(rotateY > 0)
			{
				rotateY = 0;
			}
		}
		
		if(lastx > 300 && lastx < 340)
		{
			if(rotateY < 0)
			{
				rotateY = 0;
			}
		}

		transform.RotateAround(player.position, Vector3.up, rotateX);
		transform.RotateAround(player.position, this.transform.right, rotateY);

		lastRotateX = rotateX;
		lastRotateY = rotateY;



	}
}
