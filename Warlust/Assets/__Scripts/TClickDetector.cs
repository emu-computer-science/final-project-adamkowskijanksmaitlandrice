using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TClickDetector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 10;
            pos = Camera.main.ScreenToWorldPoint(pos);
            Vector3Int cel = TMapController.M.land.WorldToCell(pos);
            pos = TMapController.M.land.CellToWorld(cel);

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
