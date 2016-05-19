using UnityEngine;
using System.Collections;

// This class just checks that the character is grounded.

[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(CapsuleCollider))]

public class GroundedHelper : MonoBehaviour {

    public GenericCharacterController characterController;

    public bool grounded = false;

    void OnCollisionStay()
    {
        grounded = true;
    }

    void OnCollisionExit()
    {
        grounded = false;
    }
}
