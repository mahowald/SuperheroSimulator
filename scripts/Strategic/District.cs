using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class District {

	public Dictionary<Faction, int> ownership;

	public string name;
	public int happiness;
	public int index;

	public District(string title, int ind)
	{
		name = title;
		ownership = new Dictionary<Faction, int>();
		happiness = 0;
		index = ind;
	}

	public District() : this("Unnamed district", -1) {}

	public bool ControlledBy(Faction f)
	{
		if(ownership.ContainsKey(f))
		{
			int factionWeight = ownership[f];
			return 2*factionWeight >= this.TotalWeight(); // the owning faction controls at least 50% of the district. 
		}

		return false;
	}

    public Faction Controller
    {
        get
        {
            Faction owner = null;
            int total = 0;

            foreach(Faction f in ownership.Keys)
            {
                if(ownership[f] >= total)
                {
                    owner = f;
                    total = ownership[f];
                }
            }

            return owner;
        }
    }

	public bool ControlledByCriminals()
	{
		int evilWeight = 0;
		foreach(Faction f in ownership.Keys)
		{
			if(f.evil)
			{
				evilWeight += ownership[f];
			}
		}

		return 2*evilWeight >= this.TotalWeight();
	}

	public float CriminalsControl()
	{
		float totalWeight = (float) this.TotalWeight();
		if(totalWeight == 0)
		{
			return 0f;
		}
		int evilWeight = 0;
		foreach(Faction f in ownership.Keys)
		{
			if(f.evil)
			{
				evilWeight += ownership[f];
			}
		}
		return ((float)evilWeight)/(totalWeight);
	}

	private int TotalWeight()
	{
		int weight = 0;
		foreach(Faction f in ownership.Keys)
		{
			weight += ownership[f];
		}

		return weight;
	}

	
	
	public List<Agent> Agents
	{
		get
		{
			List<Agent> agents = new List<Agent>();

			foreach(Faction f in GlobalData.strategicData.factions)
			{
				foreach(Agent a in f.agents)
				{
					if(a.location.name == this.name) 
                        // need to compare by names, which are hopefully unique.
                        // otherwise, weird stuff happens on game load/reload. 
					{
						agents.Add(a);
					}
                }
            }

			return agents;
		}
	}

	public Dictionary<Faction, List<Agent>> GetOccupants()
	{
        
		List<Agent> agents = this.Agents;

		Dictionary<Faction, List<Agent>> conflict = new Dictionary<Faction, List<Agent>>();
        
		foreach(Agent a in agents)
		{
			Faction f = a.employer;
			if(conflict.ContainsKey(f))
			{
				conflict[f].Add(a);
			}
			else
			{
				conflict.Add(f, new List<Agent>());
				conflict[f].Add(a);
			}
		}
        

		return conflict;
	}

	public void Serialize(System.Xml.XmlWriter writer)
	{
		writer.WriteStartElement ("District");
		writer.WriteElementString("Name", this.name);
		writer.WriteElementString("Index", this.index.ToString());
		writer.WriteElementString("Happiness", this.happiness.ToString());
		writer.WriteStartElement ("Factions");
		foreach(Faction f in ownership.Keys)
		{
			writer.WriteStartElement ("Item");
			writer.WriteElementString("FactionName", f.name);
			writer.WriteElementString("Influence", ownership[f].ToString());
			writer.WriteEndElement();
		}
		writer.WriteEndElement();

		writer.WriteEndElement ();
	}

	public void Deserialize(System.Xml.XmlReader reader, Dictionary<string, Faction> factionDict)
	{
		reader.ReadToFollowing("Name");
		name = reader.ReadElementContentAsString();
		reader.ReadToFollowing("Index");
		index = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing("Happiness");
		reader.ReadElementContentAsInt ();
		reader.ReadToFollowing ("Factions");

		ownership = new Dictionary<Faction, int>();
		System.Xml.XmlReader inner = reader.ReadSubtree();
		while(inner.ReadToFollowing("Item"))
		{
			inner.ReadToFollowing("FactionName");
			Faction f = factionDict[reader.ReadElementContentAsString()];
			inner.ReadToFollowing("Influence");
			int influence = reader.ReadElementContentAsInt ();
			ownership.Add(f, influence);
		}
		inner.Close ();
		reader.Close ();
	}

	public List<District> GetAdjacentDistricts()
	{
		int ind = this.index;
		List<District> adjacents = new List<District>();
		for(int i = 0; i < 9; i++)
		{
			if(Constants.districtAdjacency[ind,i] == 1)
			{
				adjacents.Add(GlobalData.strategicData.districts[i]);
			}
		}

		return adjacents;
	}

}
