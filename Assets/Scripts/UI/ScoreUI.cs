using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        int initialScore = Blackboard.Instance.GetValue<int>(BlackboardKey.Score);
        scoreText.text = initialScore.ToString();

        Blackboard.Instance.Subscribe<int>(BlackboardKey.Score, OnScoreChanged);
    }

    private void OnScoreChanged(int newScore)
    {
        scoreText.text = newScore.ToString();
    }
}
