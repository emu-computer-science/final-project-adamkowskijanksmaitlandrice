using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
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

    private GameObject moving;
    public static MapController M;
    private List<Vector3Int> withinRange;
    private List<Vector3Int> excludeRange;
    public mapRound roundState;

    // Start is called before the first frame update
    void Start()
    {
        M = this;
        attacker = attackerGameObject.GetComponent<Army>();
        defender = defenderGameObject.GetComponent<Army>();
        currentTurn = attacker;
        roundState = mapRound.moving;

        makeUnit(archer, 'A', 1, -2);
        makeUnit(warrior, 'A', -2, -1);
        makeUnit(wizard, 'D', 2, 0);

		attacker.BeginTurn();
		defender.EndTurn();
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown("space")) {
			clearMove();
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

    public void clearMove()
    {
        highlights.ClearAllTiles();
        withinRange = new List<Vector3Int>();
        excludeRange = new List<Vector3Int>();
    }

    public void startMove(GameObject toMove, Vector3Int currentTile, int moveRange)
    {
        if (roundState != mapRound.moving)
        {
            print("roundState != mapRound.moving");
            return;
        }
        if (toMove.GetComponent<Unit>().currentState != unitState.idle)
        {
            print("toMove.GetComponent<Unit>().currentState != unitState.idle");
            return;
        }

        print("About to start move");
        clearMove();
        moving = toMove;
        List<Vector3Int> queue = new List<Vector3Int>() { currentTile };
        for (int i = 0; i < moveRange; i++)
            queue = updateQueue(queue, 'M', true);
        foreach (Vector3Int v in withinRange)
            highlights.SetTile(v, moveHighlight);
    }

	public void StartAttack(GameObject toAttack, Vector3Int currentTile, int minAtkRange, int maxAtkRange)
    {
        if (roundState != mapRound.attacking)
        {
            print("roundState != mapRound.attacking");
            return;
        }
        if (toAttack.GetComponent<Unit>().currentState != unitState.moved)
        {
            print("toAttack.GetComponent<Unit>().currentState != unitState.moved");
            return;
        }

        print("About to start attack");
        clearMove();
        List<Vector3Int> queue = new List<Vector3Int>() { currentTile };
        for (int i = 0; i < minAtkRange - 1; i++)
            queue = updateQueue(queue, 'A', false);
        for (int i = 0; i < maxAtkRange - minAtkRange + 1; i++)
            queue = updateQueue(queue, 'A', true);
        foreach (Vector3Int v in withinRange)
            highlights.SetTile(v, attackHighlight);
    }

    public List<Vector3Int> updateQueue(List<Vector3Int> queue, char mode, bool include)
    {
        List<Vector3Int> queue2 = new List<Vector3Int>();
        while (queue.Count > 0)
        {
            Vector3Int check = queue[0];
            int x = check.x, y = check.y, z = check.z;
            queue.Remove(check);
            int row = Mathf.Abs(check.y % 2);
            Vector3Int left = new Vector3Int(x-1, y, z);
            Vector3Int right = new Vector3Int(x+1, y, z);
            Vector3Int upLeft = new Vector3Int(x-1+row, y+1, z);
            Vector3Int upRight = new Vector3Int(x+row, y+1, z);
            Vector3Int downLeft = new Vector3Int(x-1+row, y-1, z);
            Vector3Int downRight = new Vector3Int(x+row, y-1, z);
            foreach (Vector3Int dir in new Vector3Int[] {
                left, right, upLeft, upRight, downLeft, downRight})
            {
                if (obstacles.HasTile(dir)) continue;

                if (land.HasTile(dir) && !withinRange.Contains(dir) && !excludeRange.Contains(dir))
                {
                    bool OK = true;
                    foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                    { 
                        if (mode == 'M')
                        {
                            if (land.WorldToCell(unit.transform.position) == dir)
                            {
                                OK = false;
                                break;
                            }
                        }
                        else
                                if (land.WorldToCell(unit.transform.position) == dir &&
                                    unit.GetComponent<Unit>().army == currentTurn)
                        {
                            OK = false;
                            break;
                        }
                    }
                    if (OK)
                    {
                        if (include) withinRange.Add(dir);
                        else excludeRange.Add(dir);
                    }
                    queue2.Add(dir);
                }
            }
        }
        queue = queue2;
        return queue;
    }

    public void endMove(Vector3 destTile)
    {
        if (moving == null)
        {
            print("moving == null | "+ currentTurn +" | "+ roundState);
            return;
        }

        switch (moving.GetComponent<Unit>().currentState) 
        {
            case unitState.idle:
                foreach (Vector3Int v in withinRange)
                    if (destTile == v)
                    {
                        moving.GetComponent<Unit>().SetPosition(v);
                        moving.GetComponent<Unit>().currentState = unitState.moved;
                        roundState = mapRound.attacking;
                        break;
                    }
                break;
            case unitState.moved:
                foreach (Vector3Int v in withinRange)
                    if (destTile == v)
                        //foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                            //if (land.WorldToCell(unit.transform.position) == v)
                            {
                                //print(unit.GetComponent<Unit>().TakeDamage(moving.GetComponent<Unit>().Attack()));
                                moving.GetComponent<Unit>().currentState = unitState.idle;
                                roundState = mapRound.moving;
                                if (currentTurn == attacker) currentTurn = defender;
                                else currentTurn = attacker;
                                moving = null;
                                break;
                            }
                break;
        }
        clearMove();
    }

	public void UnitAttacked(Unit attackedUnit, Vector3 destTile) {
		foreach (Vector3Int v in withinRange)
        {
            if (destTile == v)
            {
				/*if (moving.GetComponent<Unit>().currentState == unitState.moved) {
					foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                        {
                            if (land.WorldToCell(unit.transform.position) == v)
                            {
                                print(unit.GetComponent<Unit>().TakeDamage(moving.GetComponent<Unit>().Attack()));
                                break;
                            }
                        }
				} else {*/
                attackedUnit.TakeDamage(moving.GetComponent<Unit>().Attack());
				//}
                break;
            }
        }
        clearMove();
	}

	public void ArmyLost(Army loser) {
		if (loser == attacker) {
			print("The attacker was defeated.\nThe defender has won!");
		} else {
			print("The attacker has defeated the defender!");
		}
	}
}
