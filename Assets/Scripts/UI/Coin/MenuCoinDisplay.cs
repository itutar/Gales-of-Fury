using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays the saved coins in the menu.
/// </summary>
public class MenuCoinDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;

    private void OnEnable()
    {
        RefreshFromSave(); // Refresh on enable
        if (OnCoinChangeEvent.Instance != null)
            OnCoinChangeEvent.Instance.CoinChanged += RefreshFromSave;
    }

    private void OnDisable()
    {
        if (OnCoinChangeEvent.Instance != null)
            OnCoinChangeEvent.Instance.CoinChanged -= RefreshFromSave;
    }

    /// <summary>
    /// Loads coin amount from ES3 and updates the label.
    /// </summary>
    public void RefreshFromSave()
    {
        int savedCoins = ES3.Load<int>("coins", 0);
        coinText.text = savedCoins.ToString();
    }
}
