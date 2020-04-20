using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SetUnitController : MonoBehaviour {
	[Header("Set in Inspector")]
	public int attackerMinX = -2;
	public int attackerMaxX = 0;
	public int defenderMinX = 6;
	public int defenderMaxX = 8;

	[Header("Set Dynamically")]
	public Army currentArmy;

	private List<Vector3Int> allowed;
	private string[] unitDescriptions;
	private int index;
	private bool attackersTurn;
	private bool playersTurn = false;

    // Start is called before the first frame update
    void Start() {
        currentArmy = TMapController.M.attacker;
		unitDescriptions = currentArmy.unitDescriptions;
		index = 0;
		TMapController.M.turn.text = "Attacker's Turn\nPlace your units along the left side of the screen.";
		HighlightMap(attackerMinX, attackerMaxX);
		TMapController.M.message.text = ("Place your " + unitDescriptions[index]);
		attackersTurn = true;
		if (currentArmy == TMapController.M.aiArmy) {
			Invoke("SetAIUnits", 1f);
		} else playersTurn = true;
    }

    // Update is called once per frame
    void Update() {
        if (playersTurn && Input.GetMouseButtonUp(0))
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 10;
            pos = Camera.main.ScreenToWorldPoint(pos);
            Vector3Int cel = TMapController.M.land.WorldToCell(pos);
            //pos = TMapController.M.land.CellToWorld(cel);
			//print(pos.x);
			if (!allowed.Contains(cel)) return;

			currentArmy.SetUnit(index, cel);
			allowed.Remove(cel);
			TMapController.M.highlights.SetTile(cel, null);
			index++;
			if (index == unitDescriptions.Length) {
				if (attackersTurn) {
					currentArmy = TMapController.M.defender;
					unitDescriptions = currentArmy.unitDescriptions;
					index = 0;
					attackersTurn = false;
					TMapController.M.turn.text = "Defender's Turn\nPlace your units along the right side of the screen.";
					TMapController.M.ClearHighlights();
					HighlightMap(defenderMinX, defenderMaxX);
					TMapController.M.message.text = ("Place your " + unitDescriptions[index]);
					Invoke("SetAIUnits", 1f);
				} else {
					TMapController.M.ClearHighlights();
					TMapController.M.StartBattle();
				}
			} else TMapController.M.message.text = ("Place your " + unitDescriptions[index]);
        }
    }

	void HighlightMap(int xMin, int xMax)
	{
		allowed = new List<Vector3Int>();
		for (int y = -5; y < 6; y++)
			for (int x = xMin; x < xMax; x++)
			{
				Vector3Int v = new Vector3Int(x, y, 0);
				TMapController.tileStruct tileHas = TMapController.M.TileHas(v);
				if (tileHas.land && !tileHas.obstacle)
				{
					TMapController.M.SetHighlight(v, 'M');
					allowed.Add(v);
				}
			}
	}

	public void SetAIUnits() {
		for (int i = 0; i < unitDescriptions.Length; i++) {
			Vector3Int cel = allowed[Random.Range(0, allowed.Count)];
			currentArmy.SetUnit(i, cel);
			allowed.Remove(cel);
			TMapController.M.highlights.SetTile(cel, null);
		}
		if (!attackersTurn) {
			TMapController.M.ClearHighlights();
			TMapController.M.StartBattle();
		} else {
			currentArmy = TMapController.M.defender;
			unitDescriptions = currentArmy.unitDescriptions;
			index = 0;
			attackersTurn = false;
			TMapController.M.turn.text = "Defender's Turn\nPlace your units along the right side of the screen.";
			TMapController.M.ClearHighlights();
			HighlightMap(defenderMinX, defenderMaxX);
			TMapController.M.message.text = ("Place your " + unitDescriptions[index]);
			playersTurn = true;
		}
	}
}
