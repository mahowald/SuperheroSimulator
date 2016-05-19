using UnityEngine;
using System.Collections;


public enum CameraMode {
	Free,
	Target,
	Locked,
};


public class CameraHandler : MonoBehaviour {

	public CameraMode mode;
	public Transform target;
	public Transform player;
	public Transform baseObject;

	private float cameraSpeed;

	private float cameraDistance;

	private Vector3 freeOffset = new Vector3(0f, 0f, -1f);

	private Vector3 targetOffset = new Vector3(0f, 2f, -2.5f);
	private Quaternion lockedRotation = Quaternion.Euler(30f, 0f, 0f);
	private Quaternion freeRotation = Quaternion.Euler(0f, 0f, 0f);

	private float lastScroll = 0f;
	private float scrollVelocity = 0f;

	public TacticalObject highlightedObject = null;

//	Quaternion lastRotation;


	// Use this for initialization
	void Start () {
		// mode = CameraMode.Free;
		cameraSpeed = Constants.CameraSpeed;
//		lastRotation = this.transform.rotation;
		cameraDistance = this.transform.localPosition.z;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		switch(mode)
		{

		case CameraMode.Locked:
			this.transform.localPosition = targetOffset;
			this.transform.localRotation = lockedRotation;
			break;
		case CameraMode.Target:
			this.transform.localPosition = targetOffset;
			this.transform.localRotation = lockedRotation;
			break;
		case CameraMode.Free:
			this.transform.localRotation = freeRotation;
			FreeCameraMode();
			SetCameraDistance();
			HighlightTarget();
			break;
		default:
			break;
		}

		// transform.position -= (transform.position - player.position)*Time.deltaTime;

	}

	void SetCameraDistance()
	{
		int playerLayerMask = 1 << Constants.PlayerLayer;
		int enemyLayerMask = 1 << Constants.EnemyLayer;
		int layerMask = playerLayerMask | enemyLayerMask;

		
		layerMask = ~layerMask;

		RaycastHit hit;

		Vector3 parentPos = this.transform.parent.transform.position;

		if(Physics.Raycast (parentPos, this.transform.position - parentPos, out hit, Mathf.Abs (cameraDistance), layerMask))
		{
			float dist = Vector3.Magnitude(hit.point - parentPos);
			transform.localPosition = -1f*dist*(new Vector3(0, 0, 1));
		}
		else
		{
			transform.localPosition = cameraDistance*(new Vector3(0, 0, 1));
		}
	}

	void FreeCameraMode() // Now scrolling just affects how close the camera is to the character.
	{
		float scroll = Input.GetAxis ("LookZoom")*cameraSpeed*Time.deltaTime;
		
		scroll = Mathf.SmoothDamp(lastScroll, scroll, ref scrollVelocity, 0.1f);

		if(scroll > 0 && transform.localPosition.z < -0.7f)
		{
			cameraDistance += scroll;
		}
		if(scroll < 0 && transform.localPosition.z > -5f)
		{
			cameraDistance += scroll;
		}

		lastScroll = scroll;

	}

	void HighlightTarget()
	{
		RaycastHit hit;
		LayerMask mask = LayerMask.GetMask("Player");
		mask = ~mask; 
		if(Physics.Raycast(this.transform.position, this.transform.forward, out hit, 100f, mask))
		{
			TacticalObject targeted = hit.transform.gameObject.GetComponentInParent<TacticalObject>();
			if(targeted != null && targeted != highlightedObject) // we see a highlighted object
			{
				targeted.Highlighted = true;
				if(highlightedObject != null)
				{
					highlightedObject.Highlighted = false;
				}
				highlightedObject = targeted;
			}
			else
			{
				if(targeted == null && highlightedObject != null)
				{
					highlightedObject.Highlighted = false;
					highlightedObject = null;
				}
			}
		}
		else
		{
			if(highlightedObject != null)
			{
				highlightedObject.Highlighted = false;
				highlightedObject = null;
			}

		}
	}


	/** 
	void FreeCameraMode()
	{

		float rotateY = 0.5f*Input.GetAxis("LookVertical")*cameraSpeed*Time.deltaTime; // + looks up, - looks down
		float rotateX = Input.GetAxis("LookHorizontal")*cameraSpeed*Time.deltaTime;


		transform.RotateAround(player.position, Vector3.up, rotateX);

		float lastx = lastRotation.eulerAngles.x;

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

		transform.RotateAround(player.position, this.transform.right, rotateY);

		// transform.LookAt(player.position + heightOffset);

		lastRotation = transform.rotation;
	}
	**/

	float GetCameraDistance()
	{
		return Vector3.Magnitude(this.transform.position - player.position);
	}
	
}
