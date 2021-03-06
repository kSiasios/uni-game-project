using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Tooltip("Reference to the PauseMenu UI Gameobject")]
    public GameObject pauseMenuUI;
    [Tooltip("Reference to the Gameplay UI Gameobject")]
    public GameObject gameplayUI;

    private void Awake()
    {
        if (pauseMenuUI == null)
        {
            // if pauseMenuUI is not initialized, initialize it
            pauseMenuUI = transform.Find("PauseMenu").gameObject;
        }

        if (gameplayUI == null)
        {
            // if pauseMenuUI is not initialized, initialize it
            gameplayUI = transform.Find("GameplayUI").gameObject;
        }

        if (pauseMenuUI.activeInHierarchy)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (GameManager.gameIsPaused)
            {
                if (pauseMenuUI.activeInHierarchy)
                {
                    ResumeGame();
                }
                else
                {
                    // Another script has has paused the game
                    // The pause menu has priority over them
                    PauseGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        // Enable all other UI elements
        gameplayUI.SetActive(true);

        Time.timeScale = 1f;
        GameManager.gameIsPaused = false;
    }

    void PauseGame()
    {
        // Disable all other UI elements
        gameplayUI.SetActive(false);
        // Enable the pause menu gameobject to make it visible
        pauseMenuUI.SetActive(true);
        // Freeze time
        Time.timeScale = 0f;
        GameManager.gameIsPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
