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

    void Start()
    {
        // load the saved coins from ES3
        int savedCoins = ES3.Load<int>("coins", 0); 
        coinText.text = savedCoins.ToString();
    }
}
