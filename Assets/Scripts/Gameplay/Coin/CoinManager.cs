using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the coin counter in the game.
/// </summary>
public class CoinManager : MonoBehaviour
{
    #region Singleton Pattern
    public static CoinManager Instance { get; private set; }
    #endregion

    #region Unity Methods
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Let the counter start from zero.
        Blackboard.Instance.SetValue(BlackboardKey.Coin, 0);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Increases the coin counter and writes it on the Blackboard.
    /// </summary>
    public void Add(int amount)
    {
        int current = Blackboard.Instance.GetValue<int>(BlackboardKey.Coin);
        int updated = current + amount;
        Blackboard.Instance.SetValue(BlackboardKey.Coin, updated);
    }
    #endregion
}
