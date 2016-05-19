using System.Collections;


public enum StrategicActionType
{
	MoveAgents, // move agents to another, adjacent district
	// Fortify, // agent(s) are stationary, but get a defensive bonus
	Recruit, // recruit new agents
	// IncreaseInfluence, // increase control in the district.
	DoNothing
	
};

public class StrategicAction // Just collects what the player actually does. 
{
	// public string name;
	public District location;
	public StrategicActionType actionType; 
	// For moveAgents, District is where we are currently stationed, and agents contains the list of agents we move to that spot.
}
