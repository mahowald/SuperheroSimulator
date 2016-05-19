using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Transform femaleTransform;

	public Transform maleTransform;

	public Camera mainCamera;

	public List<AIEnemy> enemies;

	public List<AIAgent> npcs;

	private Transform hero;

	// Use this for initialization. called after awake.
	void Start () {
		hero = GetHeroTransform();

		StartCoroutine(Heartbeat());
	}

	void Awake() // called only once, called before any start functions.
	{
		// Application.targetFrameRate = 120; // to help with choppiness

		// GlobalData.SaveGameState(@"Data\Saves\save.xml");

		GlobalData.LoadGameState(@"Data\Saves\save.xml");
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public Transform GetHeroTransform()
	{
		if(GlobalData.playerData.appearance.isFemale)
		{
			return femaleTransform;
		}
		else
		{
			return maleTransform;
		}
	}


    public Text successText;
    bool success = false;
	IEnumerator Heartbeat()
	{
		while(true)
		{
            if(DefeatedEnemies() && !success)
            {
                success = true;
                successText.gameObject.SetActive(true);
                yield return new WaitForSeconds(10f);
            }

            if(success)
            {
                // reset the cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                GlobalData.crossoverData.missionState = CrossoverData.MissionState.Success;
                Application.LoadLevel("strategy_map");
            }

			yield return new WaitForSeconds(1f);
		}
	}

    bool DefeatedEnemies()
    {

        if (enemies.Count == 0)
            return false;

        foreach(AIEnemy enemy in enemies)
        {
            CombatCharacterController combatant = enemy.GetComponent<CombatCharacterController>();
            if(combatant.combatantData.health != 0) // HE LIIIIIVESSS!
            {
                return false;
            }
        }

        return true; // they're all gone
    }


	Dictionary<AIEnemy, Vector3> PickEnemyLocations(List<AIEnemy> enemies)
	{
		/*
		enemies.Sort(delegate(AIEnemy x, AIEnemy y) {
			float xdist = Vector3.Distance(hero.position, x.transform.position);
			float ydist = Vector3.Distance(hero.position, y.transform.position);
			if(xdist < ydist) return -1;
			else if(xdist == ydist) return 0;
			else if(xdist > ydist) return 1;
			return 0;
		});
		*/

		Dictionary<AIEnemy, Vector3> locations = new Dictionary<AIEnemy, Vector3>();


		foreach(AIEnemy enemy in enemies)
		{
			// float offset = Random.Range(-1f,1f);
			// Vector3 target = (offset + enemy.attackRange)*((enemy.transform.position - hero.position).normalized) + hero.position;
			// locations.Add(enemy, target);
			locations.Add(enemy, enemy.transform.position);
		}

		return locations;
	}


	AIEnemy PickEnemyForAttack()
	{
		List<AIEnemy> possibilities = GetEnemiesInRange();

		int range = possibilities.Count;
		if(range == 0)
		{
			return null;
		}

		return possibilities[GlobalData.rand.Next(0, range)];
	}


	List<AIEnemy> GetEnemiesInRange()
	{
		List<AIEnemy> possibilities = new List<AIEnemy>();
		foreach(AIEnemy enemy in enemies)
		{
			// if(Vector3.Distance(hero.transform.position, enemy.transform.position) + enemy.attackRange <= 3f)
			//if(enemy.IsEnemyInRange())
			// {
				possibilities.Add(enemy);
			// }
		}

		return possibilities;
	}



}
