using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//I'm hoping this class will eventually be used to help us manage state.
//For now it isn't really useful though
public class GameState : MonoBehaviour {
	[Header("Set in Inspector")]
	//public GameObject kingdomPrefab;
	//public GameObject squadronPrefab;
	public GameObject armyPrefab;
	public List<GameObject> unitPrefabs;

	[Header("Set Dynamically")]
	public static GameState GS;
	//public List<Kingdom> kingdoms;
	//public List<GameEvent> events;

	private void Awake() {
		if (GS == null) {
			GS = this;
			DontDestroyOnLoad(this.gameObject);
		} else {
			Destroy(this);
		}
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnNewGame() {
		//New game stuff here
		/*kingdoms = new List<Kingdom>();
		kingdoms.Add(Instantiate(kingdomPrefab).GetComponent<Kingdom>());
		kingdoms[0].squadrons.Add(Instantiate(squadronPrefab).GetComponent<Squad>());
		kingdoms[0].squadrons[0].currentPlayerTile = new Vector3Int(1, 0, 0);
		//kingdoms[0].squadrons[0].sqArmy = kingdoms[0];
		kingdoms.Add(Instantiate(kingdomPrefab).GetComponent<Kingdom>());
		kingdoms[1].squadrons.Add(Instantiate(squadronPrefab).GetComponent<Squad>());
		kingdoms[1].squadrons[0].currentPlayerTile = new Vector3Int(-1, 0, 0);
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

public struct GameEvent {
	public Army initiator;
	public Army target;
	public eventType type;

	public GameEvent(Army a, Army b, eventType e) {
		initiator = a;
		target = b;
		type = e;
	}
}