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
        //EnemyType[] values = (EnemyType[])System.Enum.GetValues(typeof(EnemyType));
        //int randomIndex = Random.Range(0, values.Length);
        //return values[randomIndex];
        return EnemyType.RegularPirate2;
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
        float targetXPosition;
        GameObject spawnedEnemy = null;

        switch (enemyType)
        {
            case EnemyType.Shark:
                spawnedEnemy = Instantiate(sharkPrefab, GetSpawnPosition(out targetXPosition), Quaternion.identity);
                var moverShark = spawnedEnemy.GetComponent<MoveEnemyTowardsTargetLane>();
                moverShark.Initialize(targetXPosition);
                
                break;
            case EnemyType.RegularPirate1:
                spawnedEnemy = Instantiate(regularPirate1Prefab, GetSpawnPosition(out targetXPosition), Quaternion.identity);
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
                spawnedEnemy = Instantiate(krakenPrefab, GetSpawnPosition(out targetXPosition), Quaternion.identity);
                var moverKraken = spawnedEnemy.GetComponent<MoveEnemyTowardsTargetLane>();
                moverKraken.Initialize(targetXPosition);
                
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
            float waitTime = Random.Range(15f, 20f);// 5-10
            yield return new WaitForSeconds(waitTime);

            EnemyEventManager.Instance?.OnEnemySpawned.Invoke(GetRandomEnemyType());
        }
    }

    /// <summary>
    /// Generates and returns a random position for enemy spawning.
    /// </summary>
    /// <returns>A Vector3 representing the spawn position for an enemy.</returns>
    private Vector3 GetSpawnPosition(out float targetXPosition)
    {
        // spawn at a random side of the screen
        bool spawnOnLeft = Random.value < 0.5f;

        // Pick a random Z distance (depth) where the enemy will appear
        float minZ = 10f;
        float maxZ = 50f; //20
        float spawnZ = Random.Range(minZ, maxZ);
        float camZ = Camera.main.transform.position.z;


        // Compute the world-space X at the chosen spawnZ plane:
        //    - Viewport X = 0 gives the left edge; Viewport X = 1 gives the right edge.
        //    - The 'z' parameter for ViewportToWorldPoint is the distance from camera.
        float distanceFromCamera = spawnZ - camZ;
        Vector3 leftWorldPoint = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, distanceFromCamera));
        Vector3 rightWorldPoint = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, distanceFromCamera));

        // put some space between the enemy and the screen edge
        float spawnXPosition = spawnOnLeft ? leftWorldPoint.x - 3f : rightWorldPoint.x + 3f;

        int laneIndex = Random.Range(0, LaneManager.instance.NumberOfLanes);
        targetXPosition = LaneManager.instance.GetLanePosition(laneIndex);
        return new Vector3(spawnXPosition, 5.5f, spawnZ);
    }

    #endregion

}
