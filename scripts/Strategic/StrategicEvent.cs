using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StrategicEvent {

    public struct UIData
    {
        public Sprite image; // the banner image
        public string text; // text that appears in the box

        public void Serialize(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("UIData");
            writer.WriteElementString("Image", image.name);
            string textstr = text.Replace("<b>", "[b]");
            textstr = textstr.Replace("</b>", "[/b]");

            writer.WriteElementString("Text", textstr);
            writer.WriteEndElement();
        }

        public void Deserialize(System.Xml.XmlReader reader)
        {
            reader.ReadToFollowing("UIData");
            reader.ReadToFollowing("Image");
            string imagename = reader.ReadElementContentAsString();
            image = Resources.Load<Sprite>("textures/ui/banner-images/" + imagename);
            reader.ReadToFollowing("Text");
            string textstr = reader.ReadElementContentAsString();

            textstr = textstr.Replace("[b]", "<b>");
            textstr = textstr.Replace("[/b]", "</b>");
            text = textstr;
            reader.Close();
        }

    }

    public UIData uidata;

	public string eventName;

	public int meanTurnsToHappen; // # of turns until P(event hasn't happened) < 0.5 

	private float probability; // P = 1 - 0.5^(1/n) where n = meanTurnsToHappen

	public List<Condition> requirements; // these must be met before the event fires

	public Dictionary<Condition, float> modifiers; // modifies mean time to happen

	public bool onlyHappensOnce = false; // If the event should only happen once.

	public Dictionary<string, List<Effect>> choices; // The string is the name of the choice, and then the list of effects is what happens when you pick that.


	public StrategicEvent()
	{
		eventName = "I have no name.";
		meanTurnsToHappen = 0;

		requirements = new List<Condition>();
		modifiers = new Dictionary<Condition, float>(); // modify mean time to happen
		choices = new Dictionary<string, List<Effect>>();
	}

	public float Probability
	{
		get
		{
			float p = 0;
			if(!this.MeetsRequirements())
			{
				return p;
			}
			float mtth = 1f*meanTurnsToHappen;
			if(modifiers.Count > 0)
			{
				foreach(Condition c in modifiers.Keys)
				{
					if(c.IsTrue())
					{
						mtth += modifiers[c];
					}
				}
			}

			p = (float) (1f - System.Math.Pow(0.5, 1d/mtth));
			return p;
		}

	}

	private bool MeetsRequirements()
	{
		if(requirements.Count == 0)
			return true;

		foreach(Condition c in requirements)
		{
			if(!c.IsTrue())
			{
				return false;
			}
		}

		return true;
	}

	public void Serialize(System.Xml.XmlWriter writer)
	{
		
		writer.WriteStartElement ("StrategicEvent");
		writer.WriteElementString("EventName", eventName);
		writer.WriteElementString ("MeanTurnsToHappen", meanTurnsToHappen.ToString());
		writer.WriteElementString ("OnlyHappensOnce", onlyHappensOnce.ToString());
		writer.WriteStartElement("Requirements");
		foreach(Condition c in requirements)
		{
			c.Serialize(writer);
		}
		writer.WriteEndElement();

		writer.WriteStartElement("Modifiers");
		foreach(Condition c in modifiers.Keys)
		{
			writer.WriteStartElement ("Modifier");
			writer.WriteAttributeString("condition", c.ToString());
			writer.WriteAttributeString("value", modifiers[c].ToString());
			writer.WriteEndElement();
		}
		writer.WriteEndElement();

		writer.WriteStartElement ("Choices");
		foreach(string s in choices.Keys)
		{
			writer.WriteStartElement ("Choice");
			writer.WriteAttributeString ("name", s);
			writer.WriteStartElement ("Effects");
			foreach(Effect e in choices[s])
			{
				e.Serialize(writer);
			}
			writer.WriteEndElement ();
			writer.WriteEndElement();
		}
        
		writer.WriteEndElement();

        uidata.Serialize(writer);
    }

	public void Deserialize(System.Xml.XmlReader reader, Dictionary<string, District> districtDict, Dictionary<string, Faction> factionDict)
	{
		reader.ReadToFollowing("EventName");
		eventName = reader.ReadElementContentAsString ();
		reader.ReadToFollowing("MeanTurnsToHappen");
		meanTurnsToHappen = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing ("OnlyHappensOnce");
		onlyHappensOnce = bool.Parse (reader.ReadElementContentAsString ());

		reader.ReadToFollowing("Requirements");
		System.Xml.XmlReader inner = reader.ReadSubtree();
		requirements = new List<Condition>();
		while(inner.ReadToFollowing("Condition"))
		{
			Condition c = Condition.ReadFromString(inner.ReadElementContentAsString (), districtDict, factionDict);
			requirements.Add (c);
		}
		inner.Close();

		modifiers = new Dictionary<Condition, float>();
		reader.ReadToFollowing("Modifiers");
		inner = reader.ReadSubtree();
		while(inner.ReadToFollowing("Modifier"))
		{
			Condition c = Condition.ReadFromString(inner.GetAttribute("condition"), districtDict, factionDict);
			float val = float.Parse(inner.GetAttribute("value"));
			modifiers.Add(c, val);
		}
		inner.Close ();

		choices = new Dictionary<string, List<Effect>>();
		reader.ReadToFollowing("Choices");
		inner = reader.ReadSubtree();
		while(inner.ReadToFollowing("Choice"))
		{
			string choicename = inner.GetAttribute("name");
			inner.ReadToFollowing("Effects");
			List<Effect> effectlist = new List<Effect>();
			System.Xml.XmlReader subinner = inner.ReadSubtree();
			while(subinner.ReadToFollowing ("Effect"))
			{
				effectlist.Add(Effect.Deserialize(subinner, districtDict, factionDict));
			}
			choices.Add(choicename, effectlist);
			inner.Close ();
		}

        reader.ReadToFollowing("UIData");
        inner = reader.ReadSubtree();
        uidata.Deserialize(inner);
        
		reader.Close ();

	}


}
