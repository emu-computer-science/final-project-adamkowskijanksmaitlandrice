using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{
    public enum kingdomColor { red, blue };

    [Header("Set in Inspector")]
    public kingdomColor setKingdom;
	public int townID;

    [Header("Set Dynamically")]
    public Kingdom kingdom;
    public Vector3Int currentTile;

    public Kingdom townKingdom
    {
        set
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            foreach (SpriteRenderer csr in sr.GetComponentsInChildren<SpriteRenderer>())
            {
                if (csr == sr) continue;
                if (kingdom == GameState.GS.kingdoms[0]) csr.color = Color.red;
                else csr.color = Color.blue;
            }
        }
        get { return kingdom; }
    }
    // Start is called before the first frame update
    void Start()
    {
        /*if (setKingdom == kingdomColor.red) kingdom = GameState.GS.kingdoms[0];
        else kingdom = GameState.GS.kingdoms[1];*/
		foreach (int id in GameState.GS.kingdoms[0].townIDs) {
			if (townID == id) kingdom = GameState.GS.kingdoms[0];
		}
		foreach (int id in GameState.GS.kingdoms[1].townIDs) {
			if (townID == id) kingdom = GameState.GS.kingdoms[1];
		}
        townKingdom = kingdom;
        currentTile = WMapController.M.land.WorldToCell(transform.position);
        transform.position = WMapController.M.land.CellToWorld(currentTile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Flip()
    {
        if (kingdom == WMapController.M.red)
        {
            kingdom = WMapController.M.blue;
            townKingdom = kingdom;
        }
        else
        {
            kingdom = WMapController.M.red;
            townKingdom = kingdom;
        }
    }
}
