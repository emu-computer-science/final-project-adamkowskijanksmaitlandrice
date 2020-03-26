using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kingdom : MonoBehaviour {
	[Header("Set Dynamically")]
	public List<Squad> squadrons;
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

	public void UnitDied(Squad deadUnit) {
		squadrons.Remove(deadUnit);
		if (squadrons.Count == 0)
		{
			//WMapController.M.ArmyLost(this);
			//Game over
		}
	}
}
