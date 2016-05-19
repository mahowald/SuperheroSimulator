using UnityEngine;
using System.Collections;

public class HashIDs : MonoBehaviour {
	
	public int fMoveFwd;
	public int fSpeed;
	public int fDirection;
	public int fMoveRight;
	public int fSmoothDirection;


	public int fActivePower;
	public int fNormalizedTime;
	public int fInclination; // The inclination of the current slope. 
	public int fGender; // 0 = female, 1 = male
	public int fAttackInt; // random variable indicates attack animation
	public int fVerticalSpeed; 
	public int fAirtime;
	public int fHitDirection; // direction that we're hit from. 

	public int tDo180;
	public int tJump;
	public int tBasicAttack;
	public int tDoHit;
	public int tDoLanding;

	public int bWallCollide;
	public int bAirborne;

	public int bBlocking;

	public int bFire;
	public int bTKMode;
	public int bCombatStance;

	public int sIdle;
	public int sSlideToStop;
	public int sSlide180;
	public int sIdle180;
	public int sIdle90R;
	public int sIdle90L;
	public int sLocomotion;
	public int sStandardJump;
	public int sBT_BasicAttack1;
	public int sBT_BasicAttack2;
	public int sBT_BasicAttack3;
	public int sBT_Blocking;

	public int sCollideWithWall;

	public int iAttackCount;

	private bool debug = true;


	void Awake()
	{
		Setup ();
	}

	public void Setup()
	{
		fSpeed = Animator.StringToHash("Speed");
		fDirection = Animator.StringToHash ("Direction");
		fMoveFwd = Animator.StringToHash ("MoveFwd");
		fMoveRight = Animator.StringToHash("MoveRight");
		fSmoothDirection = Animator.StringToHash("SDirection");
		fActivePower = Animator.StringToHash ("ActivePower");
		fNormalizedTime = Animator.StringToHash ("NormalizedTime");
		fInclination = Animator.StringToHash ("Inclination");
		fGender = Animator.StringToHash ("Gender");
		fAttackInt = Animator.StringToHash("AttackInt");
		fVerticalSpeed = Animator.StringToHash ("VerticalSpeed");
		fAirtime = Animator.StringToHash("Airtime");
		fHitDirection = Animator.StringToHash("HitDirection");

		tDo180 = Animator.StringToHash ("Do180");
		tJump = Animator.StringToHash ("Jump");
		tBasicAttack = Animator.StringToHash ("BasicAttack");
		tDoHit = Animator.StringToHash("DoHit");
		tDoLanding = Animator.StringToHash("DoLanding");
		
		bWallCollide = Animator.StringToHash ("WallCollide");
		bAirborne = Animator.StringToHash ("Airborne");
		bFire = Animator.StringToHash("Fire");
		bTKMode = Animator.StringToHash("TKMode");
		bBlocking = Animator.StringToHash("Blocking");
		bCombatStance = Animator.StringToHash("CombatStance");
		
		sSlideToStop = Animator.StringToHash ("Base Layer.SM_Locomotion.BT_SlideToStop");
		sSlide180 = Animator.StringToHash ("Base Layer.SM_Locomotion.BT_Slide180");
		sIdle = Animator.StringToHash ("Base Layer.S_Idle");
		sIdle180 = Animator.StringToHash ("Base Layer.SIdle180");
		sIdle90R = Animator.StringToHash ("Base Layer.SIdle90R");
		sIdle90L = Animator.StringToHash ("Base Layer.SIdle90L");
		sLocomotion = Animator.StringToHash ("Base Layer.SM_Locomotion.BT_Locomotion");
		sStandardJump = Animator.StringToHash ("Base Layer.SM_Jump.BT_StandardJump");
		sCollideWithWall = Animator.StringToHash ("SM_Locomotion.CollideWithWall");
		sBT_BasicAttack1 = Animator.StringToHash("Base Layer.SM_BasicCombat.BT_BasicAttack1");
		sBT_BasicAttack2 = Animator.StringToHash("Base Layer.SM_BasicCombat.BT_BasicAttack2");
		sBT_BasicAttack3 = Animator.StringToHash("Base Layer.SM_BasicCombat.BT_BasicAttack3");
		sBT_Blocking = Animator.StringToHash("Base Layer.SM_BasicCombat.BT_Blocking");

		iAttackCount = Animator.StringToHash("AttackCount");
	}

}
