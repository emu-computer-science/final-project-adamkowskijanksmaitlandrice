using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class UnitDescription {
	public unitType type;
	public Vector3Int coordinates;
}*/
/*public enum unitType {
	warrior,
	archer,
	knight,
	wizard
}*/

public class Army : MonoBehaviour {
	[Header("Set in Inspector")]
	public GameObject warriorPrefab;
	public GameObject archerPrefab;
	public GameObject knightPrefab;
	public GameObject wizardPrefab;
	//public List<string> unitDescriptions;
	public string[] unitDescriptions;

	//public List<UnitDescription> descriptions = new List<UnitDescription>();

	[Header("Set Dynamically")]
	public Kingdom kingdom;
	public List<Unit> troops;
	public moraleState currentMorale;
	public int armyBonus;
	public List<Unit> activeTroops;
	public List<Unit> inactiveTroops;

	private void Awake() {
		//DontDestroyOnLoad(this.gameObject);
		troops = new List<Unit>();
		activeTroops = new List<Unit>();
		inactiveTroops = new List<Unit>();
	}

    // Start is called before the first frame update
    void Start() 
	{

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnTacticalBattleStart() {
		/*foreach (UnitDescription description in descriptions) {
			MakeUnit(description);
		}*/
		foreach (Unit unit in troops) unit.ColorOn(false);
		armyBonus = 0;
		currentMorale = moraleState.neutral;
	}

	 /*public void MakeUnit(UnitDescription description) {
		GameObject prefab = warriorPrefab;

		switch (description.type) {
			case unitType.warrior:
			prefab = warriorPrefab;
			break;
			case unitType.archer:
			prefab = archerPrefab;
			break;
			case unitType.knight:
			prefab = knightPrefab;
			break;
			case unitType.wizard:
			prefab = wizardPrefab;
			break;
		}

        GameObject unit	= Instantiate(prefab);
        Unit unitScript = unit.GetComponent<Unit>();
        //Vector3Int v3i = new Vector3Int(x, y, 0);
        unitScript.currentPlayerTile = description.coordinates;
		TMapController.M.PlaceUnit(unit);
        unitScript.army = this;
		troops.Add(unitScript);
    }*/
	public void SetUnit(int index, Vector3Int cel) {
		GameObject prefab = warriorPrefab;

		switch (unitDescriptions[index]) {
			case "warrior":
			prefab = warriorPrefab;
			break;
			case "archer":
			prefab = archerPrefab;
			break;
			case "knight":
			prefab = knightPrefab;
			break;
			case "wizard":
			prefab = wizardPrefab;
			break;
		}

		GameObject unit	= Instantiate(prefab);
        Unit unitScript = unit.GetComponent<Unit>();
        //Vector3Int v3i = new Vector3Int(x, y, 0);
        unitScript.currentPlayerTile = cel;
		TMapController.M.PlaceUnit(unit);
        unitScript.army = this;
		troops.Add(unitScript);
		inactiveTroops.Add(unitScript);
		unitScript.ColorOn(true);
	}

	//This is just a temprorary function until we implement the morale system
	public void SetMorale(moraleState newMorale) {
		if (currentMorale != newMorale) {
			if (newMorale == moraleState.high) {
				armyBonus += 1;
			} else if (newMorale == moraleState.low || currentMorale == moraleState.high) {
				armyBonus -= 1;
			//} else if (currentMorale == moraleState.high) {
				//take away high morale bonus
			} else {
				armyBonus += 1;
			}
			currentMorale = newMorale;
		}
	}

	public void BeginTurn() {
		/*
		if (activeTroops.Count == 0) {
			List<Unit> temp = activeTroops;
			activeTroops = inactiveTroops;
			inactiveTroops = temp;
			foreach (Unit u in activeTroops) u.ColorOn(true);
		}
		*/
		foreach (Unit u in activeTroops) {
			u.StartTurn();
		}
	}

	public void EndTurn() {
		foreach (Unit u in troops) {
			u.EndTurn();
		}
	}

	public void UnitMoved(Unit movedUnit) {
		activeTroops.Remove(movedUnit);
		inactiveTroops.Add(movedUnit);
		movedUnit.ColorOn(false);
	}

	public void UndoMove(Unit movedUnit) {
		inactiveTroops.Remove(movedUnit);
		activeTroops.Add(movedUnit);
		movedUnit.ColorOn(true);
	}

	public void UnitDied(Unit deadUnit) {
		troops.Remove(deadUnit);
		activeTroops.Remove(deadUnit);
		inactiveTroops.Remove(deadUnit);
		TMapController.M.unitQueue.Remove(deadUnit);
		if (troops.Count == 0)
			TMapController.M.ArmyLost(this);
		else Morale.M.MoraleLost(this, deadUnit.morale);
	}
}
