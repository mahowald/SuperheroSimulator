using UnityEngine;
using System.Collections;

public class UIToggle : MonoBehaviour {

	private bool enabled;

	public GameObject target;

	void Start()
	{
		enabled = target.activeSelf;
	}

	public void Toggle()
	{
		if(enabled)
		{
			enabled = false;
			target.SetActive(false);
		}
		else
		{
			enabled = true;
			target.SetActive(true);
		}
	}

}
