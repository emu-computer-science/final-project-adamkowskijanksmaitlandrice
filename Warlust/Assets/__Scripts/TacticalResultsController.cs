using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TacticalResultsController : MonoBehaviour {
	[Header("Set in Inspector")]
	public Button submitButton;
	public AudioClip victorySong;
	public Text text;

    // Start is called before the first frame update
    void Start() {
		if (GameState.GS.battleResult) text.text = "The attacker has defeated the defender!";
		else text.text = "The attacker was defeated.\nThe defender has won!";
		AudioSource audioSource = GameObject.FindGameObjectWithTag("AudioSource").GetComponent<AudioSource>();
		audioSource.Stop();
		audioSource.clip = victorySong;
		audioSource.Play();
        //victorySong.Play();
		//submitButton.GetComponent<Button>().OnClick.AddListener(SceneManager.LoadScene("World", LoadSceneMode.Single));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnButtonClicked() {
		SceneManager.LoadScene("World", LoadSceneMode.Single);
	}
}
