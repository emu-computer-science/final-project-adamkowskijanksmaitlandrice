using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army : MonoBehaviour {
	[Header("Set Dynamically")]
	public List<Unit> troops;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	//This is just a temprorary function until we implement the morale system
	public void SetMorale() {}

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
			MapController.M.ArmyLost(this);
	}
}
