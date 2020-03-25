using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WMapController : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Tilemap land;
    public Tilemap obstacles;
    public Tilemap highlights;
    public TileBase moveHighlight;
    public GameObject squadron;
    public GameObject attackerGameObject;
    public GameObject defenderGameObject;

    [Header("Set Dynamically")]
    public Squad attacker;
    public Squad defender;
    public Squad currentTurn;

    private GameObject moving;
    public static WMapController M;
    private List<Vector3Int> withinRange;
    private List<Vector3Int> excludeRange;

    // Start is called before the first frame update
    void Start()
    {
        M = this;
        attacker = attackerGameObject.GetComponent<Squad>();
        defender = defenderGameObject.GetComponent<Squad>();
        currentTurn = attacker;

        makeSquad(squadron, 'A', -1, 0);
        makeSquad(squadron, 'D', 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown("space")) {
			clearMove();
			print("End of current turn");
			if (currentTurn == attacker) currentTurn = defender;
			else  currentTurn = attacker;
        }
    }

    public void makeSquad(GameObject prefab, char team, int x, int y)
    {
        GameObject squad = Instantiate(prefab);
        Squad squadScript = squad.GetComponent<Squad>();
        Vector3Int v3i = new Vector3Int(x, y, 0);
        squadScript.currentPlayerTile = v3i;
        squad.transform.position = land.CellToWorld(squadScript.currentPlayerTile);
        if (team == 'A')
        {
            attacker.troops.Add(squadScript);
            squadScript.sqArmy = attacker;
        }
        else
        {
            defender.troops.Add(squadScript);
            squadScript.sqArmy = defender;
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
        print("About to start move");
        clearMove();
        moving = toMove;
        excludeRange.Add(currentTile);
        List<Vector3Int> queue = new List<Vector3Int>() { currentTile };
        for (int i = 0; i < moveRange; i++)
            queue = updateQueue(queue);
        foreach (Vector3Int v in withinRange)
            highlights.SetTile(v, moveHighlight);
    }

    public List<Vector3Int> updateQueue(List<Vector3Int> queue)
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
                if (obstacles.HasTile(dir) || !land.HasTile(dir)) continue;
                if (!withinRange.Contains(dir) && !excludeRange.Contains(dir)) withinRange.Add(dir);
                queue2.Add(dir);
            }
        }
        queue = queue2;
        return queue;
    }

    public void endMove(Vector3 destTile)
    {
        if (moving == null)
        {
            //print("moving == null | "+ currentTurn +" | "+ roundState);
            return;
        }

        foreach (Vector3Int v in withinRange)
        {
            if (destTile == v)
            {
                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                {
                    if (land.WorldToCell(unit.transform.position) == v &&
                        unit.GetComponent<Squad>().sqArmy != currentTurn)
                    {
                        print("Attack!");
                        // do the actual stuff here...
                        break;
                    }
                }
                moving.GetComponent<Squad>().SetPosition(v);
                if (currentTurn == attacker) currentTurn = defender;
                else currentTurn = attacker;
                moving = null;
                break;
            }
        }
        clearMove();
    }

    public void UnitAttacked(Unit attackedUnit, Vector3 destTile)
    {
        foreach (Vector3Int v in withinRange)
        {
            if (destTile == v)
            {
                if (moving.GetComponent<Unit>().currentState == unitState.moved)
                {
                    foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                    {
                        if (land.WorldToCell(unit.transform.position) == v)
                        {
                            print(unit.GetComponent<Unit>().TakeDamage(moving.GetComponent<Unit>().Attack()));
                            break;
                        }
                    }
                }
                else
                {
                    attackedUnit.TakeDamage(moving.GetComponent<Unit>().Attack());
                    //}
                    break;
                }
                clearMove();
            }
        }
    }

	public void ArmyLost(Army loser) 
    {
		if (loser == attacker) 
        {
			print("The attacker was defeated.\nThe defender has won!");
		} 
            else {
			print("The attacker has defeated the defender!");
		}
	}
}
