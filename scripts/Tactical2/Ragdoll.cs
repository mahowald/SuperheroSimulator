/**
 * Ragdoll script
 * with much credit to P. Hamalainen http://perttu.info
 * 
 * **/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ragdoll : MonoBehaviour {

	public Rigidbody root;
    Rigidbody host; // the rigidbody associated to the rootcollider
    public Collider rootCollider;
	public NavMeshAgent agent;
	public AIAgent agentAI;
	public Animator animator;
	public List<Rigidbody> ragdollRigidbodies;
	public List<Transform> omittedBones;

	public GenericCharacterController characterController;
    


	List<BodyPart> bodyParts = new List<BodyPart>();
	Vector3 ragdolledHipPosition,ragdolledHeadPosition,ragdolledFeetPosition;

	float ragdollingEndTime = -100f;

	enum RagdollState
	{
		animated,
		ragdolled,
		blendToAnim
	}

	RagdollState state = RagdollState.animated; // by default, start out animated

    private AnimatorUpdateMode startingmode;

	public class BodyPart
	{
		public Transform transform;
		public Vector3 storedPosition;
		public Quaternion storedRotation;
	}

	public bool Ragdolled
	{
		get
		{
			return state != RagdollState.animated;
		}
		set
		{
			if(value == true)
			{
				rootCollider.enabled = false;
				SetKinematic(false);
				animator.enabled = false;
				state = RagdollState.ragdolled;
                if(agent != null)
                {
                    agent.enabled = false;
                }
                if(agentAI != null)
                {
                    agentAI.enabled = false;
                }
				characterController.enabled = false;
			}
			else // returning from ragdolling:
			{
				if(state == RagdollState.ragdolled)
				{
					SetKinematic(true); // NOTE: May need to update this.
                    rootCollider.enabled = true;

                    ragdollingEndTime = Time.time; // store the state change time
					animator.enabled = true;
                    animator.updateMode = AnimatorUpdateMode.Normal; // This will help with the jitters if we animate on physics

					state = RagdollState.blendToAnim;

					foreach(BodyPart b in bodyParts)
					{
						b.storedPosition = b.transform.position;
						b.storedRotation = b.transform.rotation;
					}

					// Set the animator triggers
				}
			}
		}
	}

	void SetKinematic(bool newValue)
	{
		foreach(Rigidbody rb in ragdollRigidbodies)
		{
			rb.isKinematic = newValue;
            rb.detectCollisions = !newValue;
		}
        host.isKinematic = !newValue;
        host.detectCollisions = newValue;
	}


	void Start()
	{
        host = rootCollider.GetComponent<Rigidbody>();
		// characterController = this.GetComponent<GenericCharacterController>();
		SetKinematic(true);
        startingmode = animator.updateMode;

		// Transform[] components = GetComponentsInChildren<Transform>();
		// foreach(Transform c in components)
        foreach(Rigidbody rb in ragdollRigidbodies)
		{
            Transform c = rb.GetComponent<Transform>();
			if(!omittedBones.Contains(c))
			{
				BodyPart bodyPart = new BodyPart();
				bodyPart.transform = c as Transform;
				bodyParts.Add(bodyPart);
			}
		}
	}

	void Update()
	{
		if(Input.GetKeyDown (KeyCode.Keypad1))
		{
			Ragdolled = !Ragdolled;
		}
	}


	void LateUpdate() // this happens after the animator pass. 
	{
		if(state == RagdollState.blendToAnim)
		{
			//compute the ragdoll blend amount in the range 0...1
			float ragdollBlendAmount=1.0f-(Time.time-ragdollingEndTime)/Constants.RagdollBlendTime;
			ragdollBlendAmount=Mathf.Clamp01(ragdollBlendAmount);

			if(Time.time <= ragdollingEndTime)
			{
			}

			foreach(BodyPart b in bodyParts)
			{
				if(b.transform != animator.transform)
				{
					if(b.transform == animator.GetBoneTransform(HumanBodyBones.Hips)) // root
					{
						b.transform.position = Vector3.Lerp (b.transform.position, b.storedPosition, ragdollBlendAmount);
					}
					// everything else
					b.transform.rotation=Quaternion.Slerp(b.transform.rotation, b.storedRotation, ragdollBlendAmount);

				}
			}

			if (ragdollBlendAmount == 0)
			{
				state = RagdollState.animated;
                animator.updateMode = startingmode;
				// rootCollider.enabled = true;
                if (agent != null)
                {
                    agent.enabled = true;
                }
                if (agentAI != null)
                {
                    agentAI.enabled = true;
                }
                characterController.enabled = true;
				return;
			}
		}
	}

	public void ApplyForce(Vector3 f)
	{
		foreach(Rigidbody rb in ragdollRigidbodies)
		{
			rb.AddForce(f);
		}

		return;
	}
}
