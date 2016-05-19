using UnityEngine;
using System.Collections;

public class CharacterCreatorCameraControls : MonoBehaviour {

	[SerializeField]
	private Transform rotationBase;

	public Transform RotationBase
	{
		get { return rotationBase; }
	}

	[SerializeField]
	private Transform bodyCameraLocation;
	public Transform BodyCameraLocation
	{
		get { return bodyCameraLocation; }
	}

	[SerializeField]
	private Transform faceCameraLocation;
	public Transform FaceCameraLocation
	{
		get { return faceCameraLocation; }
	}

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButton(1)) // 1 = right mouse button
		{
			UpdateRotation();
		}

		UpdateCameraZoom();
	
	}

	float rotateXVelocity = 0f;
	float lastRotateX = 0f;
	float cameraSpeed = Constants.CameraSpeed;

	void UpdateRotation()
	{
		float axis = Input.GetAxis("LookHorizontal");
		float rotateX = axis*cameraSpeed*Time.deltaTime;

		// smooth the values:
		
		rotateX = Mathf.SmoothDamp(lastRotateX, rotateX, ref rotateXVelocity, 0.1f);

		rotationBase.RotateAround(rotationBase.position, Vector3.up, rotateX);
		
		lastRotateX = rotateX;
	}

	float lastScroll = 0f;
	float scrollVelocity = 0f;
	float scrollPos = 0f; // 0 = body, 1 = head

	void UpdateCameraZoom()
	{
		float scroll = Input.GetAxis("LookZoom")*cameraSpeed*Time.deltaTime;
		
		scroll = Mathf.SmoothDamp(lastScroll, scroll, ref scrollVelocity, 0.1f);

		scrollPos += scroll;

		scrollPos = Mathf.Clamp(scrollPos, 0f, 1f);

		this.transform.position = Vector3.Lerp(bodyCameraLocation.position, faceCameraLocation.position, scrollPos);

		this.transform.rotation = Quaternion.Lerp(bodyCameraLocation.rotation, faceCameraLocation.rotation, scrollPos);
		
		lastScroll = scroll;
	}
}
