using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCallTokenDrop : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject cannonCallTokenPrefab;
    [SerializeField] private GameObject catapultCallTokenPrefab;
    [SerializeField] private GameObject archerCallTokenPrefab;
    [SerializeField] private GameObject pistoleerCallTokenPrefab;
    [SerializeField] private float tokenSpawnOffset = 4f;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        EnemyEventManager.Instance.OnEnemyCallTokenDrop.AddListener(HandleCallTokenDrop);
    }

    private void OnDisable()
    {
        EnemyEventManager.Instance.OnEnemyCallTokenDrop.RemoveListener(HandleCallTokenDrop);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles the spawning of a specific CallToken object based on drop probability when an enemy attacks.
    /// Spawns one of the predefined tokens behind the enemy's position based on weighted probabilities.
    /// </summary>
    /// <param name="enemy"></param>
    private void HandleCallTokenDrop(GameObject enemy)
    {
        float randomValue = Random.Range(0f, 100f); 

        GameObject tokenPrefabToSpawn = null;

        if (randomValue <= 10f)
        {
            tokenPrefabToSpawn = cannonCallTokenPrefab; // %10 
        }
        else if (randomValue <= 25f)
        {
            tokenPrefabToSpawn = catapultCallTokenPrefab; // %15 
        }
        else if (randomValue <= 55f)
        {
            tokenPrefabToSpawn = archerCallTokenPrefab; // %30 
        }
        else
        {
            tokenPrefabToSpawn = pistoleerCallTokenPrefab; // %45 
        }

        if (tokenPrefabToSpawn != null)
        {
            Vector3 spawnPosition = enemy.transform.position - enemy.transform.forward * tokenSpawnOffset;
            Instantiate(tokenPrefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    #endregion
}
