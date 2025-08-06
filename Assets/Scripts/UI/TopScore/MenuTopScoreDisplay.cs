using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays the saved top score in the menu.
/// </summary>
public class MenuTopScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text topScoreText;

    void Start()
    {
        // load the saved coins from ES3
        int savedCoins = ES3.Load<int>("topScore", 0);
        topScoreText.text = savedCoins.ToString();
    }
}
