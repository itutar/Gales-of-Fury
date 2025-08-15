using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTiles : MonoBehaviour
{
    #region Fields

    public GameObject tilePrefab;        // 400x400 tile prefab
    public int numberOfTiles = 2;        // How many tiles are kept alive
    public float tileSize = 400f;        // Length of a tile along Z

    [SerializeField] private PlayerReference playerRef;  // SO holding player reference

    // List of spawned tiles
    private List<GameObject> spawnedTiles = new List<GameObject>();

    // Next spawn Z
    private float nextSpawnZ = 0f;

    // Cached player transform
    private Transform playerTr;

    #endregion

    #region Unity Methods

    //private void Awake()
    //{
    //    // Cache player transform from ScriptableObject
    //    if (playerRef == null || playerRef.player == null)
    //    {
    //        Debug.LogError("EndlessTiles: PlayerReference or its player is not assigned. Please create/assign the PlayerReference asset and set its player.");
    //    }
    //    else
    //    {
    //        playerTr = playerRef.player.transform; // cache transform for runtime speed
    //    }
    //}

    private void Start()
    {
        // Cache player transform from ScriptableObject
        if (playerRef == null || playerRef.player == null)
        {
            Debug.LogError("EndlessTiles: PlayerReference or its player is not assigned. Please create/assign the PlayerReference asset and set its player.");
        }
        else
        {
            playerTr = playerRef.player.transform; // cache transform for runtime speed
        }
        // Spawn initial tiles
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile(nextSpawnZ);
            nextSpawnZ += tileSize;
        }
    }

    private void Update()
    {
        if (playerTr == null) return; // Safety guard

        if (spawnedTiles.Count > 0)
        {
            GameObject firstTile = spawnedTiles[0];
            float tileZ = firstTile.transform.position.z;

            // If the tile fell behind the player by one tile length, move it to the front
            if (tileZ + tileSize < playerTr.position.z)
            {
                spawnedTiles.RemoveAt(0);

                float newZ = spawnedTiles[spawnedTiles.Count - 1].transform.position.z + tileSize;
                firstTile.transform.position = new Vector3(
                    firstTile.transform.position.x,
                    firstTile.transform.position.y,
                    newZ
                );

                spawnedTiles.Add(firstTile);
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Spawns a tile at given Z position.
    /// </summary>
    private void SpawnTile(float zPos)
    {
        Vector3 spawnPosition = new Vector3(0f, 5f, zPos);
        GameObject newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
        spawnedTiles.Add(newTile);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the spawned tile list.
    /// </summary>
    public List<GameObject> GetSpawnedTiles()
    {
        return spawnedTiles;
    }

    #endregion

}
