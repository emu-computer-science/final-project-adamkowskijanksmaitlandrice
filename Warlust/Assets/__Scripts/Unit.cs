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
	public int minAtkRange;
	public int maxAtkRange;
	public int atkSplash;
	public int defense;
	public int attack = 5; //How much damage the unit deals
	public int morale = 1;

    [Header("Set Dynamically")]
    public Tilemap land;
    public Vector3Int currentPlayerTile;
	public unitState currentState;

	private Army _army;

    void Start()
    {
        Tilemap[] tms = GameObject.FindObjectsOfType<Tilemap>();
        foreach (Tilemap tm in tms) if (tm.name == "Land")
            {
                land = tm;
                break;
            }
        currentPlayerTile = land.WorldToCell(transform.position);
        transform.position = land.CellToWorld(currentPlayerTile);

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
        transform.position = land.CellToWorld(currentPlayerTile);
		currentState = unitState.moved;
    }

    public void clicked()
    {
		switch (currentState) {
			case unitState.idle:
			if (TMapController.M.currentTurn == _army)
				TMapController.M.startMove(gameObject, currentPlayerTile, moveRange);
			else TMapController.M.UnitAttacked(this, currentPlayerTile);
			break;
			case unitState.moved:
			TMapController.M.StartAttack(gameObject, currentPlayerTile, minAtkRange, maxAtkRange);
			break;
			/*case unitState.attacked:
			print("About to be attacked!");
			TMapController.M.UnitAttacked(this, currentPlayerTile);
			break;
			default:
			break;*/
		}
    }

	public int Attack() {
		//currentState = unitState.attacked;
		return Random.Range(0, attack);
	}
	
	public bool TakeDamage(int damage) {
		if (damage > defense) {
			currentState = unitState.dead;
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
