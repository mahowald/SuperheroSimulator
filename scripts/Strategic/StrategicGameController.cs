using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StrategicGameController : MonoBehaviour {

	public List<GameObject> districtGameObjects;
	public List<Transform> districtCenters;

    private List<DistrictHandler> districtHandlers;

	// public Transform agentTokenPlaceholder; // eventually, each agent will come equipped with their own data about what token to use!

    public Transform policeAgentToken; // for the police (always blue)
    public Transform criminalAgentToken; // for criminals. (will be colored by faction color)

	private Dictionary<Agent, Transform> agentTokens;

    private District selectedDistrict;


	/** in alphabetical order:
	 * Chinatown
	 * Docks
	 * Downtown
	 * Eastport
	 * Midtown
	 * North End
	 * Pleasantview
	 * Sunny Acres
	 * Uptown
	 * */

	public List<StrategicEvent> events; // the strategic events
	public Dictionary<Faction, List<AIAction>> aiActions; // what the AI players chooses to do.
	public StrategicAction playerAction; // What the human player chooses to do. 

	private int turncounter = 0;

	void FirstTimeSetup()
	{
		FirstTimeSetupFactions();
		FirstTimeSetupDistricts();
		GlobalData.lookupDictionaries.Generate(); // this is done when we load the game state.
	}

	void LoadExternalFiles()
	{
		DataDeserializer.Deserialize(); 
	}

	void Start () 
	{
		if(false) // (GlobalData.newGame)
		{
            Debug.Log("starting new game!");
			FirstTimeSetup();

            // Condition testCondition = Condition.ReadFromString("NOT={District-Downtown:controlled_by=Police,player:has_money=5}", GlobalData.lookupDictionaries.districtDict, GlobalData.lookupDictionaries.factionDict);
            // Debug.Log (testCondition.IsTrue());


            // TEST Condition:
            /** 
			Condition<string, int> testCondition1 = new Condition<string, int>(null, ECondition.Has_money, 0); // true
			Condition<District, object> testCondition2 = new Condition<District, object>(GlobalData.strategicData.districts[0], ECondition.Controlled_by, GlobalData.strategicData.factions[0]); // false


			List<Condition> conditions = new List<Condition>();
			conditions.Add (testCondition1);
			conditions.Add (testCondition2);

			Condition<object, List<Condition>> listCondition = new Condition<object, List<Condition>>(ECondition.Or, conditions);

			Debug.Log (listCondition.IsTrue());
			**/

            // Debug.Log (System.IO.File.ReadAllText(@"Data\data.txt"));



            // GlobalData.SaveGameState(@"Data\Saves\save.xml");
            // GlobalData.LoadGameState(@"Data\Saves\save.xml");
            // SetupDistricts();

            /**  Testing save game loading
			Faction f = GlobalData.strategicData.factions[0];
			District d = GlobalData.strategicData.districts[0];
			Debug.Log ("Faction: " + f.name + " has influence=" + d.ownership[f].ToString() + " in district " + d.name);
			**/

            // SaveGame();
            
		}

        

    }

    bool firstFrame = true;

    // Update is called once per frame
    void Update () {
        if(firstFrame)
        {
            agentTokens = new Dictionary<Agent, Transform>();
            LoadGame();
            LoadExternalFiles();

            // Set up the events list.
            SetupEvents();

            playerAction = new StrategicAction();
            aiActions = new Dictionary<Faction, List<AIAction>>();
            foreach (Faction f in GlobalData.strategicData.factions)
            {
                aiActions.Add(f, new List<AIAction>());
            }

            conflictDistricts = new List<District>();

            DoTurn();
            
            firstFrame = false;
            
        }
        
        
	}

    public void Patrol() // the player does their stuff
    {
        GlobalData.crossoverData.location = selectedDistrict;
        bool districtEmpty = true;
        GlobalData.crossoverData.showTutorials = false;

        // if the district is empty, do nothing:
        foreach(Faction f in GlobalData.strategicData.factions)
        {
            if(f.HasAgentInDistrict(selectedDistrict))
            {
                districtEmpty = false;
            }
        }

        if(districtEmpty)
        {
            DoTurn();
        }


        // if there are enemies present
        if (districtEmpty == false)
        {
            SaveGame();
            GlobalData.crossoverData.missionState = CrossoverData.MissionState.Incomplete;
            Application.LoadLevel("level_alley");
        }
    }


    /** UI Triggered Actions **/

    List<District> conflictDistricts;

    private void DetermineConflicts()
    {
        // Update conflict districts
        conflictDistricts = new List<District>();

        foreach (District d in GlobalData.strategicData.districts)
        {
            Dictionary<Faction, List<Agent>> conflict = d.GetOccupants();

            if (conflict.Keys.Count > 1) // if more than one faction has agents here
            {
                //TODO: Handle conflict!

                conflictDistricts.Add(d);

                // districtGameObjects[d.index].GetComponent<MoveCameraToDistrict>().BaseTint = Color.red;
            }
            else
            {
                // districtGameObjects[d.index].GetComponent<MoveCameraToDistrict>().ResetTint();
            }
        }
    }

	public void DoTurn()
	{
        // this is called after the player returns from the tactical map.
		// Handle Current Player Choices
		HandlePlayerAction();
        District playerDistrict = GlobalData.crossoverData.location;
        
        // Resolve conflict districts
        foreach (District d in conflictDistricts)
        {
            // The player-selected district is handled specially. 
            if (d.name == playerDistrict.name)
            {
                continue;
            }

            // autoresolve conflicts
            Dictionary<Faction, List<Agent>> conflict = d.GetOccupants();

            Dictionary<Faction, int> factionStrengths = new Dictionary<Faction, int>();

            foreach (Faction f in conflict.Keys)
            {
                factionStrengths.Add(f, conflict[f].Count);
                if(f.evil) // give a boost to the criminals
                {
                    factionStrengths[f] += 1;
                }
            }

            Faction winner = WeightedRandomChooser.ChooseRandom<Faction>(factionStrengths, GlobalData.rand);

            foreach (Faction f in conflict.Keys)
            {
                if (f != winner)
                {
                    foreach (Agent a in conflict[f])
                    {
                        f.RemoveAgent(a);
                        a.employer = null;
                        Destroy(agentTokens[a].gameObject);
                        agentTokens.Remove(a);
                    }
                    Debug.Log("Faction " + f.name + " lost.");
                }
            }

        }

        // Update Factions
        UpdateFactions();

        // Update Districts
        UpdateDistricts ();

		// Update Player
		UpdatePlayer ();

		// Update turn counter.
		GlobalData.strategicData.turn += 1;

        /**
		 * Now it is the next turn!
		 * **/

        turncounter++;
        Debug.Log("Turn: " + turncounter.ToString());

        // Random Events
        StrategicEvent ev = PickEvent();
		FireEvent(ev); // This is where we handle the event!
		if(ev != null)
		{
			if(ev.onlyHappensOnce)
			{
				events.Remove(ev);
			}
		}

		// Factions Move, starting with criminals (hopefully?)
		int policeIndex = 0;
		for(int i = 0; i < GlobalData.strategicData.factions.Count; i++)
		{
			Faction f = GlobalData.strategicData.factions[i];
			if(!f.evil)
			{
				policeIndex = i; // TODO: Change this if we have more than one non-evil faction!
				continue;
			}
			aiActions[f] = f.FactionAI.GetChoices();
			ResolveAIAction(f); // each moves in turn. 
		}
		Faction police = GlobalData.strategicData.factions[policeIndex];
		aiActions[police] = police.FactionAI.GetChoices ();
		ResolveAIAction(police);

        // Resolve AI Actions
        // Instead of resolving all the actions at once at the end of the turn, we each take turns.
        // ResolveAIActions();
        
        // Determine the conflicts that the player will see. 
        DetermineConflicts();

        

	}

	void ResolveAIAction(Faction f)
	{
		foreach(AIAction action in aiActions[f])
		{
			if(action.actionType != StrategicActionType.DoNothing)
			{
				Debug.Log ("Faction: " + f.name + " chooses to " + action.actionType.ToString() + " in " + action.location.name);
			}
			switch(action.actionType)
			{
			    case StrategicActionType.MoveAgents:
				    foreach(Agent agent in action.agents.Keys)
				    {
					
					    agent.MoveTo(action.agents[agent]); // Update the location.
					    // Debug.Log (f.name + " moves agents " + action.location.name + "->" + action.agents[agent].name);

					    //TODO: Animate movement token? 
					    agentTokens[agent].GetComponent<MoveTransformToPosition>().SetTarget(districtCenters[action.agents[agent].index]);
				    }
				    break;
				
			        case StrategicActionType.Recruit:
				        Agent newagent = new Agent(f, action.location, 1);
				        f.AddAgent(newagent);
				        Debug.Log (f.name + " recruits 1 agent in " + action.location.name);
                        if(!f.evil)
                        {
                            Transform agentToken = (Transform)Instantiate(policeAgentToken, districtCenters[action.location.index].position, Quaternion.identity);
                            agentTokens.Add(newagent, agentToken);
                        }
                        else
                        {
                            Transform agentToken = (Transform)Instantiate(criminalAgentToken, districtCenters[action.location.index].position, Quaternion.identity);
                            agentTokens.Add(newagent, agentToken);
                        }
                    
				        break;
			    default:
				    break;
			}
		}
	}

	void ResolveAIActions()
	{
		/** Possible choices are: 
		MoveAgents, // move agents to another, adjacent district
		Fortify, // agent(s) are stationary, but get a defensive bonus
		Recruit, // recruit new agents
		IncreaseInfluence // increase control in the district.
		**/
		foreach(Faction f in aiActions.Keys)
		{
			ResolveAIAction (f);
		}

	}

	void HandlePlayerAction()
	{
        Debug.Log("Handling player action");
        District d = GlobalData.crossoverData.location;
        if (d == null) // happens on startup. 
            return;

        Dictionary<Faction, List<Agent>> conflict = d.GetOccupants();

        foreach (Faction f in conflict.Keys)
        {
            if (f.evil)
            {
                foreach (Agent a in conflict[f])
                {
                    f.RemoveAgent(a);
                    a.employer = null;
                    Destroy(agentTokens[a].gameObject);
                    agentTokens.Remove(a);
                }
                Debug.Log("Player victorious in " + d.name);
            }
        }
    }
	

	/** UPDATES: Called Each turn. **/

	void UpdateDistricts()
	{
		// TODO: Implement this.
		foreach(District d in GlobalData.strategicData.districts)
		{
			// Update the district.
		}

        foreach(DistrictHandler dh in districtHandlers)
        {
            dh.UpdateDistrictHandler();
        }
        
	}

	void UpdateFactions()
	{
		foreach(Faction f in GlobalData.strategicData.factions)
		{
			// Update the faction. 
            foreach(Agent a in f.agents)
            {
                // increase influence in places we're stationary
                if(a.location == a.lastLocation)
                {
                    District d = a.location;
                    if(!d.ownership.ContainsKey(f))
                    {
                        d.ownership.Add(f, 0);
                    }
                    
                    d.ownership[f] += 5;
                    Debug.Log(f.name + " agent in " + d.name + " was stationary.");
                }

            }
		}
	}

	void UpdatePlayer()
	{
		//TODO: Implement this.

		// Health and stats

		// injuries

		// equipment purchased/researched

		// public opinion

	}
    /** END UPDATES **/
    public Text districtText;
    public Button patrolButton;

    public void SelectDistrict(District d)
    {
        selectedDistrict = d;
        if(d != null)
        {
            districtText.text = "PATROLLING: " + d.name;
            patrolButton.interactable = true;
        }
        if(d == null)
        {
            districtText.text = "NO DISTRICT SELECTED";
            patrolButton.interactable = false;
        }
        
    }


	/******************** 
	 *     EVENTS !!!   *
	 ********************/

	StrategicEvent PickEvent()
	{
		if(events.Count == 0)
		{
			return null;
		}
		StrategicEvent ev = null;
		float prob = Random.Range (0f, 1f); // the probability check. 
	
		events.Shuffle(); // So we don't check the events in the same order each time. See Extension Methods.

		foreach(StrategicEvent e in events)
		{
			if(e.Probability > prob)
			{
				ev = e;
				break;
			}
		}

		return ev;
	}

	void FireEvent(StrategicEvent ev)
	{
		//TODO: Pass event to event display.
	}

    /********************************
    *             Setup
    ********************************/
	void SetupDistricts()
	{
        districtHandlers = new List<DistrictHandler>();
		for(int i = 0; i < GlobalData.strategicData.districts.Count; i++)
		{
			District d = GlobalData.strategicData.districts[i];
			GameObject dgo = districtGameObjects[i];
            DistrictHandler dh = dgo.GetComponent<DistrictHandler>();
			dh.district = d;
            districtHandlers.Add(dh);
		}
	}

    void SetupAgents()
    {
        // remove existing agent tokens
        foreach(Agent a in agentTokens.Keys)
        {
            if (agentTokens[a] != null)
            {
                Destroy(agentTokens[a].gameObject);
            }
        }
        agentTokens.Clear();

        foreach(Faction f in GlobalData.strategicData.factions)
        {
            foreach(Agent a in f.agents)
            {
                if (!f.evil)
                {
                    Transform agentToken = (Transform)Instantiate(policeAgentToken, districtCenters[a.location.index].position, Quaternion.identity);
                    agentTokens.Add(a, agentToken);
                }
                else
                {
                    Transform agentToken = (Transform)Instantiate(criminalAgentToken, districtCenters[a.location.index].position, Quaternion.identity);
                    agentTokens.Add(a, agentToken);
                }
            }
        }

    }

	/** First Time SETUP **/
	void FirstTimeSetupDistricts()
    {
        districtHandlers = new List<DistrictHandler>();
        GlobalData.strategicData.districts.Clear();
		for(int i = 0; i < districtGameObjects.Count; i++)
		{
			GameObject dgo = districtGameObjects[i];
			District district = new District(dgo.name, i);
			GlobalData.strategicData.districts.Add(district);
            DistrictHandler dh = dgo.GetComponent<DistrictHandler>();
            dh.district = district;
            districtHandlers.Add(dh);
        }

		GlobalData.strategicData.districts[2].ownership[GlobalData.strategicData.factions[0]] = 5; // police own downtown
		GlobalData.strategicData.districts[3].ownership[GlobalData.strategicData.factions[1]] = 5; // criminals own eastport
	}

	void FirstTimeSetupFactions()
	{
		Faction police = new Faction("Police", false);
		Faction mafia = new Faction("The Mafia", true);

		police.primaryColor = new Color(30f/255f, 122f/225f, 227f/255f);
		mafia.primaryColor = new Color(227f/255f, 30f/255f, 30f/255f);

		GlobalData.strategicData.factions.Add (police);
		GlobalData.strategicData.factions.Add (mafia);

		agentTokens = new Dictionary<Agent, Transform>();
	}

	void SetupEvents()
	{
		events = new List<StrategicEvent>();

		//TODO: Load events from file.
	}


    /**
    * Saving and loading files
    **/

    public void SaveGame()
    {
        GlobalData.SaveGameState(@"Data\Saves\save.xml");
    }

    public void LoadGame()
    {
        GlobalData.LoadGameState(@"Data\Saves\save.xml");
        SetupDistricts();
        SetupAgents();

        // Update Factions
        UpdateFactions();
        // Update Districts
        UpdateDistricts();
        // Update Player
        UpdatePlayer();
        // Determine conflicts
        DetermineConflicts();
        Debug.Log("Conflict in districts: ");
        foreach(District d in conflictDistricts)
        {
            Debug.Log(d.name);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
