using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction {
	public string name;

	public Color primaryColor = Color.white;

	public List<Agent> agents; // these are our troops.
	
	public bool evil; 

	private FactionAI factionAI;

	private int influenceModifier = 1;

	public Faction(string name, bool evil, List<Agent> agents)
	{
		this.name = name;
		this.evil = evil;
		this.agents = agents;

		factionAI = new FactionAI(this);
	}
	public Faction(string name, bool evil) : this(name, evil, new List<Agent>())
	{
	}

	public Faction() : this("unnamed faction", true)
	{
	}

	public int ControlledDistricts()
	{
		int count = 0;
		foreach(District d in GlobalData.strategicData.districts)
		{
			if(d.ControlledBy(this))
			{
				count++;
			}
		}
		return count;
	}

    public int AgentLimit()
    // how many agents are we allowed to recruit?
    {
        return ControlledDistricts() + 3;
    }

	public void AddAgent(Agent a)
	{
		this.agents.Add(a);
	}

    public void RemoveAgent(Agent a)
    {
        if(agents.Contains(a))
        {
            agents.Remove(a);
        }
    }

	public FactionAI FactionAI
	{
		get
		{
			return factionAI;
		}
	}

	public int InfluenceModifier
	{
		get
		{
			return influenceModifier;
		}
	}

	public bool HasAgentInDistrict(District d)
	{
		foreach(Agent a in agents)
		{
			if(a.location == d)
			{
				return true;
			}
		}
		return false;
	}

	public List<Agent> AgentsInDistrict(District d)
	{
		List<Agent> aind = new List<Agent>();
		foreach(Agent a in agents)
		{
			if(a.location == d)
			{
				aind.Add(a);
			}
		}

		return aind;
	}

	public bool HasEnemyInDistrict(District d)
	{
		foreach(Agent a in d.Agents)
		{
			if(a.employer != this)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAdjacentEnemy(District d)
	{
		foreach(District adj in d.GetAdjacentDistricts())
		{
			foreach(Agent a in adj.Agents)
			{
				if(a.employer != this)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void Serialize(System.Xml.XmlWriter writer)
	{
		writer.WriteStartElement ("Faction");
		writer.WriteElementString("Name", name);
		writer.WriteElementString("Evil", evil.ToString());
        writer.WriteElementString("PrimaryColor", (255f * primaryColor.r).ToString() + "," + (255f * primaryColor.g).ToString() + "," + (255f * primaryColor.b).ToString());
        writer.WriteElementString ("InfluenceModifier", influenceModifier.ToString());
		writer.WriteStartElement("Agents");
		// agents
        foreach(Agent a in this.agents)
        {
            writer.WriteStartElement("Agent");
            writer.WriteElementString("Location", a.location.name);
            writer.WriteEndElement();
        }
		writer.WriteEndElement();
		writer.WriteEndElement();
	}

    // we can't set the agent locations until after
    // we've already deserialized all the districts.
    // so instead, store that data, and come back to it.
    private Dictionary<Agent, string> temporaryAgentData;

	public void Deserialize(System.Xml.XmlReader reader)
	{
		reader.ReadToFollowing("Name");
		name = reader.ReadElementContentAsString ();
		reader.ReadToFollowing("Evil");
		evil = bool.Parse (reader.ReadElementContentAsString ());
        reader.ReadToFollowing("PrimaryColor");
        primaryColor = Appearance.ReadColorFromString(reader.ReadElementContentAsString());
		reader.ReadToFollowing("InfluenceModifier");
		influenceModifier = reader.ReadElementContentAsInt();

        reader.ReadToFollowing("Agents");
        temporaryAgentData = new Dictionary<Agent, string>();
        agents = new List<Agent>();
        System.Xml.XmlReader inner = reader.ReadSubtree();
        while(inner.ReadToFollowing("Agent"))
        {
            inner.ReadToFollowing("Location");
            string dname = inner.ReadElementContentAsString();
            // District d = GlobalData.lookupDictionaries.districtDict[inner.ReadElementContentAsString()];
            Agent a = new Agent(this, null, 1);
            this.AddAgent(a);
            temporaryAgentData.Add(a, dname);
        }
        inner.Close();

        reader.Close ();
	}

    public void DeserializeAgents()
    {
        foreach(Agent a in temporaryAgentData.Keys)
        {
            District location = GlobalData.lookupDictionaries.districtDict[temporaryAgentData[a]];
            a.location = location;
        }
    }

}
