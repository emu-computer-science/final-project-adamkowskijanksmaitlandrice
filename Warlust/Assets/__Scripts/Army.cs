using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army : MonoBehaviour {
	[Header("Set Dynamically")]
	public List<Unit> troops;
	public moraleState currentMorale;
	public int armyBonus;

    // Start is called before the first frame update
    void Start() {
		armyBonus = 0;
		currentMorale = moraleState.neutral;
    }

    // Update is called once per frame
    void Update()
    {
        
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
		foreach (Unit u in troops) {
			u.StartTurn();
		}
	}

	public void EndTurn() {
		foreach (Unit u in troops) {
			u.EndTurn();
		}
	}

	public void UnitDied(Unit deadUnit) {
		troops.Remove(deadUnit);
		if (troops.Count == 0)
			TMapController.M.ArmyLost(this);
		else Morale.M.MoraleLost(this, deadUnit.morale);
	}
}
