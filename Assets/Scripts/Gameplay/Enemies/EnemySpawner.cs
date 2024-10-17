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
    // Coroutine reference
    private Coroutine spawnCoroutine;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        EnemyEventManager.Instance.OnEnemySpawned.AddListener(SpawnEnemy);
    }

    private void OnDisable()
    {
        EnemyEventManager.Instance?.OnEnemySpawned.RemoveListener(SpawnEnemy);
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnEnemyAtRandomIntervals());
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a random value from the EnemyType enum.
    /// It selects a random index from the list of available EnemyType values
    /// and returns the corresponding enum value.
    /// </summary>
    /// <returns>A randomly selected EnemyType value.</returns>
    public EnemyType GetRandomEnemyType()
    {
        EnemyType[] values = (EnemyType[])System.Enum.GetValues(typeof(EnemyType));
        int randomIndex = Random.Range(0, values.Length);
        return values[randomIndex];
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Spawns an enemy based on the specified enemy type.
    /// Instantiates the corresponding enemy prefab at a random lane 
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
    /// Coroutine that spawns a random enemy prefab at random intervals between 5 to 10 seconds.
    /// </summary>
    private IEnumerator SpawnEnemyAtRandomIntervals()
    {
        while (true)
        {
            float waitTime = Random.Range(5f, 10f);
            yield return new WaitForSeconds(waitTime);

            EnemyEventManager.Instance?.OnEnemySpawned.Invoke(GetRandomEnemyType());
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
