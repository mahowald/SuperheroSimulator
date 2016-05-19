using System.Collections;
using System.Collections.Generic;

public abstract class Effect
{
	abstract public void Apply();
	
	abstract public void Serialize(System.Xml.XmlWriter writer);
	public static Effect Deserialize(System.Xml.XmlReader reader, Dictionary<string, District> districtDict, Dictionary<string, Faction> factionDict)
	{
		return new Effect<object, object>(null, EEffect.Null, null);
	}
}

// very similar to the Condition class, except these actually do things to 
// the global data and the strategic data.
// Have a target, an action, and a value.

public enum EEffect
{
	// target = player:
	Change_money,
	Change_reputation,
	Change_research,

	// target = district:
	Change_crime,

	// target = faction

	Null = 0
};

public class Effect<T, V> : Effect
{
	private EEffect effect;
	private T target;
	private V value;

	public Effect(T target, EEffect effect, V value)
	{
		this.target = target;
		this.effect = effect;
		this.value = value;
	}

	override public void Apply()
	{
		switch(effect)
		{
		default:
		break;
		}
	}

	override public void Serialize(System.Xml.XmlWriter writer)
	{
		writer.WriteStartElement("Effect");
		writer.WriteEndElement();
	}


}