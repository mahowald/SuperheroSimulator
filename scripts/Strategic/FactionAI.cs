using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionAI 
{
	private Faction faction;
	private Dictionary<District, AIAction> actions;

	private Dictionary<Condition, Dictionary<StrategicActionType, int>> modifiers; // if the condition is true, how do we modify the weights?
	private static Faction conditionThisFaction = null;
	private static District conditionThisDistrict = null;

	// private Dictionary<StrategicActionType, Condition> requirements; // if the condition is not met, then we will not choose this.

	public static Faction ConditionThisFaction()
	{
		return conditionThisFaction;
	}

	public static District ConditionThisDistrict()
	{
		return conditionThisDistrict;
	}
	
	public FactionAI(Faction faction)
	{
		this.faction = faction;
		actions = new Dictionary<District, AIAction>();

		modifiers = DataDeserializer.factionAIModifiers; 
		// requirements = new Dictionary<StrategicActionType, Condition>();
	}

    

	public List<AIAction> GetChoices()
	{
		modifiers = DataDeserializer.factionAIModifiers;

		List<AIAction> choices = new List<AIAction>();
        int recruits = 0; // keep track of # of recruit orders
		foreach(District d in GlobalData.strategicData.districts)
		{
			AIAction action = GetActionInDistrict(d, ref recruits);
			choices.Add(action);
		}

		return choices;
	}

	public AIAction GetActionInDistrict(District d, ref int recruits)
	{
		//TODO: Eventually, re-write this so none of these values are hard-coded!
		// (i.e., we load them from some file!)

		AIAction action = null;
		
		// Setup the base priorities:
		Dictionary<StrategicActionType, int> weights = new Dictionary<StrategicActionType, int>()
		{
			{StrategicActionType.DoNothing, 5},
			// {StrategicActionType.Fortify, 1}, // fortify = don't move agents.
			// {StrategicActionType.IncreaseInfluence, 5},
			{StrategicActionType.MoveAgents, 5},
			{StrategicActionType.Recruit, 5}
		};



		// do the modifiers:
		// first we have to set up the right slots!
		conditionThisDistrict = d;
		conditionThisFaction = this.faction;
		foreach(Condition c in modifiers.Keys)
		{
			if(c.IsTrue())
			{
				foreach(StrategicActionType actiont in modifiers[c].Keys)
				{
					weights[actiont] += modifiers[c][actiont];
				}
			}
		}
        
        // "smarter" behavior?

		if(d.ControlledBy(faction) && !faction.HasAdjacentEnemy(d))
		{
			weights[StrategicActionType.Recruit] += 5;
		}
        
		if(faction.HasAgentInDistrict(d) && faction.HasAdjacentEnemy(d))
		{
			weights[StrategicActionType.MoveAgents] += 10;
		}


		// do the requirements:
		if(!d.ControlledBy(faction)) // if we don't control the district:
		{
			weights[StrategicActionType.Recruit] = 0;
			// weights[StrategicActionType.Fortify] = 0;
		}

        if(recruits + faction.agents.Count > faction.AgentLimit())
        {
            weights[StrategicActionType.Recruit] = 0;
        }

		if(!faction.HasAgentInDistrict(d)) // if we don't have an agent in the district
		{
			weights[StrategicActionType.MoveAgents] = 0;
			// weights[StrategicActionType.IncreaseInfluence] = 0;
		}
        

		StrategicActionType actionType = WeightedRandomChooser.ChooseRandom(weights, GlobalData.rand);

		switch(actionType)
		{
		    case StrategicActionType.MoveAgents:
			    Dictionary<Agent, District> moveOrders = new Dictionary<Agent, District>();
			    List<Agent> agentsInDistrict = faction.AgentsInDistrict(d);
			    foreach(Agent a in agentsInDistrict)
			    {
				    Dictionary<District, int> moveTargets = new Dictionary<District, int>();
				    foreach(District adj in d.GetAdjacentDistricts())
				    {
					    moveTargets.Add(adj, 1);
					    if(!adj.ControlledBy(faction))
					    {
						    moveTargets[adj] += 5; // seek out uncontrolled spots
						    if(adj == d)
						    {
							    moveTargets[adj] += 25; // stay in one spot, so we can increase influence next turn
						    }
					    }
					    if(!faction.HasEnemyInDistrict(adj))
					    {
						    moveTargets[adj] += 3; // if they're uncontrolled and no enemies, seek those out
					    }
					    if(faction.HasEnemyInDistrict(adj))
					    {
						    moveTargets[adj] += 5; // but really want to disrupt the enemy spots
						
						    if(adj.ControlledBy(faction)) // and especially if they're hitting our stuff!
						    {
							    moveTargets[adj] += 10;
						    }
					    }

				    }
				
				    District target = WeightedRandomChooser.ChooseRandom(moveTargets, GlobalData.rand);

				    moveOrders.Add(a, target);
			    }
			    action = new AIAction(actionType, d, moveOrders);
			    break;
            case StrategicActionType.Recruit:
                action = new AIAction(actionType, d, null);
                recruits += 1;
                break;
            default:
			    action = new AIAction(actionType, d, null);
			    break;
		}
        

		return action;
	}

}
