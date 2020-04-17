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
	public List<int> townIDs;
	/*public List<Squad> squadrons;
	public moraleState currentMorale;
	public int armyBonus;*/

	private void Awake() {
		DontDestroyOnLoad(this.gameObject);
		//squadrons = new List<squadStruct>();
	}

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void AddSquadron(squadStruct squadron) {
		if (squadrons == null) squadrons = new List<squadStruct>();
		squadrons.Add(squadron);
	}

	public void SquadDefeated(int squadID) {
		squadrons.Remove(squadrons[squadID]);
	}
}
