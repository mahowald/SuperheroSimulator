using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataDeserializer 
{
	public static Dictionary<Condition, Dictionary<StrategicActionType, int>> factionAIModifiers;

	public static void Deserialize()
	{
		DeserializeFactionAIModifiers(@"Data\Strategic\aimodifiers.xml");
	}

	private static void DeserializeFactionAIModifiers(string path)
	{
		factionAIModifiers = new Dictionary<Condition, Dictionary<StrategicActionType, int>>();

		Dictionary<string, District> districtDict = GlobalData.lookupDictionaries.districtDict;
		Dictionary<string, Faction> factionDict = GlobalData.lookupDictionaries.factionDict;

		System.Xml.XmlReader reader = System.Xml.XmlReader.Create(path);

		reader.ReadToFollowing("Modifiers");

		System.Xml.XmlReader subreader = reader.ReadSubtree();

		while(subreader.ReadToFollowing("Modifier"))
		{
			subreader.ReadToFollowing("Condition");
			Condition c = Condition.ReadFromString (subreader.ReadElementContentAsString(), districtDict, factionDict, Condition.Context.FactionAI);


			subreader.ReadToFollowing("DoNothing");
			int nothingInt = subreader.ReadElementContentAsInt();
			subreader.ReadToFollowing("Fortify");
			int fortifyInt = subreader.ReadElementContentAsInt ();
			subreader.ReadToFollowing ("IncreaseInfluence");
			int influenceInt = subreader.ReadElementContentAsInt ();
			subreader.ReadToFollowing ("MoveAgents");
			int moveagentsInt = subreader.ReadElementContentAsInt ();
			subreader.ReadToFollowing("Recruit");
			int recruitInt = subreader.ReadElementContentAsInt ();

			Dictionary<StrategicActionType, int> d = new Dictionary<StrategicActionType, int>()
			{
				{StrategicActionType.DoNothing, nothingInt},
				// {StrategicActionType.Fortify, fortifyInt},
				// {StrategicActionType.IncreaseInfluence, influenceInt},
				{StrategicActionType.MoveAgents, moveagentsInt},
				{StrategicActionType.Recruit, recruitInt}
			};

			factionAIModifiers.Add(c, d);

		}

		reader.Close ();

		Debug.Log("Faction AI Modifiers read: " + factionAIModifiers.Count.ToString());
	}
}
