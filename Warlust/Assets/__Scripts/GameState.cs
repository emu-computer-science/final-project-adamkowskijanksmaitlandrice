using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This class works! But it could still use some work.
public class GameState : MonoBehaviour {
	[Header("Set in Inspector")]
	//public GameObject kingdomPrefab;
	//public GameObject squadronPrefab;
	public GameObject armyPrefab;
	public List<GameObject> unitPrefabs;

	[Header("Set Dynamically")]
	public static GameState GS;
	public List<Kingdom> kingdoms;
	public List<GameEvent> events;
	public Kingdom currentTurn;
	public bool battleResult;

	private void Awake() {
		if (GS == null) {
			GS = this;
			// DontDestroyOnLoad(this.gameObject);
		} else {
			Destroy(this);
		}
	}

    // Start is called before the first frame update
    void Start()
    {
        events = new List<GameEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnNewGame() {
		//New game stuff here
		kingdoms = new List<Kingdom>();
		kingdoms.Add(gameObject.AddComponent<Kingdom>());
		kingdoms[0].AddSquadron(new squadStruct(new int[] {1, 0, 0}, new string[] {"archer", "archer", "warrior"}));
		kingdoms.Add(gameObject.AddComponent<Kingdom>());
		kingdoms[0].AddSquadron(new squadStruct(new int[] { 3, 0, 0 }, new string[] { "knight", "wizard", "wizard" }));
		//kingdoms[0].squadrons[0].coordinates =  new float[] {1, 0, 0};
		//kingdoms[0].squadrons[0].units = new string[] {"knight", "archer", "archer", "archer", "warrior"};
		//kingdoms[0].squadrons[0].sqArmy = kingdoms[0];
		kingdoms.Add(gameObject.AddComponent<Kingdom>());
		kingdoms[1].AddSquadron(new squadStruct(new int[] {-1, 0, 0}, new string[] {"knight", "archer", "wizard", "warrior", "warrior"}));
		//kingdoms[1].squadrons.Add(new squadStruct());
		//kingdoms[1].squadrons[0].coordinates = new float[] {-1, 0, 0};
		//kingdoms[1].squadrons[0].units = new string[] {"knight", "archer", "archer", "archer", "warrior"};
		//kingdoms[1].squadrons[0].sqArmy = kingdoms[1];*/
		SceneManager.LoadScene("World", LoadSceneMode.Single);
	}

	public void LoadGame() {
		//Load game stuff here
	}
}

public enum eventType {
	attacks,
	defeats
}

public class GameEvent {
	public Kingdom initiator;
	public int initiatorArmyID;
	public Kingdom target;
	public int targetID;
	public eventType type;

	public GameEvent(Kingdom a, int aID, Kingdom b, int bID, eventType e) {
		initiator = a;
		initiatorArmyID = aID;
		target = b;
		targetID = bID;
		type = e;
	}
}