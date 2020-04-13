using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class WMapController : MonoBehaviour
{
    [Header("Set in Inspector")]
    public Tilemap land;
    public Tilemap obstacles;
    public Tilemap highlights;
    public TileBase moveHighlight;
    public GameObject squadron;
    public GameObject redGameObject;
    public GameObject blueGameObject;
    public Unit archer;
    public Unit warrior;
    public Unit wizard;

    [Header("Set Dynamically")]
    public Kingdom red;
    public Kingdom blue;
    public Kingdom currentTurn;

    private GameObject moving;
    public static WMapController M;
    private List<Vector3Int> withinRange;
    private List<Vector3Int> excludeRange;

    // Start is called before the first frame update
    void Awake()
    {
        M = this;
		if (GameState.GS == null) {
        red = redGameObject.GetComponent<Kingdom>();
        blue = blueGameObject.GetComponent<Kingdom>();
		/*red = GameState.GS.kingdoms[0];
		blue = GameState.GS.kingdoms[1];

		foreach (Squad squadron in red.squadrons) {
			squadron.land = land;
			squadron.SetPosition(squadron.currentPlayerTile);
			squadron.sqKingdom = red;
		}
		foreach (Squad squadron in blue.squadrons) {
			squadron.land = land;
			squadron.SetPosition(squadron.currentPlayerTile);
			squadron.sqKingdom = blue;
		}*/

        List<Unit> standard = new List<Unit>() { archer, warrior, wizard };
        makeSquad(squadron, standard, 'R', -1, 0);
        makeSquad(squadron, standard, 'B', 1, 0);
		currentTurn = red;
		} else {
			red = GameState.GS.kingdoms[0];
			blue = GameState.GS.kingdoms[1];

			int id = 0;
			foreach (squadStruct squad in red.squadrons) {
				makeSquad(squad, red, id);
				id++;
			}
			id = 0;
			foreach (squadStruct squad in blue.squadrons) {
				makeSquad(squad, blue, id);
				id++;
			}
			if (GameState.GS.currentTurn != null) currentTurn = GameState.GS.currentTurn;
			else currentTurn = blue;
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown("space")) {
			clearMove();
			print("End of current turn");
			if (currentTurn == red) currentTurn = blue;
			else  currentTurn = red;
        }
    }

    public void makeSquad(GameObject prefab, List<Unit> units, char team, int x, int y)
    {
        GameObject squad = Instantiate(prefab);
        Squad squadScript = squad.GetComponent<Squad>();
        squadScript.troops = units;
        Vector3Int v3i = new Vector3Int(x, y, 0);
        squadScript.currentPlayerTile = v3i;
        squad.transform.position = land.CellToWorld(squadScript.currentPlayerTile);
        if (team == 'R')
        {
            //red.squadrons.Add(squadScript);
            squadScript.sqKingdom = red;
        }
        else
        {
            //blue.squadrons.Add(squadScript);
            squadScript.sqKingdom = blue;
        }
    }

	public void makeSquad(squadStruct squad, Kingdom kingdom, int armyID) {
		GameObject squadGO = Instantiate(squadron);
		Squad squadScript = squadGO.GetComponent<Squad>();
		List<Unit> standard = new List<Unit>() { archer, warrior, wizard };
		squadScript.troops = standard;
		int x = squad.coordinates[0];
		int y = squad.coordinates[1];
		Vector3Int v3i = new Vector3Int(x, y, 0);
        squadScript.currentPlayerTile = v3i;
        squadGO.transform.position = land.CellToWorld(squadScript.currentPlayerTile);
		squadScript.sqKingdom = kingdom;
		squadScript.ID = armyID;
	}

    public void clearMove()
    {
        highlights.ClearAllTiles();
        withinRange = new List<Vector3Int>();
        excludeRange = new List<Vector3Int>();
    }

    public void startMove(GameObject toMove, Vector3Int currentTile, int moveRange)
    {
        print("About to start move");
        clearMove();
        moving = toMove;
        excludeRange.Add(currentTile);
        List<Vector3Int> queue = new List<Vector3Int>() { currentTile };
        for (int i = 0; i < moveRange; i++)
            queue = updateQueue(queue);
        foreach (Vector3Int v in withinRange)
            highlights.SetTile(v, moveHighlight);
    }

    public List<Vector3Int> updateQueue(List<Vector3Int> queue)
    {
        List<Vector3Int> queue2 = new List<Vector3Int>();
        while (queue.Count > 0)
        {
            Vector3Int check = queue[0];
            int x = check.x, y = check.y, z = check.z;
            queue.Remove(check);
            int row = Mathf.Abs(check.y % 2);
            Vector3Int left = new Vector3Int(x-1, y, z);
            Vector3Int right = new Vector3Int(x+1, y, z);
            Vector3Int upLeft = new Vector3Int(x-1+row, y+1, z);
            Vector3Int upRight = new Vector3Int(x+row, y+1, z);
            Vector3Int downLeft = new Vector3Int(x-1+row, y-1, z);
            Vector3Int downRight = new Vector3Int(x+row, y-1, z);
            foreach (Vector3Int dir in new Vector3Int[] {
                left, right, upLeft, upRight, downLeft, downRight})
            {
                if (obstacles.HasTile(dir) || !land.HasTile(dir)) continue;
                if (!withinRange.Contains(dir) && !excludeRange.Contains(dir)) withinRange.Add(dir);
                queue2.Add(dir);
            }
        }
        queue = queue2;
        return queue;
    }

    public void endMove(Vector3Int destTile)
    {
        if (moving == null)
        {
            //print("moving == null | "+ currentTurn +" | "+ roundState);
            return;
        }

        if (withinRange.Contains(destTile))
        {
            foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
            {
                if (land.WorldToCell(unit.transform.position) == destTile &&
                    unit.GetComponent<Squad>().sqKingdom != currentTurn)
                {
                    print("Attack!");
                    // do the actual stuff here...
					if (currentTurn == red) {
						GameState.GS.currentTurn = blue;
						GameState.GS.events.Add(new GameEvent(red, moving.GetComponent<Squad>().ID, blue, unit.GetComponent<Squad>().ID, eventType.attacks));
					} else {
						GameState.GS.currentTurn = red;
						GameState.GS.events.Add(new GameEvent(blue, moving.GetComponent<Squad>().ID, red, unit.GetComponent<Squad>().ID, eventType.attacks));
					}
					SceneManager.LoadScene("Tactical", LoadSceneMode.Single);
                    break;
                }
            }
            moving.GetComponent<Squad>().SetPosition(destTile);

            foreach (GameObject town in GameObject.FindGameObjectsWithTag("Town"))
                if (land.WorldToCell(town.transform.position) == destTile &&
                    town.GetComponent<Town>().townKingdom != currentTurn)
                {
                    town.GetComponent<Town>().Flip();
                    print("Town taken!");
                }

                    if (currentTurn == red) currentTurn = blue;
            else currentTurn = red;
            moving = null;
        }
        clearMove();
    }
}
