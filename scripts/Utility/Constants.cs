using UnityEngine;

public static class Constants
{
	public const float CameraSpeed = 100f;
	public const float HeroRunTurnSpeed = 360f;
	public const float NPCTurnSpeed = 300f;
	public const float SlopeLimit = 45f;

	public const int PlayerLayer = 8;
	public const int EnemyLayer = 9;

	// player combat stuff
	public const float AttackDist = 3f; // distance we can attack over, in meters.
	public const float AttackAngle = 45f; // how far off-center can we be to turn to attack?
	public const float AttackMeleeDist = 1f; // offset for melee attacks.
	public const float MaxAttackMeleeDist = 5f; // how far away we can be before the attack doesn't hit. 

	public const float AttackTime = 2f; // how long do we wait before reseting the chamber?
	public const int AttackVariations = 3; // how many attack variations are there?


	// AI stuff
	public const float AIWalkSpeed = 1f;
	public const float AIRunSpeed = 3f;

	public const float FleeDistance = 10f; // How far to go before we stop running.
	public const float AIViewDistance = 20f; // How far away do the bad guys see us?
	public const float AIViewAngle = 80f; // What's the field of view for the bad guys?
	public const float AIHearDistance = 5f; // How far away can the AI hear us?


	public const float AIWaitTime = 10f; // How long should we be on alert after seeing the player?
	public const float AIHeartbeat = 3f; 
	// public const float GameControllerHeartbeat = 0.5f; // heartbeat in seconds

	// Ragdoll stuff

	public const float RagdollVelocityScale = 1f;
	public const float RagdollBlendTime = 1f;

	public const float TKMoveSpeed = 1f;
	public const float TKLaunchForce = 10f;
	public const float TKHorizontalBound = 0.71f; // = sin(pi/4)
	public const float TKVerticalBound = 0.71f;
	public const float TKReleasedTime = 1.5f;

	/* Tactical Objects */
	public static readonly Color enemyHighlightColor = new Color(1f, 0f, 0f);
	public static readonly Color neutralHighlightColor = new Color(0f, 0f, 1f);
	public static readonly Color allyHighlightColor = new Color(0f, 1f, 0f);
	public static readonly Color propHighlightColor = new Color(0f, 0f, 1f);


	/* Strategic */
	public const float CameraMoveTime = 1f; // How long to move the camera?

	public const int InfluenceChange = 1; // Each "increase control" increases control in the district by 1 pt. 

	/** in alphabetical order:
	 * Chinatown
	 * Docks
	 * Downtown
	 * Eastport
	 * Midtown
	 * North End
	 * Pleasantview
	 * Sunny Acres
	 * Uptown
	 * */

	public static int[,] districtAdjacency =  new int[,]{
		{1, 1, 0, 1, 1, 0, 0, 0, 0},
		{1, 1, 0, 0, 0, 0, 0, 0, 0},
		{0, 0, 1, 0, 1, 0, 0, 0, 0},
		{1, 0, 0, 1, 1, 0, 0, 0, 0},
		{1, 0, 1, 1, 1, 0, 1, 0, 1},
		{0, 0, 0, 0, 0, 1, 1, 0, 1},
		{0, 0, 0, 0, 1, 1, 1, 1, 1},
		{0, 0, 0, 0, 0, 0, 1, 1, 0},
		{0, 0, 0, 0, 1, 1, 1, 0, 1}}; // Which districts are adjacent to each other?

		
	

}
