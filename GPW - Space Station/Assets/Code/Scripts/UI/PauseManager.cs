using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;

    private bool isPaused = false;

    void Start()
    {
        // Hide menu by default
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
    }

    void Update()
    {
        // ESC key to open the pause menu only
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenu == null)
        {
            Debug.LogError("Pause menu is n/a");
            return;
        }

        // Set the pause menu active
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (pauseMenu == null || settingsMenu == null)
        {
            Debug.LogError("Pause or settings menu is n/a!");
            return;
        }

        // Deactivate pause and settings menu
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);

        // Resume game
        Time.timeScale = 1f;
        isPaused = false;

        // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Game Resumed");
    }

    public void OpenSettings()
    {
        if (settingsMenu == null || pauseMenu == null)
        {
            Debug.LogError("Settings menu missing!");
            return;
        }

        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);

        Debug.Log("Settings Opened");
    }

    public void CloseSettings()
    {
        if (settingsMenu == null || pauseMenu == null)
        {
            Debug.LogError("Pause / settings menu reference is missing!");
            return;
        }

        // Close the settings menu and return to the pause menu
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);

        Debug.Log("Settings Closed");
    }
}
