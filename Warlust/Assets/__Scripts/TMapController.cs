using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
	public Text turn;
	public Text message;

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
			attacker.kingdom = GameState.GS.events[0].initiator;
			defender.unitDescriptions = temp.target.squadrons[temp.targetID].units;
			defender.kingdom = GameState.GS.events[0].target;

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
		if (attacker.kingdom == WMapController.M.blue)
			Canvas.FindObjectOfType<Morale>().SwapColors();

		currentTurn = attacker;
        roundState = mapRound.moving;
        moving = null;

        //makeUnit(archer, 'A', 1, -2);
        //makeUnit(warrior, 'A', -2, -1);
        //makeUnit(wizard, 'D', 7, 1);
		//makeUnit(knight, 'D', 5, 1);*/
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown("space"))
		{
			highlights.ClearAllTiles();
			roundState = mapRound.moving;
			moving = null;
			//print("End of current turn");
			if (currentTurn == attacker)
			{
				currentTurn = defender;
				turn.text = "Defender's Turn";
				message.text = "Move a unit or press \"spacebar\" to skip your turn";
				attacker.EndTurn();
				defender.BeginTurn();
			}
			else
			{
				turn.text = "Attacker's Turn";
				message.text = "Move a unit or press \"spacebar\" to skip your turn";
				currentTurn = attacker;
				attacker.BeginTurn();
				defender.EndTurn();
			}
		}
		else if (roundState == mapRound.attacking && Input.GetKeyDown("backspace"))
		{
			moving.GetComponent<Unit>().Undo();
		}
	}

	public void StartBattle() {
		Destroy(gameObject.GetComponent<SetUnitController>());
		gameObject.AddComponent<TClickDetector>();

		attacker.OnTacticalBattleStart();
		defender.OnTacticalBattleStart();

		attacker.BeginTurn();
		defender.EndTurn();
		turn.text = "Attacker's Turn";
		message.text = "Move a unit or press \"spacebar\" to skip your turn";
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
		if (moving != null) {
			moving.GetComponent<Unit>().endMove(cel);
			message.text = "Attack, press \"backspace\" to Undo or press \"spacebar\" to end your turn";
		}
    }

	public void NextTurn() {
		if (TMapController.M.currentTurn == TMapController.M.attacker) {
            TMapController.M.currentTurn = TMapController.M.defender;
			turn.text = "Defender's Turn";
		} else {
			TMapController.M.currentTurn = TMapController.M.attacker;
			turn.text = "Attacker's Turn";
		}
        TMapController.M.moving = null;
		TMapController.M.message.text = "Move a unit or press \"spacebar\" to skip your turn";
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

	public struct tileStruct
	{
		public bool land;
		public bool obstacle;
		public bool dead;
		public Army army;
	}

	public tileStruct TileHas(Vector3Int pos)
	{
		tileStruct tileHas = new tileStruct();
		if (land.HasTile(pos)) tileHas.land = true;
		if (obstacles.HasTile(pos)) tileHas.obstacle = true;
		foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
		{
			if (land.WorldToCell(unit.transform.position) == pos &&
				unit.GetComponent<Unit>().currentState == unitState.dead)
			{
				tileHas.dead = true;
				break;
			}
			if (land.WorldToCell(unit.transform.position) == pos)
			{
				tileHas.army = unit.GetComponent<Unit>().army;
				break;
			}
		}
		return tileHas;
	}

	public void SetHighlight(Vector3Int pos, char color)
	{
		if (color == 'M') highlights.SetTile(pos, moveHighlight);
		else highlights.SetTile(pos, attackHighlight);
	}

	public void ClearHighlights()
	{
		highlights.ClearAllTiles();
	}
}
