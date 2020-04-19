using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenuControl : MonoBehaviour
{
	public void ButtonBackToMain() {
		SceneManager.LoadScene(0);
	}

	
	public void ButtonQuit() {
		Application.Quit();
	}
}