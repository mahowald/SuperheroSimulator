using System.Collections;
using System.Collections.Generic;

public class Agent {

	public Faction employer; // who employs us?

	public District location; // where are we deployed? 

	public District lastLocation; // where were we last turn?

	public int strength; // how strong are we? (used for auto-resolving conflicts)

	public Agent(Faction employer, District location, int str)
	{
		this.employer = employer;
		this.location = location;
		this.strength = str;
		this.lastLocation = null;

		// employer.agents.Add(this);
	}

	public void MoveTo(District d)
	{
		lastLocation = location;
		location = d;
	}

}
