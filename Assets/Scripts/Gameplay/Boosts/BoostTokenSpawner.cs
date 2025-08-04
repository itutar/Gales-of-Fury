using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTokenSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] boostTokenPrefabs; 

    private float spawnInterval = 10f;//50
    private float spawnY = -21f;
    private float spawnZ = 120f;

    private void Start()
    {
        if (boostTokenPrefabs == null || boostTokenPrefabs.Length == 0)
        {
            Debug.LogError($"{nameof(BoostTokenSpawner)}: Boost Token prefab’leri atanmadý!");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnToken();
        }
    }

    private void SpawnToken()
    {
        if (LaneManager.instance == null)
        {
            Debug.LogWarning($"{nameof(BoostTokenSpawner)}: LaneManager bulunamadý.");
            return;
        }

        int randomLane = Random.Range(0, LaneManager.instance.NumberOfLanes);
        float spawnX = LaneManager.instance.GetLanePosition(randomLane);
        Vector3 spawnPos = new(spawnX, spawnY, spawnZ);

        // pick a random prefab from the array
        int randomIndex = Random.Range(0, boostTokenPrefabs.Length);
        //GameObject selectedPrefab = boostTokenPrefabs[randomIndex];
        GameObject selectedPrefab = boostTokenPrefabs[1];

        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
    }
}
