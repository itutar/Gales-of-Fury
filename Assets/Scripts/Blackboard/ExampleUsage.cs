using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example of subscribing to the Blackboard and updating/reading values.
/// In 'Gales of Fury', you might track score, distance, or power-ups.
/// </summary>
public class ExampleUsage : MonoBehaviour
{
    private void OnEnable()
    {
        Blackboard.Instance.Subscribe<int>(BlackboardKey.Score, score =>
        {
            Debug.Log("Score updated (typed): " + score);
        });
    }

    private void Start()
    {
        // Initialize score and distance
        Blackboard.Instance.SetValue(BlackboardKey.Score, 0);
    }

    private void Update()
    {
        // Example: Add points when player collects a coin
        if (Input.GetKeyDown(KeyCode.C))
        {
            int currentScore = Blackboard.Instance.GetValue<int>(BlackboardKey.Score);
            Blackboard.Instance.SetValue(BlackboardKey.Score, currentScore + 10);
        }
    }
}
