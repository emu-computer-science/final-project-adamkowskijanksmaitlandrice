using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TacticalResultsController : MonoBehaviour {
	[Header("Set in Inspector")]
	public Button submitButton;
	public AudioClip victorySong;

    // Start is called before the first frame update
    void Start() {
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
