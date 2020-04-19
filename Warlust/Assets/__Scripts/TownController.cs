using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TownController : MonoBehaviour {
	[Header("Set in Inspector")]
	public Button buyButton;
	public Text text;

	[Header("Set Dynamically")]
	public static Town town;

    // Start is called before the first frame update
    void Start()
    {
		string unitString = "";
		for (int i = 0; i < town.units.Length; i++) {
			unitString += town.units[i] + "\n";
		}
        text.text = unitString + "\nCost:\t" + town.armyCost;
		if (town.kingdom.gold < town.armyCost) buyButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void BuyArmy() {
		town.BuyArmy();
		SceneManager.UnloadSceneAsync("TownScene");
	}

	public void ReturnToWorldMap() {
		SceneManager.UnloadSceneAsync("TownScene");
	}
}
