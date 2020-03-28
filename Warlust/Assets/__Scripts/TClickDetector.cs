using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TClickDetector : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Tilemap land;

    // Start is called before the first frame update
    void Start()
    {
		land = TMapController.M.land;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 10;
            pos = Camera.main.ScreenToWorldPoint(pos);
            Vector3Int cel = land.WorldToCell(pos);
            pos = land.CellToWorld(cel);

			foreach (Unit unit in TMapController.M.currentTurn.troops) {
				if (unit.gameObject.transform.position == pos) {
					unit.clicked();
					return;
				}
			}
            TMapController.M.endMove(cel);
        }
    }


}
