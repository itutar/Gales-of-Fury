using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the changes of scenes in scene1.
/// </summary>
public class Scene1SceneManager : MonoBehaviour
{
    float delay = 3f;

    [SerializeField] private PauseUIRefs pauseUIRefs;

    /// <summary>
    /// Loads Scene0MainMenu (index 0) from pause menuUI.
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (pauseUIRefs.PauseUI != null)
            pauseUIRefs.PauseUI.SetActive(false);
        // Trigger GameOverEvent
        Debug.Log("ReturnToMainMenu metodu çalýþtý");
        GameOverEvent.instance.TriggerGameOver();
    }

    /// <summary>
    /// Subscribes the coroutine to the GameOverEvent.
    /// </summary>
    public void StartSceneLoadCoroutine()
    {
        // Start the coroutine to load the main menu scene after a delay
        StartCoroutine(LoadScene1WithDelay());
    }

    /// <summary>
    /// loads Scene0MainMenu (index 0) after a specified delay.
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    public IEnumerator LoadScene1WithDelay()
    {
        yield return new WaitForSecondsRealtime(delay);
        // Load the main menu scene (index 0)
        Time.timeScale = 1f; // Ensure time scale is reset to normal
        SceneManager.LoadScene(0);
    }
}
