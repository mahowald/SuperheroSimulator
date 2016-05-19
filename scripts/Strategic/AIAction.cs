using System.Collections;
using System.Collections.Generic;


public class AIAction : StrategicAction
{
	public Dictionary<Agent, District> agents; // where we are moving each agent to.
	/** Inherited members:
	public string name;
	public District location;
	public StrategicActionType actionType; 
		MoveAgents, // move agents to another, adjacent district
		Fortify, // agent(s) are stationary, but get a defensive bonus
		Recruit, // recruit new agents
		IncreaseInfluence // increase control in the district.
	**/

	// For moveAgents, District is the target, and agents contains the list of agents we move to that spot. 

	public AIAction(StrategicActionType actionType, District location, Dictionary<Agent, District> agents)
	{
		this.actionType = actionType;
		// this.name = name;
		this.location = location;
		this.agents = agents;
	}

	public AIAction(District location):this(StrategicActionType.DoNothing, location, new Dictionary<Agent, District>())
	{

	}
}
