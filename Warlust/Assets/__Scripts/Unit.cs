﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	public int attackMin;
    public int attackMax;
    public int maxHP;
	public int morale = 1;
	public Sprite skullSprite;

    [Header("Set Dynamically")]
    public Vector3Int currentPlayerTile;
	public unitState currentState;
    public int hitpoints;

	private Army _army;
    private List<Vector3Int> withinRange;
    private List<Vector3Int> excludeRange;
    private Vector3Int previousTile;

	public List<Vector3Int> range {
		get {return withinRange;}
	}

    void Start()
    {
        currentPlayerTile = TMapController.M.land.WorldToCell(transform.position);
        transform.position = TMapController.M.land.CellToWorld(currentPlayerTile);
        hitpoints = maxHP;
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
			if (TMapController.M.moving == this)
				endMove(currentPlayerTile);
			else if (TMapController.M.currentTurn == _army)
				startMove(this, currentPlayerTile, moveRange);
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

    private void startMove(Unit toMove, Vector3Int currentTile, int moveRange)
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

    public void endMove(Vector3Int destTile)
    {
        if (TMapController.M.moving == null)
        {
            //print("moving == null | "+ currentTurn +" | "+ roundState);
            return;
        }

        switch (currentState)
        {
            case unitState.idle:
                if (withinRange.Contains(destTile))
                {
                    previousTile = currentPlayerTile;
                    SetPosition(destTile);
                    currentState = unitState.moved;
                    TMapController.M.roundState = TMapController.mapRound.attacking;
					clearMove();
					startAttack(gameObject, currentPlayerTile, minAtkRange, maxAtkRange);
					_army.UnitMoved(this);
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
                                else unit.GetComponent<HealthDisplay>().ShowDmg("Miss!");
                                TMapController.M.moving.GetComponent<Unit>().currentState = unitState.idle;
                                TMapController.M.roundState = TMapController.mapRound.moving;
								clearMove();
								TMapController.M.NextTurn();
                                break;
                            }
                break;
        }
    }

    public void Undo()
    {
        SetPosition(previousTile);
		_army.UndoMove(this);
        TMapController.M.moving = null;
        currentState = unitState.idle;
        TMapController.M.roundState = TMapController.mapRound.moving;
        TMapController.M.ClearHighlights();
    }

    public int attackRoll {
		get {return Random.Range(attackMin, attackMax + 1) +_army.armyBonus;}
	}

	//This method returns true if an attack hit
    public bool AttackHit(int attackRoll) {
		return attackRoll > defense;
	}

	//This method returns true if the unit was killed by an attack
    public bool TakeDamage(int damage) {
		hitpoints -= damage - defense;
        print("hit for " + damage + " (- " + defense + ")");
        gameObject.GetComponentInChildren<Slider>().value = (float) hitpoints / maxHP;
        gameObject.GetComponent<HealthDisplay>().ShowDmg((damage - defense).ToString());
		if (hitpoints <= 0) { //+ _army.armyBonus)) {
            print("Unit killed!");
			currentState = unitState.dead;
			//Destroy(gameObject);
			 gameObject.GetComponent<SpriteRenderer>().sprite = skullSprite;
            if (gameObject.transform.localScale.x == 2) RescaleDeadKnight();
            _army.UnitDied(this);
            ColorOn(false);
			return true;
		}
		return false;
	}

	public void StartTurn() {
		currentState = unitState.idle;
        previousTile = currentPlayerTile;
	}

	public void EndTurn() {
		currentState = unitState.idle;
	}

    public void ColorOn(bool colorOn)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        foreach (SpriteRenderer csr in sr.GetComponentsInChildren<SpriteRenderer>())
        {
            if (csr == sr) continue;
            Color c;
            if (_army.kingdom == WMapController.M.blue)
            {
                if (colorOn) c = Color.blue;
                else c = Color.Lerp(Color.blue, Color.white, .75f);
            }
            else
            {
                if (colorOn) c = Color.red;
                else c = Color.Lerp(Color.red, Color.white, .75f);
            }
            csr.color = c;
        }
    }

    private void RescaleDeadKnight()
    {
        Vector3 spriteScale = new Vector3(1, 1, 1);
        Vector3 colorScale = new Vector3(2, .75f, 1);
        Vector3 colorPos = new Vector3(0, -.25f, 0);
        Vector3 canvasScale = new Vector3(.1f, .1f, 1);
        gameObject.transform.localScale = spriteScale;
        Transform tc = gameObject.FindComponentInChildWithTag<Transform>("TeamColor");
        tc.localScale = colorScale;
        tc.localPosition = colorPos;
        Transform cv = gameObject.FindComponentInChildWithTag<Transform>("Canvas");
        cv.localScale = canvasScale;
    }
}

public static class Helper
{
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
    {
        Transform t = parent.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }
}
