using UnityEngine;
using System.Collections;

public enum PlayerState {
	Idle,
	Run,
	Airborne,
	Combat,
	Dead,
	TargetedSpecial,
};

public class PlayerStateManager : MonoBehaviour {
	/** 
	public InputHandler inputHandler;
	public GenericCharacterController playerLocomotor;

	public PlayerState state;

	// Use this for initialization
	void Start () {
		state = PlayerState.Idle;
	}
	
	// Update is called once per frame
	void Update () {
	
		switch(state)
		{
		case PlayerState.Idle:
		default:
			playerLocomotor.speed = inputHandler.moveStr;
			playerLocomotor.direction = inputHandler.moveTilt;
			playerLocomotor.moveFwd = inputHandler.moveFwd;
			playerLocomotor.moveRight = inputHandler.moveRight;
			if(inputHandler.IsThereMovementInput())
			{
				playerLocomotor.targetHeading = inputHandler.moveDir;
				state = PlayerState.Run;
			}
			else
			{
				playerLocomotor.targetHeading = Vector3.zero;
				state = PlayerState.Idle;
			}

			// playerLocomotor.jump = inputHandler.jump;
			// playerLocomotor.basicAttack = inputHandler.basicAttack;

			break;
		}
	}
	**/
}
