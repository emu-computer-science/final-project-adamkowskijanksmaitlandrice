using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum moraleState {
	neutral,
	low,
	high
}

public class Morale : MonoBehaviour {
	[Header("Set in Inspector")]
	public Slider attackerBarGUI, defenderBarGUI;

	public static Morale M;

	public int attackerMorale, defenderMorale;
	private int lowMorale = 3, highMorale = 8;

    // Start is called before the first frame update
    void Start() {
        attackerMorale = 5;
		defenderMorale = 5;
		M = this;
    }

    // Update is called once per frame
    void Update() {
    }

	public void MoraleLost(Army a, int moraleLost) {
		if (a == TMapController.M.attacker) MoraleGained(TMapController.M.defender, moraleLost);
		else MoraleGained(TMapController.M.attacker, moraleLost);
	}

	public void MoraleGained(Army a, int moraleGained) {
		Army attacker = TMapController.M.attacker;
		Army defender = TMapController.M.defender;

		if (a == attacker) {
			if (attackerMorale <= lowMorale) {
				moraleGained *= 2;
			}
			attackerMorale += moraleGained;

			if (attackerMorale >= 10) {
				TMapController.M.ArmyLost(defender);
			} else if (attackerMorale >= highMorale) {
				attacker.SetMorale(moraleState.high);
			} else if (attackerMorale > lowMorale) {
				attacker.SetMorale(moraleState.high);
			}

			defenderMorale -= moraleGained;
			if (defenderMorale <= lowMorale) {
				defender.SetMorale(moraleState.low);
			} else if (defenderMorale < highMorale) {
				defender.SetMorale(moraleState.neutral);
			}
		} else {
			if (defenderMorale <= lowMorale) {
				moraleGained *= 2;
			}
			defenderMorale += moraleGained;

			if (defenderMorale >= 10) {
				TMapController.M.ArmyLost(attacker);
			} else if (defenderMorale >= highMorale) {
				defender.SetMorale(moraleState.high);
			} else if (defenderMorale > lowMorale) {
				defender.SetMorale(moraleState.high);
			}

			attackerMorale -= moraleGained;
			if (attackerMorale <= lowMorale) {
				attacker.SetMorale(moraleState.low);
			} else if (attackerMorale < highMorale) {
				attacker.SetMorale(moraleState.neutral);
			}
		}
		attackerBarGUI.value = attackerMorale;
		defenderBarGUI.value = defenderMorale;
	}
}
