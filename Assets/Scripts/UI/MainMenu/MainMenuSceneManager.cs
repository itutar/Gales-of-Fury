using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the changes of scenes in the main menu scene.
/// </summary>
public class MainMenuSceneManager : MonoBehaviour
{
    /// <summary>
    /// Starts the game by loading the gameplay scene (index 1).
    /// </summary>
    public void StartGame()
    {
        // Load the gameplay scene
        SceneManager.LoadScene(1);
    }



    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
