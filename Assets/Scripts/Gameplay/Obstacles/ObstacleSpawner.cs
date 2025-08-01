using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject logPrefab;
    private float spawnY = -21f; 
    private float spawnZ = 120f; 

    private float minSpawnDelay = 2f;
    private float maxSpawnDelay = 5f;

    // start rotation for the spawned logs
    private Vector3 spawnRotation = new Vector3(0f, 90f, 0f);

    private Coroutine spawnRoutine;

    private float speedMultiplier = 1f;

    private void Start()
    {
        if (logPrefab == null)
        {
            Debug.LogError("ObstacleSpawner: Missing references.");
            enabled = false;
            return;
        }

        spawnRoutine = StartCoroutine(SpawnLoop());

        // Subscribe to Blackboard SpeedMultiplier updates
        Blackboard.Instance.Subscribe<float>(BlackboardKey.SpeedMultiplier, OnSpeedMultiplierChanged);

        // Initialize local speedMultiplier
        speedMultiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
        if (speedMultiplier <= 0) speedMultiplier = 1f;
    }

    private IEnumerator SpawnLoop()
    {
        // Infinite loop that spawns a log, waits a random delay, then repeats
        while (true)
        {
            SpawnLog();
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay) / speedMultiplier;
            yield return new WaitForSeconds(delay);
        }
    }

    private void OnSpeedMultiplierChanged(float newMultiplier)
    {
        speedMultiplier = Mathf.Clamp(newMultiplier / 1.5f, 0.5f, 3f);
        // Clamped between 0.5 and 3 (you can tweak these clamp values)
    }

    private void SpawnLog()
    {
        // Choose a random lane index and convert it to an X coordinate
        int randomLane = Random.Range(0, LaneManager.instance.NumberOfLanes);
        float xPos = LaneManager.instance.GetLanePosition(randomLane);

        Vector3 spawnPos = new Vector3(xPos, spawnY, spawnZ);
        // Use the spawnRotation field
        Quaternion rotation = Quaternion.Euler(spawnRotation);
        Instantiate(logPrefab, spawnPos, rotation);
    }
}
