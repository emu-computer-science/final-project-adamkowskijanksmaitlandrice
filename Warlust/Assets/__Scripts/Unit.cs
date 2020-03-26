using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum unitState {
	idle,
	moved,
	//attacked,
	//dead
}

public class Unit : MonoBehaviour
{
	[Header("Set in Inspector")]
	public int moveRange;
    public bool flying;
	public int minAtkRange;
	public int maxAtkRange;
	public int atkSplash;
	public int defense;
	public int attack = 5; //How much damage the unit deals
	public int morale = 1;

    [Header("Set Dynamically")]
    public Vector3Int currentPlayerTile;
	public unitState currentState;

	private Army _army;
    private List<Vector3Int> withinRange;
    private List<Vector3Int> excludeRange;

    void Start()
    {
        currentPlayerTile = TMapController.M.land.WorldToCell(transform.position);
        transform.position = TMapController.M.land.CellToWorld(currentPlayerTile);

		currentState = unitState.idle;
    }

    // Update is called once per frame
    void Update()
    {

    }

	public Army army {
		set {
			_army = value;
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			foreach (SpriteRenderer csr in sr.GetComponentsInChildren<SpriteRenderer>())
			{
				if (csr == sr) continue;
				if (_army == TMapController.M.defender)
				{
					sr.flipX = !sr.flipX;
					csr.color = Color.blue;
				}
			}
		}
		get {
			return _army;
		}
	}

    public void SetPosition(Vector3Int v)
    {
        currentPlayerTile.x = v.x;
        currentPlayerTile.y = v.y;
        transform.position = TMapController.M.land.CellToWorld(currentPlayerTile);
		currentState = unitState.moved;
    }

    public void clicked()
    {
		switch (currentState) {
			case unitState.idle:
			if (TMapController.M.moving == gameObject)
				endMove(currentPlayerTile);
			else if (TMapController.M.currentTurn == _army)
				startMove(gameObject, currentPlayerTile, moveRange);
			//else TMapController.M.UnitAttacked(this, currentPlayerTile);
			break;
			//case unitState.moved:
			//startAttack(gameObject, currentPlayerTile, minAtkRange, maxAtkRange);
			//break;
		}
    }

    private void clearMove()
    {
        TMapController.M.highlights.ClearAllTiles();
        withinRange = new List<Vector3Int>();
        excludeRange = new List<Vector3Int>();
    }

    private void startMove(GameObject toMove, Vector3Int currentTile, int moveRange)
    {
        if (TMapController.M.roundState != TMapController.mapRound.moving)
        {
            //print("roundState != mapRound.moving");
            return;
        }
        if (toMove.GetComponent<Unit>().currentState != unitState.idle)
        {
            //print("toMove.GetComponent<Unit>().currentState != unitState.idle");
            return;
        }

        //print("About to start move");
        clearMove();
        TMapController.M.moving = toMove;
		withinRange.Add(currentTile);
        List<Vector3Int> queue = new List<Vector3Int>() { currentTile };
        for (int i = 0; i < moveRange; i++)
            queue = updateQueue(queue, 'M', true);
        foreach (Vector3Int v in withinRange)
            TMapController.M.highlights.SetTile(v, TMapController.M.moveHighlight);
    }

    private void startAttack(GameObject toAttack, Vector3Int currentTile, int minAtkRange, int maxAtkRange)
    {
        if (TMapController.M.roundState != TMapController.mapRound.attacking)
        {
            //print("roundState != mapRound.attacking");
            return;
        }
        if (toAttack.GetComponent<Unit>().currentState != unitState.moved)
        {
            //print("toAttack.GetComponent<Unit>().currentState != unitState.moved");
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
            TMapController.M.highlights.SetTile(v, TMapController.M.attackHighlight);
    }

    private List<Vector3Int> updateQueue(List<Vector3Int> queue, char mode, bool include)
    {
        List<Vector3Int> queue2 = new List<Vector3Int>();
        while (queue.Count > 0)
        {
            Vector3Int check = queue[0];
            int x = check.x, y = check.y, z = check.z;
            queue.Remove(check);
            int row = Mathf.Abs(check.y % 2);
            Vector3Int left = new Vector3Int(x - 1, y, z);
            Vector3Int right = new Vector3Int(x + 1, y, z);
            Vector3Int upLeft = new Vector3Int(x - 1 + row, y + 1, z);
            Vector3Int upRight = new Vector3Int(x + row, y + 1, z);
            Vector3Int downLeft = new Vector3Int(x - 1 + row, y - 1, z);
            Vector3Int downRight = new Vector3Int(x + row, y - 1, z);
            foreach (Vector3Int dir in new Vector3Int[] {
                left, right, upLeft, upRight, downLeft, downRight})
            {
                bool canPass = ((TMapController.M.land.HasTile(dir) && 
                                 !TMapController.M.obstacles.HasTile(dir)) || 
                                flying == true || 
                                mode == 'A');
                if (!canPass) continue;
                if (mode == 'M' && flying == false)
                {
                    bool blocked = false;
                    foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                    {
                        if (TMapController.M.land.WorldToCell(unit.transform.position) == dir &&
                            unit.GetComponent<Unit>().army != TMapController.M.currentTurn)
                        {
                            blocked = true;
                            break;
                        }
                    }
                    if (blocked) continue;
                }

                if (TMapController.M.land.HasTile(dir) && 
                    !TMapController.M.obstacles.HasTile(dir) && 
                    !withinRange.Contains(dir) && 
                    !excludeRange.Contains(dir))
                {
                    bool OK = true;
                    foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                    {
                        if (mode == 'M')
                        {
                            if (TMapController.M.land.WorldToCell(unit.transform.position) == dir)
                            {
                                OK = false;
                                break;
                            }
                        }
                        else if (TMapController.M.land.WorldToCell(unit.transform.position) == dir &&
                                 unit.GetComponent<Unit>().army == TMapController.M.currentTurn)
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
                }
                queue2.Add(dir);
            }
        }
        queue = queue2;
        return queue;
    }

    public void endMove(Vector3 destTile)
    {
        if (TMapController.M.moving == null)
        {
            //print("moving == null | "+ currentTurn +" | "+ roundState);
            return;
        }

        switch (TMapController.M.moving.GetComponent<Unit>().currentState)
        {
            case unitState.idle:
                foreach (Vector3Int v in withinRange)
                    if (destTile == v)
                    {
                        TMapController.M.moving.GetComponent<Unit>().SetPosition(v);
                        TMapController.M.moving.GetComponent<Unit>().currentState = unitState.moved;
                        TMapController.M.roundState = TMapController.mapRound.attacking;
						clearMove();
						TMapController.M.moving.GetComponent<Unit>().startAttack(gameObject, currentPlayerTile, minAtkRange, maxAtkRange);
                        break;
                    }
                break;
            case unitState.moved:
                foreach (Vector3Int v in withinRange)
                    if (destTile == v)
                        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                            if (TMapController.M.land.WorldToCell(unit.transform.position) == v)
                            {
                                int dmg = TMapController.M.moving.GetComponent<Unit>().Attack();
                                print("damage: " + dmg);
                                print("killed: " + unit.GetComponent<Unit>().TakeDamage(dmg));
                                TMapController.M.moving.GetComponent<Unit>().currentState = unitState.idle;
                                TMapController.M.roundState = TMapController.mapRound.moving;
                                if (TMapController.M.currentTurn == TMapController.M.attacker)
                                    TMapController.M.currentTurn = TMapController.M.defender;
                                else TMapController.M.currentTurn = TMapController.M.attacker;
                                TMapController.M.moving = null;
								clearMove();
                                break;
                            }
                break;
        }
    }

    private int Attack() {
		return Random.Range(1, attack) + _army.armyBonus;
	}
    
    private bool TakeDamage(int damage) {
		if (damage > (defense + _army.armyBonus)) {
			//currentState = unitState.dead;
			Destroy(gameObject);
			_army.UnitDied(this);
			return true;
		}
		return false;
	}

	public void StartTurn() {
		currentState = unitState.idle;
	}

	public void EndTurn() {
		currentState = unitState.idle;
	}
}
