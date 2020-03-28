using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Squad : MonoBehaviour
{
    [Header("Set in Inspector")]
    public int moveRange;

    [Header("Set Dynamically")]
    public Tilemap land;
    public Vector3Int currentPlayerTile;
    public List<Unit> troops;
	public Army army;

	private Kingdom _sqArmy;


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
    
	public Kingdom sqArmy {
		set 
		{
            _sqArmy = value;
			SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
			if (_sqArmy == WMapController.M.blue)
				sr.color = Color.blue;
		}
		get { return _sqArmy; }
	}

    public void SetPosition(Vector3Int v)
    {
        currentPlayerTile.x = v.x;
        currentPlayerTile.y = v.y;
        transform.position = land.CellToWorld(currentPlayerTile);
    }

    public void clicked()
    {
		WMapController.M.startMove(gameObject, currentPlayerTile, moveRange);
    }
}
