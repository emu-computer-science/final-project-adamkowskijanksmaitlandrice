using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Squad : Unit
{
	[Header("Set in Inspector")]
	public int moveRange;

    [Header("Set Dynamically")]
    public Tilemap land;
    public Vector3Int currentPlayerTile;
	public List<Unit> troops;

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
    }

    // Update is called once per frame
    void Update()
    {

    }

	public Army army {
		set 
		{
			_army = value;
			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			if (_army == TMapController.M.defender)
				sr.color = Color.blue;
		}
		get { return _army; }
	}

    public void SetPosition(Vector3Int v)
    {
        currentPlayerTile.x = v.x;
        currentPlayerTile.y = v.y;
        transform.position = land.CellToWorld(currentPlayerTile);
    }

    public void clicked()
    {
		TMapController.M.startMove(gameObject, currentPlayerTile, moveRange);
    }
}
