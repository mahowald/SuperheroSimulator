using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Condition
{

	public enum Context // determines what keywords should resolve to
	{
		General=0, // keywords resolve to null.
		FactionAI 
	};

	abstract public bool IsTrue();
	override abstract public string ToString();
	abstract public void Serialize(System.Xml.XmlWriter writer);

	public static Condition Deserialize(System.Xml.XmlReader reader, Dictionary<string, District> districtDict, Dictionary<string, Faction> factionDict)
	{
		reader.ReadToFollowing("Condition");
		string conditionstr = reader.ReadElementContentAsString();
		return Condition.ReadFromString(conditionstr, districtDict, factionDict);
	}

	public static Condition ReadFromString(string input, Dictionary<string, District> ddict, Dictionary<string, Faction> fdict)
	{
		return Condition.ReadFromString(input, ddict, fdict, Context.General);
	}

	public static Condition ReadFromString(string input, Dictionary<string, District> ddict, Dictionary<string, Faction> fdict, Context context)
	{
		ECondition cond = ECondition.Null;
		string instr = input.Trim();
		
		int andIndex = instr.IndexOf("AND={");
		int orIndex = instr.IndexOf ("OR={");
		int notIndex = instr.IndexOf ("NOT={");
		int endIndex = instr.LastIndexOf("}");
		if(andIndex != -1 || orIndex != -1 || notIndex != -1)
		{
			string substr = "";
			List<Condition> conditions = new List<Condition>();
			if(andIndex == 0)
			{
				substr = input.Substring(5, endIndex - 5);
				cond = ECondition.And;
			}
			else if(orIndex == 0)
			{
				substr = input.Substring(4, endIndex - 4);
				cond = ECondition.Or;
			}
			else if(notIndex == 0)
			{
				substr = input.Substring(5, endIndex - 5);
				cond = ECondition.Not;
			}
			
			List<string> terms = new List<string>(substr.Split(new char[]{','}));
			foreach(string term in terms)
			{
				conditions.Add(Condition.ReadFromString(term, ddict, fdict));
			}
			
			return new Condition<object, List<Condition>>(cond, conditions);
			
		}
		
		// For each individual char:
		string[] splitinput = instr.Split(new Char[]{':','='});
		string scopestr = splitinput[0];
		string conditionstr = splitinput[1];
		string valuestr = splitinput[2];
		
		Del<District> d = () => null;
		Del<Faction> f = () => null;
		
		if(ddict.ContainsKey(scopestr))
		{
			d = () => ddict[scopestr];
		}
		if(ddict.ContainsKey(valuestr))
		{
			d = () => ddict[valuestr];
		}
		if(fdict.ContainsKey(scopestr))
		{
			f = () => fdict[scopestr];
		}
		if(fdict.ContainsKey(valuestr))
		{
			f = () => fdict[valuestr];
		}
		
		if(context == Context.FactionAI)
		{
			if(scopestr == "this_faction" || valuestr == "this_faction")
			{
				f = new Del<Faction>(FactionAI.ConditionThisFaction);
			}
			if(scopestr == "this_district" || valuestr == "this_district")
			{
				d = new Del<District>(FactionAI.ConditionThisDistrict);
			}
		}

		switch(conditionstr)
		{
			// Global:
		case "has_flag":
			cond = ECondition.Has_flag;
			int flag = Int32.Parse (valuestr);
			return new Condition<string, int>("global", cond, flag);
		case "num_villains":
			cond = ECondition.Num_villains;
			int numv = Int32.Parse (valuestr);
			return new Condition<string, int>("global", cond, numv);
		case "num_turns":
			cond = ECondition.Num_turns;
			int numt = Int32.Parse (valuestr);
			return new Condition<string, int>("global", cond, numt);
		case "num_turns_gt":
			cond = ECondition.Num_turns_gt;
			int numtgt = Int32.Parse (valuestr);
			return new Condition<string, int>("global", cond, numtgt);
		case "num_turns_lt":
			cond = ECondition.Num_turns_lt;
			int numtlt = Int32.Parse (valuestr);
			return new Condition<string, int>("global", cond, numtlt);
			
			
			// Player:
		case "has_money":
			cond = ECondition.Has_money;
			int numm = Int32.Parse (valuestr);
			return new Condition<string, int>("player", cond, numm);
		case "has_reputation":
			cond = ECondition.Has_reputation;
			int numr = Int32.Parse (valuestr);
			return new Condition<string, int>("player", cond, numr);
		case "has_research":
			cond = ECondition.Has_research;
			int numres = Int32.Parse (valuestr);
			return new Condition<string, int>("player", cond, numres);
		case "has_power":
			cond = ECondition.Has_power;
			SuperPower power = (SuperPower) Enum.Parse (typeof(SuperPower), valuestr);
			return new Condition<string, SuperPower>("player", cond, power);
			
			// District:
		case "controlled_by":
			cond = ECondition.Controlled_by;
			return new Condition<District, Faction>(d, cond, f);
		case "controlled_by_criminals":
			cond = ECondition.Controlled_by_criminals;
			bool bcbc = bool.Parse(valuestr);
			return new Condition<District, bool>(d, cond, bcbc);
			
		case "criminals_control":
			cond = ECondition.Criminals_control;
			float percent = float.Parse (valuestr);
			return new Condition<District, float>(d, cond, percent);
			
			// Faction: 
		case "num_agents":
			cond = ECondition.Num_agents;
			int countag = Int32.Parse(valuestr);
			return new Condition<Faction, int>(f, cond, countag);
			
		case "controls_district":
			cond = ECondition.Controls_district;
			return new Condition<Faction, District>(f, cond, d);
			
		case "controlled_districts":
			cond = ECondition.Controlled_districts;
			int countcd = Int32.Parse(valuestr);
			return new Condition<Faction, int>(f, cond, countcd);
		case "is_evil":
			cond = ECondition.Is_evil;
			bool bie = bool.Parse (valuestr);
			return new Condition<Faction, bool>(f, cond, bie);
			
		case "adjacent_enemy":
			cond = ECondition.Adjacent_enemy;
			return new Condition<Faction, District>(f, cond, d);
		case "agent_in_district":
			cond = ECondition.Agent_in_district;
			return new Condition<Faction, District>(f, cond, d);
		}
		
		return null;
	}
	/** 
	public static Condition ReadFromString(string input, Dictionary<string, District> districtDict, Dictionary<string, Faction> factionDict)
	{
		DistrictDict ddict = str => {
			if(districtDict.ContainsKey(str))
				return districtDict[str];
			else
				return null;
		};

		FactionDict fdict = str => {
			if(factionDict.ContainsKey(str))
				return factionDict[str];
			else
				return null;
		};

		return Condition.ReadFromString(input, ddict, fdict);
		
	}
	**/
	
}

public enum ECondition
{
	
	// Other
	Null=0,

	// Global scope
	Has_flag,
	Num_villains,
	Num_turns,
	Num_turns_gt,
	Num_turns_lt,
	
	// Player scope
	Has_money,
	Has_reputation,
	Has_research,
	Has_equipment,
	Has_power,
	
	// District scope
	Controlled_by,
	Controlled_by_criminals,
	Criminals_control,
	Happiness,
	
	// Faction scope
	Num_agents,
	Controls_district,
	Controlled_districts,
	Is_evil,
	Adjacent_enemy, // to given district
	Agent_in_district,

	
	// Logical
	And,
	Or,
	Not
	
};

public delegate T Del<T>();


public class Condition<S, V> : Condition // scope, value
	// Represents a logical condition.
	/** has the following properties:
	 * 	1. Scope -- what are we looking at?
	 * 		i. Global (checks the whole game board)
	 * 		ii. District (checks a specific district)
	 * 		iii. Player (checks the properties of the human player)
	 * 		iv. Faction (checks the properties of the specified faction)
	 * 	2. The actual condition to check.
	 * 		Global conditions:
	 * 			i. num_villains = int : there are this number of supervillains in play
	 * 			ii. has_flag = (flag) : the following flag has been set.
	 * 		Player conditions:
	 * 			i. has_money = int : has at least this much cash in the bank.
	 * 			ii. has_opinion = int : has at least this high public opinion.
	 * 		District conditions:
	 * 			i. controlled_by = (faction) : (faction) is the largest controller
	 * 			ii. controlled_by_criminals = T/F : evil factions control more than 50% of the district.
	 * 		Faction conditions:
	 * 			i. has_agents = int : has at least this many available agents.
	 * 			ii. controlled_districts = int : has at least this many controlled districts. 
	 * 	3. The value we want.
	 * */
{

	// public delegate S Scope<S>();
	// public delegate V Value<V>();
	
	private ECondition provision;
	private Del<S> scope;
	private Del<V> value;

	private List<Condition> conditions;


	public Condition(S scope, ECondition provision, V value)
	{
		this.scope = () => scope;
		this.provision = provision;
		this.value = () => value;
	}
	
	public Condition(Del<S> scope, ECondition provision, V value)
	{
		this.scope = scope;
		this.provision = provision;
		this.value = () => value;
	}
	
	public Condition(S scope, ECondition provision, Del<V> value)
	{
		this.scope = () => scope;
		this.provision = provision;
		this.value = value;
	}

	public Condition(Del<S> scope, ECondition provision, Del<V> value)
	{
		this.scope = scope;
		this.provision = provision;
		this.value = value;
	}

	public Condition(ECondition provision, List<Condition> conditions)
	{
		if(provision != ECondition.And && provision != ECondition.Or && provision != ECondition.Not)
		{
			throw new Exception("Not a logical condition!");
		}
		this.provision = provision;
		this.conditions = conditions;
	}

	public ECondition Provision
	{
		get
		{
			return provision;
		}
	}

	override public bool IsTrue()
	{
		switch(provision)
		{
			// Global:
		case ECondition.Has_flag:
			if(value() is int)
			{
				int flag = Convert.ToInt32(value());
				return GlobalData.strategicData.flags.Contains(flag);
			}
			break;
		case ECondition.Num_villains:
			if(value() is int)
			{
				int num = Convert.ToInt32(value());
				return this.NumVillains() == num;
			}
			break;
		case ECondition.Num_turns:
			if(value() is int)
			{
				int num = Convert.ToInt32(value());
				return GlobalData.strategicData.turn == num;
			}
			break;
		case ECondition.Num_turns_gt:
			if(value() is int)
			{
				int num = Convert.ToInt32(value());
				return GlobalData.strategicData.turn > num;
			}
			break;
		case ECondition.Num_turns_lt:
			if(value() is int)
			{
				int num = Convert.ToInt32(value());
				return GlobalData.strategicData.turn < num;
			}
			break;


			// Player:
		case ECondition.Has_money:
			if(value() is int)
			{
				int cash = Convert.ToInt32(value());
				return GlobalData.playerData.stats.money >= cash;
			}
			break;
		case ECondition.Has_reputation:
			if(value() is int)
			{
				int rep = Convert.ToInt32 (value());
				return GlobalData.playerData.stats.reputation >= rep;
			}
			break;
		case ECondition.Has_research:
			if(value() is int)
			{
				int res = Convert.ToInt32 (value());
				return GlobalData.playerData.stats.research >= res;
			}
			break;
		case ECondition.Has_power:
			if(value() is SuperPower)
			{
				SuperPower power = (SuperPower)Convert.ToInt32(value());
				return GlobalData.playerData.HasPower(power);

			}
			break;
		case ECondition.Has_equipment:
			// TODO: Implement this!
			return false;

			// District:
		case ECondition.Controlled_by:
			if(scope() is District)
			{
				if(value() is Faction)
				{
					District d = scope() as District;
					Faction f = value() as Faction;
					return d.ControlledBy(f);
				}
			}
			break; 
		case ECondition.Controlled_by_criminals:
			if(scope() is District)
			{
				if(value() is bool)
				{
					District d = scope() as District;
					bool b = Convert.ToBoolean(value());
					return d.ControlledByCriminals() == b;
				}
			}
			break;

		case ECondition.Criminals_control:
			if(scope() is District)
			{
				District d = scope() as District;
				if(value() is float)
				{
					float percent = (float) Convert.ToDouble(value());
					return d.CriminalsControl() >= percent;
				}
			}
			break;

			// Faction: 
		case ECondition.Num_agents:
			if(scope() is Faction)
			{
				Faction f = scope() as Faction;
				if(value() is int)
				{
					int count = Convert.ToInt32(value());
					return f.agents.Count >= count;
				}
			}
			break;

		case ECondition.Controls_district:
			if(scope() is Faction)
			{
				Faction f = scope() as Faction;
				if(value() is District)
				{
					District d = value() as District;
					return d.ControlledBy(f);
				}
			}
			break;

		case ECondition.Controlled_districts:
			if(scope() is Faction)
			{
				Faction f = scope() as Faction;
				if(value() is int)
				{
					int count = Convert.ToInt32 (value());
					return f.ControlledDistricts() >= count;
				}
			}
			break;
		case ECondition.Is_evil:
			if(scope() is Faction)
			{
				if(value() is bool)
				{
					Faction f = scope() as Faction;
					bool b = Convert.ToBoolean(value());

					return f.evil == b;
				}
			}
			break;
		case ECondition.Adjacent_enemy:
			if(scope() is Faction)
			{
				if(value() is District)
				{
					Faction f = scope() as Faction;
					District d = value() as District;
					return f.HasAdjacentEnemy(d);
				}
			}
			break;
		case ECondition.Agent_in_district:
			if(scope() is Faction)
			{
				if(value() is District)
				{
					Faction f = scope() as Faction;
					District d = value() as District;
					return f.HasAgentInDistrict(d);
				}
			}
			break;

		case ECondition.And: // checks that all of the conditions are true
			foreach(Condition c in conditions)
			{
				if(!c.IsTrue())
				{
					return false;
				}
			}
			return true;
			break;

		case ECondition.Or: // checks that one of the conditions is true
			foreach(Condition c in conditions)
			{
				if(c.IsTrue ())
				{
					return true;
				}
			}
			return false;

		case ECondition.Not: // really, checks that none of the conditions are true
			foreach(Condition c in conditions)
			{
				if( c.IsTrue())
					return false;
			}
			return true;


		default:
			break;
		}

		return false;
	}


	private int NumVillains()
	{
		int count = 0;
		foreach(Faction f in GlobalData.strategicData.factions)
		{
			if(f.evil)
			{
				count++;
			}
		}

		return count;
	}

	override public string ToString()
	{
		string output = "";
		switch(provision)
		{
			// Global:
		case ECondition.Has_flag:
			if(value() is int)
			{
				int flag = Convert.ToInt32(value());
				output = "global:has_flag=" + flag.ToString();
			}
			break;
		case ECondition.Num_villains:
			if(value() is int)
			{
				int num = Convert.ToInt32(value());
				output = "global:num_villains=" + num.ToString();
			}
			break;
		case ECondition.Num_turns:
			if(value() is int)
			{
				int num = Convert.ToInt32(value());
				output = "global:num_turns=" + num.ToString();
			}
			break;
		case ECondition.Num_turns_gt:
			if(value() is int)
			{
				int num = Convert.ToInt32(value());
				output = "global:num_turns_gt=" + num.ToString();
			}
			break;
		case ECondition.Num_turns_lt:
			if(value() is int)
			{
				int num = Convert.ToInt32(value());
				output = "global:num_turns_lt=" + num.ToString();
			}
			break;
			
			
			// Player:
		case ECondition.Has_money:
			if(value() is int)
			{
				int cash = Convert.ToInt32(value());
				output = "player:has_money=" + cash.ToString();
			}
			break;
		case ECondition.Has_reputation:
			if(value() is int)
			{
				int rep = Convert.ToInt32 (value());
				output = "player:has_reputation=" + rep.ToString();
			}
			break;
		case ECondition.Has_research:
			if(value() is int)
			{
				int res = Convert.ToInt32 (value());
				output = "player:has_research=" + res.ToString();
			}
			break;
		case ECondition.Has_power:
			if(value() is SuperPower)
			{
				SuperPower power = (SuperPower)Convert.ToInt32(value());
				output = "player:has_power=" + power.ToString();
				
			}
			break;
		case ECondition.Has_equipment:
			// TODO: Implement this!
			break;
			
			// District:
		case ECondition.Controlled_by:
			if(scope() is District)
			{
				if(value() is Faction)
				{
					District d = scope() as District;
					Faction f = value() as Faction;
					output = d.name + ":controlled_by=" + f.name;
				}
			}
			break; 
		case ECondition.Controlled_by_criminals:
			if(scope() is District)
			{
				if(value() is bool)
				{
					District d = scope() as District;
					bool b = Convert.ToBoolean(value());
					output = d.name + ":controlled_by_criminals=" + b.ToString();
				}
			}
			break;
			
		case ECondition.Criminals_control:
			if(scope() is District)
			{
				District d = scope() as District;
				if(value() is float)
				{
					float percent = (float) Convert.ToDouble(value());
					output = d.name + ":criminals_control=" + percent.ToString();
				}
			}
			break;
			
			// Faction: 
		case ECondition.Num_agents:
			if(scope() is Faction)
			{
				Faction f = scope() as Faction;
				if(value() is int)
				{
					int count = Convert.ToInt32(value());
					output = f.name + ":num_agents=" + count.ToString();
				}
			}
			break;
			
		case ECondition.Controls_district:
			if(scope() is Faction)
			{
				Faction f = scope() as Faction;
				if(value() is District)
				{
					District d = value() as District;
					output = f.name + ":controls_district=" + d.name;
				}
			}
			break;
			
		case ECondition.Controlled_districts:
			if(scope() is Faction)
			{
				Faction f = scope() as Faction;
				if(value() is int)
				{
					int count = Convert.ToInt32 (value());
					output = f.name + ":controlled_districts=" + count.ToString();
				}
			}
			break;
		case ECondition.Is_evil:
			if(scope() is Faction)
			{
				if(value() is bool)
				{
					Faction f = scope() as Faction;
					bool b = Convert.ToBoolean(value());
					output = f.name + ":is_evil=" + b.ToString();
				}
			}
			break;
			
		case ECondition.Adjacent_enemy:
			if(scope() is Faction)
			{
				Faction f = scope() as Faction;
				if(value() is District)
				{
					District d = value() as District;
					output = f.name + ":adjacent_enemy=" + d.name;
				}
			}
			break;
			
		case ECondition.Agent_in_district:
			if(scope() is Faction)
			{
				Faction f = scope() as Faction;
				if(value() is District)
				{
					District d = value() as District;
					output = f.name + ":agent_in_district=" + d.name;
				}
			}
			break;
			
		case ECondition.And: // checks that all of the conditions are true
			output = "AND={";
			for(int i = 0; i < conditions.Count; i++)
			{
				Condition c = conditions[i];
				output += c.ToString();
				
				if(i != conditions.Count - 1)
				{
					output += ",";
				}
			}
			output += "}";
			break;
			
		case ECondition.Or: // checks that one of the conditions is true
			output = "OR={";
			for(int i = 0; i < conditions.Count; i++)
			{
				Condition c = conditions[i];
				output += c.ToString();

				if(i != conditions.Count - 1)
				{
					output += ",";
				}
			}
			output += "}";
			break;
			
		case ECondition.Not: // really, checks that none of the conditions are true
			output = "NOT={";
			for(int i = 0; i < conditions.Count; i++)
			{
				Condition c = conditions[i];
				output += c.ToString();
				
				if(i != conditions.Count - 1)
				{
					output += ",";
				}
			}
			output += "}";
			break;
			
			
		default:
			break;
		}

		return output;
	}

	override public void Serialize(System.Xml.XmlWriter writer)
	{
		writer.WriteElementString ("Condition", this.ToString());
	}

}
