using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Fields

    // Enemy prefabs
    public GameObject sharkPrefab;
    public GameObject regularPirate1Prefab;
    public GameObject krakenPrefab;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        EnemyEventManager.Instance.OnEnemySpawned.AddListener(SpawnEnemy);
    }

    private void OnDisable()
    {
        EnemyEventManager.Instance?.OnEnemySpawned.RemoveListener(SpawnEnemy);
    }

    private void Start()
    {
        EnemyEventManager.Instance.OnEnemySpawned.Invoke(EnemyType.RegularPirate1);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Spawns an enemy based on the specified enemy type.
    /// Instantiates the corresponding enemy prefab at a random position 
    /// and triggers the OnEnemySpawned event.
    /// </summary>
    /// <param name="enemyType">The type of enemy to spawn</param>
    private void SpawnEnemy(EnemyType enemyType)
    {
        GameObject spawnedEnemy = null;

        switch (enemyType)
        {
            case EnemyType.Shark:
                spawnedEnemy = Instantiate(sharkPrefab, GetSpawnPosition(), Quaternion.identity);
                break;
            case EnemyType.RegularPirate1:
                spawnedEnemy = Instantiate(regularPirate1Prefab, GetSpawnPosition(), Quaternion.identity);
                break;
            case EnemyType.Kraken:
                spawnedEnemy = Instantiate(krakenPrefab, GetSpawnPosition(), Quaternion.identity);
                break;
        }
    }

    /// <summary>
    /// Generates and returns a random position for enemy spawning.
    /// </summary>
    /// <returns>A Vector3 representing the spawn position for an enemy.</returns>
    private Vector3 GetSpawnPosition()
    {
        int laneIndex = Random.Range(0, LaneManager.instance.NumberOfLanes);
        float xPosition = LaneManager.instance.GetLanePosition(laneIndex);
        return new Vector3(xPosition, 0, Random.Range(10, 20));
    }

    #endregion

}
