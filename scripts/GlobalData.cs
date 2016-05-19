using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Appearance // Player (or other character) appearance
{
	public struct Shape
	{
		public float nose_up;
		public float nose_down;
		public float nose_wide;
		public float nose_out;
		public float mouth_wide;
		public float mouth_big;
		public float cheekbones_out;
		public float eyes_large;
		public float eyes_back;
		public float eyes_up;
		public float eyes_down;
		public float cheeks_wide;
		public float cheeks_narrow;

		public void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Shape");
			writer.WriteElementString("nose_up", nose_up.ToString());
			writer.WriteElementString("nose_down", nose_down.ToString());
			writer.WriteElementString("nose_wide", nose_wide.ToString());
			writer.WriteElementString("nose_out", nose_out.ToString());
			writer.WriteElementString("mouth_wide", mouth_wide.ToString());
			writer.WriteElementString("mouth_big", mouth_big.ToString());
			writer.WriteElementString("cheekbones_out", cheekbones_out.ToString());
			writer.WriteElementString("eyes_large", eyes_large.ToString());
			writer.WriteElementString("eyes_back", eyes_back.ToString());
			writer.WriteElementString("eyes_up", eyes_up.ToString());
			writer.WriteElementString("eyes_down", eyes_down.ToString());
			writer.WriteElementString("cheeks_wide", cheeks_wide.ToString());
			writer.WriteElementString("cheeks_narrow", cheeks_narrow.ToString());
			writer.WriteEndElement();
		}

		public void Deserialize(System.Xml.XmlReader reader)
		{
			reader.ReadToFollowing("nose_up");
			nose_up = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("nose_down");
			nose_down = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("nose_wide");
			nose_wide = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("nose_out");
			nose_out = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("mouth_wide");
			mouth_wide = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("mouth_big");
			mouth_big = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("cheekbones_out");
			cheekbones_out = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("eyes_large");
			eyes_large = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("eyes_back");
			eyes_back = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("eyes_up");
			eyes_up = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("eyes_down");
			eyes_down = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("cheeks_wide");
			cheeks_wide = reader.ReadElementContentAsFloat();
			reader.ReadToFollowing("cheeks_narrow");
			cheeks_narrow = reader.ReadElementContentAsFloat();

			reader.Close();
		}
	}

	public bool isFemale; // Are we female?
	public Color primaryColor; // Primary costume color
	public Color secondaryColor; // Secondary costume color
	public Color tertiaryColor; // Tertiary costume color
	public Color accessoriesColor; // accessory color
	public Color emblemColor; // emblem color
	public Color capeColor; // cape color
	public Color hairTint; // Hair color
	public Color skinTint; // Skin color

    public int emblemIndex; // the index for the emblem.
    public bool hasCape;

	public Shape shape; 

	public void Serialize(System.Xml.XmlWriter writer)
	{
		writer.WriteStartElement ("Appearance");
		writer.WriteElementString ("isFemale", isFemale.ToString());
		writer.WriteElementString ("primaryColor", (255f*primaryColor.r).ToString() + "," + (255f*primaryColor.g).ToString() + "," + (255f*primaryColor.b).ToString());
		writer.WriteElementString ("secondaryColor", (255f*secondaryColor.r).ToString() + "," + (255f*secondaryColor.g).ToString() + "," + (255f*secondaryColor.b).ToString());
		writer.WriteElementString ("tertiaryColor", (255f*tertiaryColor.r).ToString() + "," + (255f*tertiaryColor.g).ToString() + "," + (255f*tertiaryColor.b).ToString());
		writer.WriteElementString ("hairTint", (255f*hairTint.r).ToString() + "," + (255f*hairTint.g).ToString() + "," + (255f*hairTint.b).ToString());
		writer.WriteElementString ("skinTint", (255f*skinTint.r).ToString() + "," + (255f*skinTint.g).ToString() + "," + (255f*skinTint.b).ToString());
		writer.WriteElementString ("emblemColor", (255f*emblemColor.r).ToString() + "," + (255f*emblemColor.g).ToString() + "," + (255f*emblemColor.b).ToString());
		writer.WriteElementString ("accessoriesColor", (255f*accessoriesColor.r).ToString() + "," + (255f*accessoriesColor.g).ToString() + "," + (255f*accessoriesColor.b).ToString());
		writer.WriteElementString ("capeColor", (255f*capeColor.r).ToString() + "," + (255f*capeColor.g).ToString() + "," + (255f*capeColor.b).ToString());
		shape.Serialize(writer);
        writer.WriteElementString("emblemIndex", emblemIndex.ToString());
        writer.WriteElementString("hasCape", hasCape.ToString());
		writer.WriteEndElement ();
	}

	public void Deserialize(System.Xml.XmlReader reader)
	{
		reader.ReadToFollowing("isFemale");
		isFemale = bool.Parse (reader.ReadElementContentAsString()); // For some reason, reading as Boolean doesn't work. 
		reader.ReadToFollowing ("primaryColor");
		primaryColor = ReadColorFromString(reader.ReadElementContentAsString());
		reader.ReadToFollowing("secondaryColor");
		secondaryColor = ReadColorFromString(reader.ReadElementContentAsString());
		reader.ReadToFollowing ("tertiaryColor");
		tertiaryColor = ReadColorFromString(reader.ReadElementContentAsString());
		reader.ReadToFollowing("hairTint");
		hairTint = ReadColorFromString(reader.ReadElementContentAsString());
		reader.ReadToFollowing ("skinTint");
		skinTint = ReadColorFromString(reader.ReadElementContentAsString());
		reader.ReadToFollowing("emblemColor");
		emblemColor = ReadColorFromString(reader.ReadElementContentAsString());
		reader.ReadToFollowing("accessoriesColor");
		accessoriesColor = ReadColorFromString(reader.ReadElementContentAsString());
		reader.ReadToFollowing("capeColor");
		capeColor = ReadColorFromString(reader.ReadElementContentAsString());

		reader.ReadToFollowing ("Shape");
		System.Xml.XmlReader inner = reader.ReadSubtree();
		shape.Deserialize(inner);

        reader.ReadToFollowing("emblemIndex");
        emblemIndex = reader.ReadElementContentAsInt();
        reader.ReadToFollowing("hasCape");
        hasCape = bool.Parse(reader.ReadElementContentAsString());

		reader.Close ();

	}

	public static Color ReadColorFromString(string s)
	{
		char[] delims = {' ', ',', ':'};

		string[] parts = s.Split (delims);

		float r = float.Parse (parts[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		float g = float.Parse (parts[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		float b = float.Parse (parts[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

		return new Color(r/255f, g/255f, b/255f);
	}

}

public struct Attributes
{
	public int constitution;
	public int stamina;
	public int intelligence;
	public int charisma;

	public void Serialize(System.Xml.XmlWriter writer)
	{
		writer.WriteStartElement ("Attributes");
		writer.WriteElementString ("constitution", constitution.ToString());
		writer.WriteElementString ("stamina", stamina.ToString());
		writer.WriteElementString ("intelligence", intelligence.ToString());
		writer.WriteElementString ("charisma", charisma.ToString());
		writer.WriteEndElement();
	}

	public void Deserialize(System.Xml.XmlReader reader)
	{
		reader.ReadToFollowing("constitution");
		constitution = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing("stamina");
		stamina = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing("intelligence");
		intelligence = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing ("charisma");
		charisma = reader.ReadElementContentAsInt();

		reader.Close ();
	}
}

public struct Stats
{
	public int health;
	public int energy;
	public int fuel;

	public int money;
	public int reputation;
	public int research;
	
	public void Serialize(System.Xml.XmlWriter writer)
	{
		writer.WriteStartElement("Stats");
		writer.WriteElementString ("health", health.ToString());
		writer.WriteElementString ("energy", energy.ToString());
		writer.WriteElementString ("fuel", fuel.ToString());
		writer.WriteElementString ("money", money.ToString());
		writer.WriteElementString ("reputation", reputation.ToString());
		writer.WriteElementString ("research", research.ToString());
		writer.WriteEndElement();
	}
	
	public void Deserialize(System.Xml.XmlReader reader)
	{
		reader.ReadToFollowing ("health");
		health = reader.ReadElementContentAsInt();
		reader.ReadToFollowing ("energy");
		energy = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing ("fuel");
		fuel = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing ("money");
		money = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing ("reputation");
		reputation = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing ("research");
		research = reader.ReadElementContentAsInt ();

		reader.Close ();
	}
}

public enum SuperPower
{
	Telekinesis,
	Strength,
	Flight,
	Fire,
	Ice,
	Speed,
	Invulnerability,
    Telepathy,
};

public struct Gear
{
	public int head;
	public int body;
	public int gloves;
	public int boots;
	public int weapon;

	public void Serialize(System.Xml.XmlWriter writer)
	{
		writer.WriteStartElement ("Gear");
		writer.WriteElementString ("head", head.ToString());
		writer.WriteElementString("body", body.ToString());
		writer.WriteElementString("gloves", gloves.ToString());
		writer.WriteElementString("boots", boots.ToString());
		writer.WriteElementString("weapon", weapon.ToString());
		writer.WriteEndElement();
	}
	
	public void Deserialize(System.Xml.XmlReader reader)
	{
		reader.ReadToFollowing ("head");
		head = reader.ReadElementContentAsInt();
		reader.ReadToFollowing ("body");
		body = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing("gloves");
		gloves = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing("boots");
		boots = reader.ReadElementContentAsInt ();
		reader.ReadToFollowing ("weapon");
		weapon = reader.ReadElementContentAsInt ();

		reader.Close ();
	}
}

public struct PlayerData
{
	public Attributes attributes;
	public Stats stats;
	public Gear gear;

	public string name; // name
	public Appearance appearance; // Physical data
	public HashSet<SuperPower> powers; // what powers we have
	
	public bool HasPower(SuperPower pow)
	{
		return(powers.Contains(pow));
	}

	public void Serialize(System.Xml.XmlWriter writer)
	{
		writer.WriteStartElement("PlayerData");
		writer.WriteElementString ("Name", name);
		attributes.Serialize(writer);
		stats.Serialize(writer);
		appearance.Serialize(writer);

		writer.WriteStartElement ("Powers");
		foreach(SuperPower power in powers)
		{
			writer.WriteElementString ("Power", power.ToString());
		}
		writer.WriteEndElement();
		gear.Serialize(writer);

		writer.WriteEndElement();
	}

	public void Deserialize(System.Xml.XmlReader reader)
	{
		reader.ReadToFollowing ("Name");
		name = reader.ReadElementContentAsString();

		reader.ReadToFollowing ("Attributes");
		System.Xml.XmlReader inner = reader.ReadSubtree();
		attributes.Deserialize(inner);

		reader.ReadToFollowing ("Stats");
		inner = reader.ReadSubtree();
		stats.Deserialize(inner);

		reader.ReadToFollowing ("Appearance");
		inner = reader.ReadSubtree();
		appearance.Deserialize(inner);

		reader.ReadToFollowing ("Powers");
		inner = reader.ReadSubtree();
		powers.Clear ();
		while(inner.ReadToFollowing ("Power"))
		{
			string powstr = inner.ReadElementContentAsString ();
			powers.Add((SuperPower) System.Enum.Parse(typeof(SuperPower), powstr));

		}

		reader.ReadToFollowing("Gear");
		inner = reader.ReadSubtree();
		gear.Deserialize(inner);

		reader.Close();
	}

}

public struct StrategicData
{
	public int turn;
	public List<Faction> factions;
	public List<District> districts;
	public List<int> flags;

	public void Serialize(System.Xml.XmlWriter writer)
	{
		writer.WriteStartElement ("StrategicData");

		writer.WriteElementString("Turn", turn.ToString());
		writer.WriteStartElement ("Flags");

		foreach(int flag in flags)
		{
			writer.WriteElementString("Flag", flag.ToString());
		}
		writer.WriteEndElement ();

		writer.WriteStartElement ("Factions");
		foreach(Faction f in factions)
		{
			f.Serialize(writer);
		}
		writer.WriteEndElement ();

		writer.WriteStartElement ("Districts");
		foreach(District d in districts)
		{
			d.Serialize(writer);
		}
		writer.WriteEndElement();


		writer.WriteEndElement();
	}

	public void Deserialize(System.Xml.XmlReader reader)
	{
		reader.ReadToFollowing ("Turn");
		turn = reader.ReadElementContentAsInt ();
		flags.Clear ();
		reader.ReadToFollowing("Flags");
		System.Xml.XmlReader inner = reader.ReadSubtree();
		while(inner.ReadToFollowing ("Flag"))
		{
			flags.Add(inner.ReadElementContentAsInt());
		}

		reader.ReadToFollowing("Factions");
		factions.Clear ();
		Dictionary<string, Faction> factionDict = new Dictionary<string, Faction>();
		inner = reader.ReadSubtree();
		while(inner.ReadToFollowing("Faction"))
		{
			System.Xml.XmlReader freader = inner.ReadSubtree();
			Faction f = new Faction();
			f.Deserialize(freader);
			factionDict.Add(f.name, f);
			factions.Add (f);
		}

		reader.ReadToFollowing("Districts");
		districts.Clear ();
		inner = reader.ReadSubtree();
		while(inner.ReadToFollowing("District"))
		{
			System.Xml.XmlReader dreader = inner.ReadSubtree();
			District d = new District();
			d.Deserialize(dreader, factionDict);
			districts.Add (d);
		}
        // we have to do the agent creation after the fact. 
        GlobalData.lookupDictionaries.Generate();
        foreach(Faction f in factions)
        {
            f.DeserializeAgents();
        }
        
	}
}

public struct CrossoverData // stuff to carry between the tactical map and the strategic map
{
    public bool showTutorials;
    public District location;
    public enum MissionState
    {
        Incomplete = 0,
        Success,
        Failure
    };
    public MissionState missionState;
}

public struct LookupDictionaries
{
	public Dictionary<string, Faction> factionDict;
	public Dictionary<string, District> districtDict;

	public void Generate()
	{
		factionDict = new Dictionary<string, Faction>();
		districtDict = new Dictionary<string, District>();
		foreach(District d in GlobalData.strategicData.districts)
		{
			districtDict.Add(d.name, d);
		}
		foreach(Faction f in GlobalData.strategicData.factions)
		{
			factionDict.Add(f.name, f);
		}
	}
}

public static class GlobalData
{
	public static bool newGame = true; // if we're a new game or we're loading from a save.
	public static string gameName = "New Game";
	public static int seed;
	public static PlayerData playerData;
	public static StrategicData strategicData;
	public static LookupDictionaries lookupDictionaries;
	public static System.Random rand;
    public static CrossoverData crossoverData;
    
	
	static GlobalData() // Set the defaults.
	{
		seed = Random.seed;
		strategicData.factions = new List<Faction>();
		strategicData.districts = new List<District>();
		strategicData.flags = new List<int>();
		strategicData.turn = 0;
		SetDefaultPlayerData();
		rand = new System.Random(seed);

        crossoverData.showTutorials = true;
	}

	public static bool SaveGameState(string path)
	{
		System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
		settings.Indent = true;
		System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(path, settings);
		// Root element
		writer.WriteStartElement("GameData");

		writer.WriteElementString ("GameName", gameName);

		// Seed
		writer.WriteElementString("Seed", seed.ToString());

		// Player Data
		playerData.Serialize(writer);

		// Strategic Data
		strategicData.Serialize(writer);


		// End Root element
		writer.WriteEndElement();
		writer.Flush();
		writer.Close();
		
		return true;
	}


	public static bool LoadGameState(string path)
	{
		System.Xml.XmlReader reader = System.Xml.XmlReader.Create(path);
		
		reader.ReadToFollowing("GameName");
		gameName = reader.ReadElementContentAsString ();

		reader.ReadToFollowing("Seed");
		int rSeed = reader.ReadElementContentAsInt ();
		seed = rSeed;

		reader.ReadToFollowing ("PlayerData");
		System.Xml.XmlReader inner = reader.ReadSubtree();
		playerData.Deserialize(inner);

		reader.ReadToFollowing("StrategicData");
		inner = reader.ReadSubtree();
		strategicData.Deserialize(inner);

		reader.Close ();

		lookupDictionaries.Generate();
		return true;
	}


	public static void SetDefaultAppearance()
	{
		playerData.appearance.isFemale = true;
		// playerData.appearance.primaryColor = new Color(12/255f, 55/255f, 112/255f);
		playerData.appearance.primaryColor = new Color(255/255f, 255/255f, 255/255f);
		playerData.appearance.secondaryColor = new Color(12/255f, 55/255f, 112/255f);
		playerData.appearance.tertiaryColor = new Color(195/255f, 186/255f, 5/255f);
		playerData.appearance.hairTint = new Color(148/255f, 21/255f, 21/255f);
		playerData.appearance.skinTint = new Color(234/255f, 216/255f, 205/255f);

		playerData.appearance.shape = new Appearance.Shape();

		// playerData.appearance.shape
		// should all be set to 0 by default. 
	}
	
	public static void SetDefaultPowers()
	{
		playerData.powers = new HashSet<SuperPower>();
		playerData.powers.Add(SuperPower.Strength);
		playerData.powers.Add(SuperPower.Flight);
	}
	
	public static void AddAllPowers()
	{
		playerData.powers.Add(SuperPower.Telekinesis);
		playerData.powers.Add(SuperPower.Flight);
		playerData.powers.Add(SuperPower.Strength);
		playerData.powers.Add(SuperPower.Fire);
		playerData.powers.Add(SuperPower.Ice);
		playerData.powers.Add(SuperPower.Speed);
		playerData.powers.Add(SuperPower.Invulnerability);
		playerData.powers.Add(SuperPower.Telepathy);
	}
	public static void SetDefaultGear()
	{
		playerData.gear.head = 0;
		playerData.gear.body = 0;
		playerData.gear.boots = 0;
		playerData.gear.gloves = 0;
		playerData.gear.weapon = 0;
	}
	
	public static void SetDefaultPlayerData()
	{
		playerData.attributes.charisma = 5;
		playerData.attributes.constitution = 5;
		playerData.attributes.intelligence = 5;
		playerData.attributes.stamina = 5;
		CalculateStats();
		playerData.stats.money = 0;
		playerData.stats.reputation = 0;
		playerData.stats.research = 0;
		playerData.name = "Captain Fantastic";
		SetDefaultPowers();
		SetDefaultAppearance();
		SetDefaultGear();
	}

	public static void CalculateStats()
	{
		playerData.stats.health = 5*playerData.attributes.constitution;
		playerData.stats.energy = 5*playerData.attributes.stamina;
	}

}
