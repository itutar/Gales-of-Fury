using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUIScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; // Assign in inspector

    private void Start()
    {
        // Subscribe to changes for the Score key
        Blackboard.Instance.Subscribe<int>(BlackboardKey.Score, UpdateScoreText);

        // Set the initial score text
        int initialScore = Blackboard.Instance.GetValue<int>(BlackboardKey.Score);
        UpdateScoreText(initialScore);
    }

    /// <summary>
    /// Updates the TMP UI text with the current score value.
    /// </summary>
    private void UpdateScoreText(int newScore)
    {
        scoreText.text = newScore.ToString();
    }
}
