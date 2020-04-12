using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Squad : MonoBehaviour
{
    [Header("Set in Inspector")]
    public int moveRange;
    public Kingdom kingdom;

    [Header("Set Dynamically")]
    public Vector3Int currentPlayerTile;
    public List<Unit> troops;
	public Army army;

	private Kingdom _sqKingdom;
	private int _ID;


    void Start()
    {
        sqKingdom = kingdom;
        currentPlayerTile = WMapController.M.land.WorldToCell(transform.position);
        transform.position = WMapController.M.land.CellToWorld(currentPlayerTile);
    }

    // Update is called once per frame
    void Update()
    {

    }

	public int ID {
		set {_ID = value;}
		get {return _ID;}
	}

	public Kingdom sqKingdom {
		set 
		{
            kingdom = value;
            _sqKingdom = kingdom;
			SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
			if (_sqKingdom == WMapController.M.blue)
				sr.color = Color.blue;
		}
		get { return _sqKingdom; }
	}

    public void SetPosition(Vector3Int v)
    {
        currentPlayerTile.x = v.x;
        currentPlayerTile.y = v.y;
		_sqKingdom.squadrons[_ID].coordinates = new int[] {v.x, v.y, v.z};
        transform.position = WMapController.M.land.CellToWorld(currentPlayerTile);
    }

    public void clicked()
    {
		WMapController.M.startMove(gameObject, currentPlayerTile, moveRange);
    }
}
