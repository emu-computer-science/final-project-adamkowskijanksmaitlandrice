using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 
public class GoToOptions : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene("Options");
    }
}