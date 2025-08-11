using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scene-based event broadcaster for coin changes.
/// </summary>
public class OnCoinChangeEvent : MonoBehaviour
{
    public static OnCoinChangeEvent Instance { get; private set; }

    public event Action CoinChanged;

    private void Awake()
    {
        // Singleton pattern for easy scene-wide access
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Notify all listeners that coins have changed.
    /// </summary>
    public void InvokeCoinChangeEvent()
    {
        CoinChanged?.Invoke();
    }
}

