using UnityEngine;
using System.Collections;

public class DebuggingScript : MonoBehaviour {

    public EventPanelUI testui;

	// Use this for initialization
	void Start () {
        // Testing events
        /** 
        StrategicEvent sevent = new StrategicEvent();
        sevent.uidata.text = "This is a test event.";
        sevent.uidata.image = Resources.Load<Sprite>("textures/ui/banner-images/demo-image");
        string path = @"Data\Strategic\Events\event.xml";
        System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
        settings.Indent = true;
        System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(path, settings);
        sevent.Serialize(writer);
        writer.Flush();
        writer.Close();
        **/

        string path = @"Data\Strategic\Events\tutorial-1.xml";
        StrategicEvent sevent2 = new StrategicEvent();
        System.Xml.XmlReader reader = System.Xml.XmlReader.Create(path);
        sevent2.Deserialize(reader, GlobalData.lookupDictionaries.districtDict, GlobalData.lookupDictionaries.factionDict);

        testui.sevent = sevent2;
        testui.SetupPanel();

        if(GlobalData.crossoverData.showTutorials == false)
        {
            testui.gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
