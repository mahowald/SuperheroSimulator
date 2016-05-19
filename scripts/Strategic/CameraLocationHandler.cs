using UnityEngine;
using System.Collections;

public class CameraLocationHandler : MonoBehaviour {

	public Transform originalPosition;

	private Vector3 oldPosition;
	private Quaternion oldRotation;

	private bool move = false;
	private float timer;

	private Transform targetTransform;

	// Use this for initialization
	void Start () {
		timer = 0f;
	}
	
	// Update is called once per frame
	void Update () {

		if(move == true)
		{
			timer += Time.deltaTime;
			this.transform.position = Vector3.Lerp(oldPosition, targetTransform.position, timer / Constants.CameraMoveTime);
			this.transform.rotation = Quaternion.Lerp (oldRotation, targetTransform.rotation, timer / Constants.CameraMoveTime);
			if(timer/Constants.CameraMoveTime >= 0.99 )
			{
				this.transform.position = targetTransform.position;
				this.transform.rotation = targetTransform.rotation;
				timer = 0f;
				move = false;
			}
		}

		if(Input.GetButtonDown ("Attack2"))
		{
			SetTarget (originalPosition);
		}

	}

	public void SetTarget(Transform position)
	{
		oldPosition = this.transform.position;
		oldRotation = this.transform.rotation;
		move = true;
		timer = 0f;
		targetTransform = position;
	}
}
