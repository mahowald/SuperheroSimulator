using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is very similar to the ragdoll script, but with some alterations b/c of the
// slightly different nature of the player character. 

public class PlayerRagdoll : MonoBehaviour {

    public GenericCharacterController characterController;
    public List<Rigidbody> ragdollRigidbodies;
    public Rigidbody playerRigidbody;
    List<BodyPart> bodyParts = new List<BodyPart>();
    public List<Transform> omittedBones;

    private Collider rootCollider;
    private Animator animator;

    float ragdollingEndTime = -100f;

    enum RagdollState
    {
        animated,
        ragdolled,
        blendToAnim
    }
    
    RagdollState state = RagdollState.animated; // by default, start out animated

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
            if (value == true)
            {
                rootCollider.enabled = false;
                SetKinematic(false);
                animator.enabled = false;
                state = RagdollState.ragdolled;
                characterController.enabled = false;
            }
            else // returning from ragdolling:
            {
                if (state == RagdollState.ragdolled)
                {
                    // rootCollider.enabled = true;
                    // SetKinematic(true); // NOTE: May need to update this.
                    ragdollingEndTime = Time.time; // store the state change time
                    animator.enabled = true;

                    state = RagdollState.blendToAnim;

                    foreach (BodyPart b in bodyParts)
                    {
                        b.storedPosition = b.transform.position;
                        b.storedRotation = b.transform.rotation;
                    }

                    // Set the animator triggers
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
        rootCollider = playerRigidbody.GetComponent<Collider>();
        animator = characterController.animator;
        SetKinematic(true);

        Transform[] components = animator.GetComponentsInChildren<Transform>();
        foreach (Transform c in components)
        {
            if (!omittedBones.Contains(c))
            {
                BodyPart bodyPart = new BodyPart();
                bodyPart.transform = c as Transform;
                bodyParts.Add(bodyPart);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Ragdolled = !Ragdolled;
        }

    }

    void SetKinematic(bool newValue)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = newValue;
            rb.detectCollisions = !newValue;
        }
        playerRigidbody.isKinematic = !newValue;
        playerRigidbody.detectCollisions = newValue;
    }

    void LateUpdate() // this happens after the animator pass. 
    {
        if (state == RagdollState.blendToAnim)
        {
            //compute the ragdoll blend amount in the range 0...1
            float ragdollBlendAmount = 1.0f - (Time.time - ragdollingEndTime) / Constants.RagdollBlendTime;
            ragdollBlendAmount = Mathf.Clamp01(ragdollBlendAmount);

            if (Time.time <= ragdollingEndTime)
            {
            }

            foreach (BodyPart b in bodyParts)
            {
                if (b.transform != animator.transform)
                {
                    if (b.transform == animator.GetBoneTransform(HumanBodyBones.Hips)) // root
                    {
                        b.transform.position = Vector3.Lerp(b.transform.position, b.storedPosition, ragdollBlendAmount);
                    }
                    // everything else
                    b.transform.rotation = Quaternion.Slerp(b.transform.rotation, b.storedRotation, ragdollBlendAmount);

                }
            }

            if (ragdollBlendAmount == 0)
            {
                state = RagdollState.animated;
                rootCollider.enabled = true;
                SetKinematic(true);
                characterController.enabled = true;
                return;
            }
        }
    }
}
