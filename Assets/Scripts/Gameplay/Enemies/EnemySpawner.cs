using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Fields

    // Enemy prefabs
    public GameObject sharkPrefab;
    public GameObject regularPirate1Prefab;
    public GameObject regularPirate2Prefab;
    public GameObject regularPirate3Prefab;
    public GameObject krakenPrefabPopUp;
    public GameObject krakenPrefabAttack;
    // Coroutine reference
    private Coroutine spawnCoroutine;

    [SerializeField] private PlayerReference playerReference;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        Blackboard.Instance.Subscribe<bool>(BlackboardKey.IsBossActive, OnBossStateChanged);
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
        if (spawnCoroutine == null)
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
        int randomIndex = UnityEngine.Random.Range(0, values.Length);
        return values[randomIndex];
        //return EnemyType.Shark;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles changes to the boss state by starting or stopping enemy spawning as needed.
    /// </summary>
    /// <remarks>This method ensures that enemy spawning is paused while the boss is active and resumes when
    /// the boss is inactive.</remarks>
    /// <param name="isBossActive">A value indicating whether the boss is currently active.  If <see langword="true"/>, enemy spawning is stopped;
    /// otherwise, enemy spawning is started.</param>
    private void OnBossStateChanged(bool isBossActive)
    {
        if (isBossActive)
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
        else
        {
            if (spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(SpawnEnemyAtRandomIntervals());
            }
        }
    }

    /// <summary>
    /// Spawns an enemy based on the specified enemy type.
    /// Instantiates the corresponding enemy prefab at a random lane 
    /// and triggers the OnEnemySpawned event.
    /// </summary>
    /// <param name="enemyType">The type of enemy to spawn</param>
    private void SpawnEnemy(EnemyType enemyType)
    {
        float targetXPosition;
        GameObject spawnedEnemy = null;

        switch (enemyType)
        {
            case EnemyType.Shark:
                spawnedEnemy = Instantiate(sharkPrefab, GetSpawnPosition(out targetXPosition), Quaternion.identity);
                var moverShark = spawnedEnemy.GetComponent<MoveSharkToLane>();
                moverShark.Initialize(targetXPosition);
                
                break;
            case EnemyType.RegularPirate1:
                spawnedEnemy = Instantiate(regularPirate1Prefab, GetSpawnPosition(out targetXPosition), Quaternion.identity);
                spawnedEnemy.transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Rotate to face the player
                var moverRegularPirate1 = spawnedEnemy.GetComponent<MoveEnemyTowardsTargetLane>();
                moverRegularPirate1.Initialize(targetXPosition);
                
                break;
            case EnemyType.RegularPirate2: 
                spawnedEnemy = Instantiate(regularPirate2Prefab, GetSpawnPosition(out targetXPosition), Quaternion.identity);
                var moverRegularPirate2 = spawnedEnemy.GetComponent<MoveEnemyTowardsTargetLane>();
                moverRegularPirate2.Initialize(targetXPosition);
                
                break;
            case EnemyType.RegularPirate3:
                spawnedEnemy = Instantiate(regularPirate3Prefab, GetSpawnPosition(out targetXPosition), Quaternion.identity);
                var moverRegularPirate3 = spawnedEnemy.GetComponent<MoveEnemyTowardsTargetLane>();
                moverRegularPirate3.Initialize(targetXPosition);
                
                break;
            case EnemyType.Kraken:
                StartCoroutine(SpawnKrakenSequence());

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
            float waitTime = UnityEngine.Random.Range(3.4f, 7f);
            float multiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
            multiplier = Mathf.Clamp(multiplier, 1f, 1.2f); // Ensure multiplier is within a reasonable range
            waitTime /= multiplier;

            float waited = 0f;
            while (waited < waitTime)
            {
                if (Blackboard.Instance.GetValue<bool>(BlackboardKey.IsBossActive))
                {
                    yield return new WaitForSeconds(1f);
                    break; // dış döngüye dön → yeniden kontrol et
                }

                float step = 0.1f;
                yield return new WaitForSeconds(step);
                waited += step;
            }

            // if boss is active, skip spawning
            if (Blackboard.Instance.GetValue<bool>(BlackboardKey.IsBossActive))
                continue;

            EnemyEventManager.Instance.OnEnemySpawned.Invoke(GetRandomEnemyType());
        }
    }

    /// <summary>
    /// Returns a spawn position that sits on a random lane,
    /// ahead of the player/camera on the Z axis.
    /// </summary>
    private Vector3 GetSpawnPosition(out float targetXPosition)
    {
        // 1) Random lane
        int laneIndex = UnityEngine.Random.Range(0, LaneManager.instance.NumberOfLanes);
        float x = LaneManager.instance.GetLanePosition(laneIndex);

        // 2) Same Y & Z as bosses
        float spawnY = -21f;
        float spawnZ = 120f;

        targetXPosition = x;     // keeps existing Initialise(float) signature happy
        return new Vector3(x, spawnY, spawnZ);
    }

    #endregion

    #region Kraken Methods

    /// <summary>
    /// spawns the Kraken prefabs at the player's position in a sequence.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnKrakenSequence()
    {
        if (playerReference.player == null)
            yield break;

        // 1. spawn
        Vector3 spawnPos = playerReference.player.transform.position;
        // ensure the spawn position is at the water surface level
        spawnPos.y = playerReference.player.GetComponent<PlayerBuoyancy>().GetPlayerWaterLevel();
        Instantiate(krakenPrefabPopUp, spawnPos, Quaternion.identity);

        // 1.5 saniye bekle
        yield return new WaitForSeconds(1.05f);

        // 2. spawn
        Instantiate(krakenPrefabAttack, spawnPos, Quaternion.identity);
    }

    #endregion

}
