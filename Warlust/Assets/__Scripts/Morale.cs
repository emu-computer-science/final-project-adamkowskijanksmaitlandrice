using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        attackerBarGUI.value = attackerMorale;
		defenderBarGUI.value = defenderMorale;
    }

	public void ChangeMorale(Army a, int moraleGained) {
		Army attacker = TMapController.M.attacker;
		Army defender = TMapController.M.defender;
		if (a == attacker) {
			if (attackerMorale <= lowMorale) {
				moraleGained *= 2;
			}
			attackerMorale += moraleGained;
			if (attackerMorale > highMorale)
				attacker.SetMorale();
			defenderMorale -= moraleGained;
		} else {
			if (defenderMorale <= lowMorale) {
				moraleGained *= 2;
			}
			defenderMorale += moraleGained;
			if (attackerMorale > highMorale)
				defender.SetMorale();
			attackerMorale -= moraleGained;
		}
	}
}
