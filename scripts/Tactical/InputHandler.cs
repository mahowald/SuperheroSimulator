using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

	public Transform activeCamera;
	public GameObject player;
	public PlayerCharacterController pcController;

	private float moveInX;
	private float moveInY;
	private float moveInXRaw;
	private float moveInYRaw;

	private bool jumpDown;
	private bool jumpUp;

	private bool basicAttack;
	private bool block;
	private bool targetMode;

	private Vector3 moveDir; // what direction do we want to move?
	private Vector3 rawMoveDir; // The raw movement direction... Don't use this.
	private float moveStr; // How fast do we want to move there?
	private float moveTilt; // How much are we turning? dot product of desired heading with player's right. Used to determine lean.
	private float moveFwd; // dot product of desired heading with player's actual heading. Used to determine when to do a 180.
	private float moveRight; // dot product of desired heading with player's actual heading. Used to determine when to do a 90-degree standing turn.

	public bool paused = false;

	private CameraHandler cameraHandler;


	// Use this for initialization
	void Start () {
		cameraHandler = activeCamera.GetComponent<CameraHandler>();
	}

	void Awake()
	{
		moveDir = player.transform.forward;
		rawMoveDir = player.transform.forward;
	}
	
	// Update is called once per frame
	void Update () {
		if(paused == false)
		{
			moveInX = Input.GetAxis("Horizontal");
			moveInY = Input.GetAxis("Vertical");
			moveInXRaw = Input.GetAxisRaw("Horizontal");
			moveInYRaw = Input.GetAxisRaw("Vertical");


			pcController.speed = Mathf.Clamp (Mathf.Sqrt (moveInX*moveInX + moveInY*moveInY), 0, 1f);

			moveDir = ComputeMoveDir();
			rawMoveDir = ComputeRawMoveDir();

			/** 
			if(IsThereMovementInput())
			{
				pcController.targetHeading = moveDir;
			}
			else
			{
				pcController.targetHeading = Vector3.zero;
			}
			**/

			pcController.direction = ComputeMoveTilt ();

			if(pcController.lockHeading)
			{
				pcController.direction = 0f;
			}

			pcController.moveFwd = ComputeMoveFwd ();
			pcController.moveRight = ComputeMoveRight ();


			jumpDown = Input.GetButtonDown ("Jump");
			jumpUp = Input.GetButtonUp("Jump");

			basicAttack = Input.GetButtonDown ("BasicAttack");

			if(jumpDown)
				pcController.ChargeJump();

			if(jumpUp)
				pcController.LaunchJump();

			if(basicAttack)
				pcController.DoBasicAttack();

			block = Input.GetButton("Block");

			pcController.block = block;

			targetMode = Input.GetButtonDown ("Target Mode");

			if(targetMode)
			{
				// toggle target mode:
				// pcController.CombatStance = !pcController.CombatStance;

				if(pcController.targetMode == PlayerCharacterController.TargetMode.Locked) // we're currently locked on to target
				{
					// free ourselves
					pcController.targetMode = PlayerCharacterController.TargetMode.Free;
					cameraHandler.mode = CameraMode.Free;

					pcController.lockHeading = false;
				}
				else // we're not locked on target, so put ourselves in target mode
				{
					pcController.lockHeading = true;

					if(cameraHandler.highlightedObject != null)
					{
						pcController.ActiveTarget = cameraHandler.highlightedObject.transform;
						cameraHandler.mode = CameraMode.Target;
						cameraHandler.target = pcController.ActiveTarget;
					}
					else
					{
						pcController.ActiveTarget = pcController.GetNearestEnemy();
						if(pcController.ActiveTarget != null)
						{
							TacticalObject tac = (pcController.ActiveTarget).GetComponent<TacticalObject>();
							tac.Highlighted = true;
							cameraHandler.highlightedObject = tac;
							cameraHandler.mode = CameraMode.Target;
							cameraHandler.target = pcController.ActiveTarget;
						}
					}
				}

			}

		}

	}

	public bool IsThereMovementInput()
	{
		if(Input.GetAxisRaw ("Horizontal") != 0)
		{
			return true;
		}
		if(Input.GetAxisRaw ("Vertical") != 0)
		{
			return true;
		}

		return false;
	}

	float ComputeMoveTilt()
	{
		if(rawMoveDir == Vector3.zero)
			return 0f;

		float right = Vector3.Dot (player.transform.right, rawMoveDir);

		float angle = Vector3.Angle(player.transform.forward, rawMoveDir);

		if(right < 0)
		{
			angle = -1f*angle;
		}

		float tilt = angle/90f; // +/- 2 is 180 degrees. 

		return tilt;
	}

	float ComputeMoveFwd()
	{
		return Vector3.Dot(player.transform.forward, moveDir);
	}
	
	float ComputeMoveRight()
	{
		return Vector3.Dot(player.transform.right, moveDir);
	}

	Vector3 ComputeMoveDir()
	{
		Vector3 cameraForward = new Vector3(activeCamera.transform.forward.x, 0, activeCamera.transform.forward.z).normalized;
		Vector3 cameraRight = new Vector3(activeCamera.transform.right.x, 0, activeCamera.transform.right.z).normalized;
		return moveInX*cameraRight + moveInY*cameraForward;
	}

	Vector3 ComputeRawMoveDir()
	{
		Vector3 cameraForward = new Vector3(activeCamera.transform.forward.x, 0, activeCamera.transform.forward.z).normalized;
		Vector3 cameraRight = new Vector3(activeCamera.transform.right.x, 0, activeCamera.transform.right.z).normalized;
		return moveInXRaw*cameraRight + moveInYRaw*cameraForward;
	}

}
