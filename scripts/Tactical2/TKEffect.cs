using UnityEngine;
using System.Collections;

public class TKEffect : MonoBehaviour {

	// public Rigidbody testInput;

	private Rigidbody rb;
	private bool ragdoll = false;

	private Transform mainCamera;

	private Ragdoll ragdollScript;

	private Transform playerTransform;

	private PlayerCharacterController playerController;

	private Animator playerAnimator;

	private CameraHandler cameraHandler;

	private float xDisplacement = 0f;
	private float yDisplacement = 0f;


	public void BeginTKEffect(Rigidbody rigidbody)
	{
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
		cameraHandler = mainCamera.GetComponent<CameraHandler>();


		playerTransform = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GetHeroTransform();

		playerAnimator = playerTransform.GetComponent<Animator>();
		playerAnimator.SetBool("TKMode", true);

		playerController = playerTransform.GetComponentInParent<PlayerCharacterController>();
		playerController.characterState = CharacterState.SpecialAbility;
		playerController.ActiveTarget = this.transform;

		ragdollScript = this.GetComponent<Ragdoll>();
		if(ragdollScript)
		{
			ragdollScript.Ragdolled = true;
			ragdoll = true;
		}

		SetRB(rigidbody);

		
		cameraHandler.target = this.rb.transform;
		cameraHandler.mode = CameraMode.Locked;
		
		playerTransform.LookAt(rb.position - new Vector3(0f, rb.position.y, 0f), Vector3.up);
		
	}

	public void SetRB(Rigidbody rigidbody)
	{
		this.rb = rigidbody;
		rb.isKinematic = true;
	}

	void Start() // just for testing...
	{
	}

	// private bool firstUpdate = true;

	private bool targetReleased = false;
	private float timeSinceReleased = 0f;

	// Update is called once per frame
	
	bool releaseTarget = false;
	void Update()
	{
		if(Input.GetButtonDown("BasicAttack"))
		{
			releaseTarget = true;
		}



	}

	void FixedUpdate () 
	{
		/**
		if(firstUpdate)
		{
			BeginTKEffect(testInput);
			firstUpdate = false;
		} **/

		TranslateAlongVector(DetermineMovementVector());
		UpdatePlayerAnimator();

		if(releaseTarget)
		{
			ReleaseTarget();
			targetReleased = true;
			releaseTarget = false;
		}

		if(targetReleased)
		{
			timeSinceReleased += Time.deltaTime;

			if(timeSinceReleased >= Constants.TKReleasedTime)
			{
				
				playerAnimator.SetBool("TKMode", false);
				playerController.characterState = CharacterState.Idle;
				playerController.ClearActiveTarget();
				
				cameraHandler.mode = CameraMode.Free;
				
				// Get rid of this script
				Destroy (this);
			}
		}


	}


	void TranslateAlongVector(Vector3 v)
	{
		rb.MovePosition(rb.position + Constants.TKMoveSpeed*v*Time.deltaTime);
	}

	void UpdatePlayerAnimator()
	{
		float deltaH = DetermineHorizontalDisplacement();
		float deltaV = DetermineVerticalDisplacement();

		playerAnimator.SetFloat ("TK_horizontal", deltaH/Constants.TKHorizontalBound);
		playerAnimator.SetFloat("TK_vertical", deltaV/Constants.TKVerticalBound);

		if(deltaH >= Constants.TKHorizontalBound)
		{
			playerAnimator.transform.Rotate(Vector3.up, 30f*Time.deltaTime);
		}
		if(deltaH <= -1f*Constants.TKHorizontalBound)
		{
			playerAnimator.transform.Rotate(Vector3.up, -30f*Time.deltaTime);
		}
	}

	Vector3 DetermineMovementVector()
	{
		Vector3 v = Vector3.zero;
		
		float moveRight = -5f*Input.GetAxis("Horizontal") - 5f*Mathf.Clamp(Input.GetAxis ("Mouse X"), -1f, 1f); // tweak this (negative b/c Unity uses a left-handed coordinate system)
		float moveForward = 5f*Input.GetAxis("Vertical"); // and this
		float moveUp = 5f*Mathf.Clamp (Input.GetAxis("Mouse Y"),-1f,1f);
		
		float deltaH = DetermineHorizontalDisplacement();
		float deltaV = DetermineVerticalDisplacement();
		
		if(deltaH > Constants.TKHorizontalBound)
		{
			if(moveRight < 0f)
				moveRight = 0f;
		}
		if(deltaH < -1f*Constants.TKHorizontalBound)
		{
			if(moveRight > 0f)
				moveRight = 0f;
		}
		if(deltaV > Constants.TKVerticalBound)
		{
			if(moveUp > 0f)
				moveUp = 0f;
		}
		if(deltaV < -1f*Constants.TKVerticalBound)
		{
			if(moveUp < 0f)
				moveUp = 0f;
		}

		
		Vector3 forward = new Vector3(mainCamera.transform.forward.x, 0f, mainCamera.transform.forward.z);
		Vector3 up = new Vector3(0, 1f, 0f);
		Vector3 right = Vector3.Cross(forward, up);
		
		
		v = moveRight*right + moveUp*up + moveForward*forward;

		return v;

	}

	float DetermineHorizontalDisplacement()
	{
		Vector3 horizontalPos = (rb.position - playerTransform.position);
		horizontalPos = horizontalPos - new Vector3(0f, horizontalPos.y, 0f);

		return Vector3.Dot(playerTransform.right, horizontalPos.normalized);
	}

	float DetermineVerticalDisplacement()
	{
		Vector3 verticalPos = (rb.position - playerTransform.position - Vector3.up);

		return Vector3.Dot(playerTransform.up, verticalPos.normalized);
	}


	void ReleaseTarget()
	{
		Vector3 v = DetermineMovementVector();
		rb.isKinematic = false;
		if(ragdoll)
		{
			ragdollScript.ApplyForce(Constants.TKLaunchForce*v);
		}
		rb.AddForce(Constants.TKLaunchForce*v);
		// rb.velocity = Constants.TKLaunchForce*v;
	}
}
