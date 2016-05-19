using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventPanelUI : MonoBehaviour {

    public StrategicEvent sevent;
    public Text text;
    public Image banner;

    public void SetupPanel()
    {
        banner.sprite = sevent.uidata.image;
        text.text = sevent.uidata.text;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DoButton()
    {
        this.gameObject.SetActive(false);
    }
}
