using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 
public class Control : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}