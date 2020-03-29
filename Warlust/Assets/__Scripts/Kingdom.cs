using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squadStruct {
	public int[] coordinates;
	public string[] units;

	public squadStruct(int[] coordinates, string[] units) {
		this.coordinates = coordinates;
		this.units = units;
	}
}

public class Kingdom : MonoBehaviour {
	[Header("Set Dynamically")]
	public List<squadStruct> squadrons;
	/*public List<Squad> squadrons;
	public moraleState currentMorale;
	public int armyBonus;*/

	private void Awake() {
		DontDestroyOnLoad(this.gameObject);
		//squadrons = new List<squadStruct>();
	}

    // Start is called before the first frame update
    void Start() {
		//armyBonus = 0;
		//currentMorale = moraleState.neutral;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	//This is just a temprorary function until we implement the morale system
/*	public void SetMorale(moraleState newMorale) {
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

	public void UnitDied(Squad deadUnit) {
		squadrons.Remove(deadUnit);
		if (squadrons.Count == 0)
		{
			//WMapController.M.ArmyLost(this);
			//Game over
		}
	}*/

	public void AddSquadron(squadStruct squadron) {
		if (squadrons == null) squadrons = new List<squadStruct>();
		squadrons.Add(squadron);
	}

	public void SquadDefeated(int squadID) {
		squadrons.Remove(squadrons[squadID]);
	}
}
