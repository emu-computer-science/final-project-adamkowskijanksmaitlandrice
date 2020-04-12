using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum unitState {
	idle,
	moved,
	//attacked,
	dead
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
    public int hitpoints;
	public int morale = 1;
	public Sprite skullSprite;

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
					sr.flipX = !sr.flipX;
                if (_army.kingdom == WMapController.M.blue)
                    csr.color = Color.blue;
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
        TMapController.M.ClearHighlights();
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
            TMapController.M.SetHighlight(v, 'M');
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

        //print("About to start attack");
        clearMove();
        List<Vector3Int> queue = new List<Vector3Int>() { currentTile };
        for (int i = 0; i < minAtkRange - 1; i++)
            queue = updateQueue(queue, 'A', false);
        for (int i = 0; i < maxAtkRange - minAtkRange + 1; i++)
            queue = updateQueue(queue, 'A', true);
        foreach (Vector3Int v in withinRange)
            TMapController.M.SetHighlight(v, 'A');
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
                TMapController.tileStruct tileHas = TMapController.M.TileHas(dir);

                if (!((tileHas.land && !tileHas.obstacle) || flying == true || mode == 'A')) 
                    continue;

                if (mode == 'M' && flying == false && tileHas.army != null && tileHas.army != army) 
                    continue;

                if (tileHas.land && !tileHas.obstacle && 
                    !withinRange.Contains(dir) && !excludeRange.Contains(dir))
                    if (!((mode == 'M') && (tileHas.army != null || tileHas.dead) ||
                          (tileHas.army == army || tileHas.dead)))
                    {
                        if (include) withinRange.Add(dir);
                        else excludeRange.Add(dir);
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
                                //int dmg = TMapController.M.moving.GetComponent<Unit>().Attack();
                                //print("damage: " + dmg);
                                //print("killed: " + unit.GetComponent<Unit>().TakeDamage(dmg));
								//unit.GetComponent<Unit>().TakeDamage(dmg);
								int dmg = this.attackRoll;
                                if (unit.GetComponent<Unit>().AttackHit(dmg))
                                    unit.GetComponent<Unit>().TakeDamage(dmg);
                                else print("Miss!");
                                TMapController.M.moving.GetComponent<Unit>().currentState = unitState.idle;
                                TMapController.M.roundState = TMapController.mapRound.moving;
								clearMove();
								TMapController.M.NextTurn();
                                break;
                            }
                break;
        }
    }

    public int attackRoll {
		get {return Random.Range(0, attack) + 1 +_army.armyBonus;}
	}

	//This method returns true if an attack hit
    public bool AttackHit(int attackRoll) {
		return attackRoll > defense;
	}

	//This method returns true if the unit was killed by an attack
    public bool TakeDamage(int damage) {
		hitpoints -= damage - defense;
        print("hit for " + damage + " (- " + defense + ")");
		if (hitpoints <= 0) { //+ _army.armyBonus)) {
            print("Unit killed!");
			currentState = unitState.dead;
			//Destroy(gameObject);
			 gameObject.GetComponent<SpriteRenderer>().sprite = skullSprite;
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
