using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameOverEvent : MonoBehaviour
{
    // --- Singleton Setup ---
    public static GameOverEvent instance;

    [Header("Game Over Events")]
    public UnityEvent OnGameOver;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Central function to be called at the end of the game
    /// </summary>
    public void TriggerGameOver()
    {
        if (OnGameOver != null)
        {
            OnGameOver.Invoke();
        }
    }
}
