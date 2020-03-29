using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class TMapController : MonoBehaviour
{
    public enum mapRound { moving, attacking }

    [Header("Set in Inspector")]
    public Tilemap land;
    public Tilemap obstacles;
    public Tilemap highlights;
    public TileBase moveHighlight;
    public TileBase attackHighlight;
    public GameObject archer;
    public GameObject warrior;
    public GameObject wizard;
	public GameObject knight;
	public GameObject armyPrefab;
    public GameObject attackerGameObject;
    public GameObject defenderGameObject;

    [Header("Set Dynamically")]
    public Army attacker;
    public Army defender;
    public Army currentTurn;
    public GameObject moving;
    public mapRound roundState;

    public static TMapController M;

	private GameEvent gEvent;

	private void Awake() {
		M = this;
		attackerGameObject = Instantiate(armyPrefab);
		defenderGameObject = Instantiate(armyPrefab);
        attacker = attackerGameObject.GetComponent<Army>();
        defender = defenderGameObject.GetComponent<Army>();
		if (GameState.GS != null) {
			GameEvent temp = GameState.GS.events[0];
			attacker.unitDescriptions = temp.initiator.squadrons[temp.initiatorArmyID].units;
			defender.unitDescriptions = temp.target.squadrons[temp.targetID].units;
			GameState.GS.events.Remove(temp);
			gEvent = temp;
		}

		/*GameEvent tEvent = GameState.GS.events[0];
		GameState.GS.events.Remove(tEvent);
		attacker = tEvent.initiator;
		defender = tEvent.target;
		attackerGameObject = attacker.gameObject;
		defenderGameObject = defender.gameObject;*/
	}

    // Start is called before the first frame update
    void Start()
    {
        currentTurn = attacker;
        roundState = mapRound.moving;

        //makeUnit(archer, 'A', 1, -2);
        //makeUnit(warrior, 'A', -2, -1);
        //makeUnit(wizard, 'D', 7, 1);
		//makeUnit(knight, 'D', 5, 1);*/
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown("space")) {
            highlights.ClearAllTiles();
            roundState = mapRound.moving;
            moving = null;
			print("End of current turn");
			if (currentTurn == attacker) {
				currentTurn = defender;
				attacker.EndTurn();
				defender.BeginTurn();
			}else {
				currentTurn = attacker;
				attacker.BeginTurn();
				defender.EndTurn();
			}
		}
    }

	public void StartBattle() {
		Destroy(gameObject.GetComponent<SetUnitController>());
		gameObject.AddComponent<TClickDetector>();

		attacker.OnTacticalBattleStart();
		defender.OnTacticalBattleStart();

		attacker.BeginTurn();
		defender.EndTurn();
	}

	public void PlaceUnit(GameObject unit) {
		Unit unitScript = unit.GetComponent<Unit>();
		unit.transform.position = land.CellToWorld(unitScript.currentPlayerTile);
	}

    public void makeUnit(GameObject prefab, char team, int x, int y)
    {
        GameObject unit = Instantiate(prefab);
        Unit unitScript = unit.GetComponent<Unit>();
        Vector3Int v3i = new Vector3Int(x, y, 0);
        unitScript.currentPlayerTile = v3i;
        unit.transform.position = land.CellToWorld(unitScript.currentPlayerTile);
        if (team == 'A')
        {
            attacker.troops.Add(unitScript);
            unitScript.army = attacker;
        }
        else
        {
            defender.troops.Add(unitScript);
            unitScript.army = defender;
        }
    }

    public void endMove(Vector3Int cel) {
		if (moving != null) moving.GetComponent<Unit>().endMove(cel);
    }

	public void ArmyLost(Army loser) {
		if (loser == attacker) {
			print("The attacker was defeated.\nThe defender has won!");
			gEvent.initiator.SquadDefeated(gEvent.initiatorArmyID);
		} else {
			print("The attacker has defeated the defender!");
			gEvent.target.SquadDefeated(gEvent.targetID);
		}
		SceneManager.LoadScene("TacticalResultsScene", LoadSceneMode.Additive);
		SceneManager.MoveGameObjectToScene(GameObject.FindWithTag("AudioSource"), SceneManager.GetSceneByName("TacticalResultsScene"));
	}
}
