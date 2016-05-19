using UnityEngine;
using System.Collections;

// The purpose of this class is to help with jumping behavior. 

public class CharacterJumpHelper : MonoBehaviour {

    private bool grounded = true;

    public bool Grounded
    {
        get
        {
            return grounded;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Landed!");
        if(collision.rigidbody == null)
        {
            grounded = true;
        }
    }

    void OnCollisionExit()
    {
        Debug.Log("Liftoff!");
        grounded = false;
    }

}
