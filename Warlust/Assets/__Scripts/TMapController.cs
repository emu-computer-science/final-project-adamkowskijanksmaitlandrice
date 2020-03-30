using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class TMapController : MonoBehaviour
{
    public enum mapRound { moving, attacking }

    [Header("Set in Inspector")]
    public Tilemap land;
    public Tilemap obstacles;
    public Tilemap highlights;
    public TileBase moveHighlight;
    public TileBase attackHighlight;
    public GameObject archer;
    public GameObject warrior;
    public GameObject wizard;
    public GameObject attackerGameObject;
    public GameObject defenderGameObject;

    [Header("Set Dynamically")]
    public Army attacker;
    public Army defender;
    public Army currentTurn;
    public GameObject moving;
    public mapRound roundState;

    public static TMapController M;

    // Start is called before the first frame update
    void Start()
    {
        M = this;
        attacker = attackerGameObject.GetComponent<Army>();
        defender = defenderGameObject.GetComponent<Army>();
        currentTurn = attacker;
        roundState = mapRound.moving;
        moving = null;

        makeUnit(archer, 'A', 1, -2);
        makeUnit(warrior, 'A', -2, -1);
        makeUnit(wizard, 'D', 7, 1);

		attacker.BeginTurn();
		defender.EndTurn();
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown("space")) {
            highlights.ClearAllTiles();
            roundState = mapRound.moving;
            moving = null;
			print("End of current turn");
			if (currentTurn == attacker) {
				currentTurn = defender;
				attacker.EndTurn();
				defender.BeginTurn();
			}else {
				currentTurn = attacker;
				attacker.BeginTurn();
				defender.EndTurn();
			}
		}
    }

    public void makeUnit(GameObject prefab, char team, int x, int y)
    {
        GameObject unit = Instantiate(prefab);
        Unit unitScript = unit.GetComponent<Unit>();
        Vector3Int v3i = new Vector3Int(x, y, 0);
        unitScript.currentPlayerTile = v3i;
        unit.transform.position = land.CellToWorld(unitScript.currentPlayerTile);
        if (team == 'A')
        {
            attacker.troops.Add(unitScript);
            unitScript.army = attacker;
        }
        else
        {
            defender.troops.Add(unitScript);
            unitScript.army = defender;
        }
    }

    public void endMove(Vector3Int cel)
    {
        if (moving != null) moving.GetComponent<Unit>().endMove(cel);
    }

    /*
	public void UnitAttacked(Unit attackedUnit, Vector3 destTile) {
		foreach (Vector3Int v in withinRange)
        {
            if (destTile == v)
            {
                attackedUnit.TakeDamage(moving.GetComponent<Unit>().Attack());
                break;
            }
        }
        clearMove();
	}
    */

	public void ArmyLost(Army loser) {
		if (loser == attacker) {
			print("The attacker was defeated.\nThe defender has won!");
		} else {
			print("The attacker has defeated the defender!");
		}
		SceneManager.LoadScene("TacticalResultsScene", LoadSceneMode.Additive);
		SceneManager.MoveGameObjectToScene(GameObject.FindWithTag("AudioSource"), SceneManager.GetSceneByName("TacticalResultsScene"));
	}
}
