using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns a random Call Token prefab at the boss’ position on a fixed interval.
/// Attach this component to the boss GameObject.
/// </summary>
public class CallTokenSpawner : MonoBehaviour
{
    #region Fields

    [Header("Prefabs")]
    [SerializeField] private GameObject archerCallToken;
    [SerializeField] private GameObject cannonCallToken;
    [SerializeField] private GameObject catapultCallToken;
    [SerializeField] private GameObject pistoleerCallToken;

    private float spawnInterval = 3f; //20

    private GameObject[] _tokenPool;
    private Coroutine _spawnLoop;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Cache prefabs in an array for easy random selection
        _tokenPool = new[]
        {
            archerCallToken,
            cannonCallToken,
            catapultCallToken,
            pistoleerCallToken
        };
    }

    private void OnEnable()
    {
        // Start continuous spawning when the boss becomes active
        _spawnLoop = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        // Stop spawning when the boss is disabled / destroyed
        if (_spawnLoop != null)
        {
            StopCoroutine(_spawnLoop);
            _spawnLoop = null;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Coroutine that runs forever, instantiating a random token every interval.
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        // Optional initial delay (uncomment if you want to wait before first spawn)
        // yield return new WaitForSeconds(spawnInterval);

        while (true)
        {
            SpawnRandomToken();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Picks a random prefab from the pool and instantiates it at the boss’ position.
    /// </summary>
    private void SpawnRandomToken()
    {
        // Safety check: ignore if any prefab is missing
        if (_tokenPool == null || _tokenPool.Length == 0) return;

        // Random index in [0, _tokenPool.Length)
        int index = Random.Range(0, _tokenPool.Length);
        GameObject prefab = _tokenPool[index];

        if (prefab != null)
        {
            Instantiate(
                prefab,
                transform.position,            // Boss position
                Quaternion.identity,            // No rotation; change if needed
                null                            // World-space (no parent)
            );
        }
        else
        {
            Debug.LogWarning($"CallTokenSpawner: Prefab at index {index} is missing.");
        }
    }

    #endregion
}
