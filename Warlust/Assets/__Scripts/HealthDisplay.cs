using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [Header("Set in Inspector")]
	public Text text;
    public float fadeOutTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowDmg(string msg)
    {
        StartCoroutine(ShowDmgRun(msg));
    }

    private IEnumerator ShowDmgRun(string msg)
    {
        text.text = msg;
        text.color = Color.red;
        for (float t = 0.01f; t < fadeOutTime; t += Time.deltaTime)
        {
            text.color = Color.Lerp(Color.red, Color.clear, Mathf.Min(1, t / fadeOutTime));
            yield return null;
        }

    }
}
