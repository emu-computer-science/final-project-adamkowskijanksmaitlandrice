﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
	public Color loadToColor = Color.white;

	public void ButtonOptions() {

		SceneManager.LoadScene(4);
	}

	public void ButtonQuit() {
		Application.Quit();
	}
   
}
