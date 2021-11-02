using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void GoToScene(int sceneIndex)
    {
        Debug.Log("Loading new scene...");
        SceneManager.LoadScene(sceneIndex);
    }
}
