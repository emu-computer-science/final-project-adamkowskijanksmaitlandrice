﻿using System.Collections;
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
    public Unit archer;
    public Unit warrior;
    public Unit wizard;
	public Unit knight;
	public GameObject armyPrefab;
    public GameObject attackerGameObject;
    public GameObject defenderGameObject;
	public GameObject canvas;
	public Text turn;
	public Text message;
	public List<Image> queueColors;
	public List<Image> queueIcons;
	public Sprite bow;
	public Sprite spear;
	public Sprite sword;
	public Sprite wand;

    [Header("Set Dynamically")]
    public Army attacker;
    public Army defender;
    public Army currentTurn;
	public Army aiArmy;
    public Unit moving;
    public mapRound roundState;
	public List<Unit> unitQueue;

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
		if (attacker.kingdom == WMapController.M.blue) {
			Canvas.FindObjectOfType<Morale>().SwapColors();
			aiArmy = defender;
		} else {
			aiArmy = attacker;
			//AIsTurn();
		}

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
			NextTurn();
		}
		/*
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
		*/
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

		//currentTurn = attacker;
		roundState = mapRound.moving;
		moving = null;

		BuildQueue();
		DisplayQueue();
		currentTurn = unitQueue[0].army;
		unitQueue[0].army.activeTroops.Add(unitQueue[0]);
		unitQueue[0].army.inactiveTroops.Remove(unitQueue[0]);
		unitQueue[0].ColorOn(true);

		currentTurn.BeginTurn();
		if (currentTurn == attacker) turn.text = "Defender's Turn";
		else turn.text = "Attacker's Turn";
		message.text = "Move a unit or press \"spacebar\" to skip your turn";
		if (unitQueue[0].army == aiArmy) Invoke("AIsTurn", 1f);
	}

	private void BuildQueue()
	{
		unitQueue = new List<Unit>();
		foreach (Unit unit in attacker.troops) unitQueue.Add(unit);
		foreach (Unit unit in defender.troops) unitQueue.Add(unit);
		for (int i = 0; i < unitQueue.Count - 1; i++)
		{
			int swap = Random.Range(i, unitQueue.Count);
			Unit temp = unitQueue[i];
			unitQueue[i] = unitQueue[swap];
			unitQueue[swap] = temp;
		}
		SortQueue();
		foreach (Unit unit in unitQueue) print(unit);
	}

	private void SortQueue()
	{
		for (int i = 0; i < unitQueue.Count - 1; i++)
			for (int j = i + 1; j < unitQueue.Count; j++)
				if (unitQueue[j].moveRange > unitQueue[i].moveRange)
				{
					Unit temp = unitQueue[i];
					unitQueue[i] = unitQueue[j];
					unitQueue[j] = temp;
				}
	}

	private void DisplayQueue()
	{
		for (int i = 0; i < unitQueue.Count && i < 9; i++)
		{
			Image icon = queueIcons[i].GetComponent<Image>();
			Image iconColor = queueColors[i].GetComponent<Image>();
			if (unitQueue[i].moveRange == 2) icon.sprite = bow;
			else if (unitQueue[i].moveRange == 3) icon.sprite = sword;
			else if (unitQueue[i].moveRange == 4) icon.sprite = wand;
			else if (unitQueue[i].moveRange == 5) icon.sprite = spear;

			if (unitQueue[i].army.kingdom == WMapController.M.blue) iconColor.color = Color.blue;
			else iconColor.color = Color.red;
			Color c = iconColor.color;
			c.a = .25f;
			iconColor.color = c;

			icon.enabled = true;
			iconColor.enabled = true;
		}
		for (int i = unitQueue.Count; i < 9; i++)
		{
			queueIcons[i].GetComponent<Image>().enabled = false;
			queueColors[i].GetComponent<Image>().enabled = false;
		}
	}

	private void AdvanceQueue()
	{
		unitQueue[0].army.activeTroops.Remove(unitQueue[0]);
		unitQueue[0].army.inactiveTroops.Add(unitQueue[0]);
		unitQueue.Add(unitQueue[0]);
		unitQueue[0].ColorOn(false);
		unitQueue.RemoveAt(0);
		unitQueue[0].army.activeTroops.Add(unitQueue[0]);
		unitQueue[0].army.inactiveTroops.Remove(unitQueue[0]);
		currentTurn = unitQueue[0].army;
		unitQueue[0].ColorOn(true);
		DisplayQueue();
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
			message.text = "Attack, press \"backspace\" to Undo, or press \"spacebar\" to end your turn";
		}
    }

	public void NextTurn() {
		highlights.ClearAllTiles();
		roundState = mapRound.moving;
		currentTurn.EndTurn();
		AdvanceQueue();
		if (currentTurn == attacker) turn.text = "Defender's Turn";
		else turn.text = "Attacker's Turn";
        moving = null;
		if (currentTurn != aiArmy)
			message.text = "Move a unit or press \"spacebar\" to skip your turn";
		currentTurn.BeginTurn();
		if (currentTurn == aiArmy) Invoke("AIsTurn", 1f);
	}

	public void AIsTurn() {
		List<Unit> enemyTroops;
		if (aiArmy == attacker) enemyTroops = defender.troops;
		else enemyTroops = attacker.troops;
		Unit currentUnit = unitQueue[0];
		int minDistance = 100000;
		Unit closestUnit = null;
		currentUnit.clicked();
		foreach (Unit enemyUnit in enemyTroops) {
			int tDist = Distance(enemyUnit.currentPlayerTile, currentUnit.currentPlayerTile);
			if (tDist < minDistance) {
				minDistance = tDist;
				closestUnit = enemyUnit;
			}
		}
		Vector3Int target = closestUnit.currentPlayerTile;
		if (currentUnit.maxAtkRange > 1) {
			int maxAtkRange = currentUnit.maxAtkRange;
			minDistance = 100000;
			Vector3Int minTarget = new Vector3Int(0,0,0);
			for (int i = 0; i < 4; i++) {
				Vector3Int tTarget = new Vector3Int(0,0,0);
				switch (i) {
					//case 0: tTarget = new Vector3Int(target.x-maxAtkRange, target.y-maxAtkRange, target.z); break;
					case 0: tTarget = new Vector3Int(target.x-maxAtkRange, target.y, target.z); break;
					case 1: tTarget = new Vector3Int(target.x, target.y-maxAtkRange, target.z); break;
					//case 3: tTarget = new Vector3Int(target.x+maxAtkRange, target.y+maxAtkRange, target.z); break;
					case 2: tTarget = new Vector3Int(target.x+maxAtkRange, target.y, target.z); break;
					case 3: tTarget = new Vector3Int(target.x, target.y+maxAtkRange, target.z); break;
				}
				int tDist = Distance(tTarget, currentUnit.currentPlayerTile);
				if (tDist < minDistance) {
					minDistance = tDist;
					minTarget = tTarget;
				}
			}
			target = minTarget;
		}
		List<Vector3Int> range = currentUnit.range;
		minDistance = 100000;
		Vector3Int destination = currentUnit.currentPlayerTile;
		foreach (Vector3Int r in range) {
			int tDist = Distance(target, r);
			if (tDist < minDistance) {
				minDistance = tDist;
				destination = r;
			}
		}
		currentUnit.endMove(destination);
		range = currentUnit.range;
		foreach (Vector3Int r in range) {
			foreach (Unit enemyUnit in enemyTroops) {
				if (enemyUnit.currentPlayerTile == r) {
					currentUnit.endMove(r);
					return;
				}
			}
		}
		Invoke("NextTurn", 1f);
	}

	private int Distance(Vector3Int a, Vector3Int b) {
		int distance = (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
		return distance;
	}

	public void ArmyLost(Army loser) {
		canvas.SetActive(false);
		if (loser == attacker) {
			print("The attacker was defeated.\nThe defender has won!");
			GameState.GS.battleResult = false;
			gEvent.initiator.SquadDefeated(gEvent.initiatorArmyID);
		} else {
			print("The attacker has defeated the defender!");
			GameState.GS.battleResult = true;
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
