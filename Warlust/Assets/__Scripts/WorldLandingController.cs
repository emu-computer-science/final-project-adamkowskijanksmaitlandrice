using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldLandingController : MonoBehaviour {
	[Header("Set in Inspector")]
	public Button submitButton;
	public AudioClip victorySong;
	public Text text;

    // Start is called before the first frame update
    void Start() {
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnButtonClicked() {
        WMapController.M.turn.text = "Blue's Turn";
        SceneManager.UnloadSceneAsync(5);
	}
}
