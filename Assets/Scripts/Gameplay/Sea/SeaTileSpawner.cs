using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeaTileSpawner : MonoBehaviour
{
    #region Fields

    // spawner support
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float spawnZDistance = 60f;
    [SerializeField] private float tileLength = 60f;

    // initialize queue
    private Queue<GameObject> activeTiles = new Queue<GameObject>();

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        // Spawn the first few tiles
        for (int i = 0; i < 5; i++)
        {
            SpawnTile();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        // Spawn a new tile if the oldest tile is behind the player
        if (activeTiles.Count > 0 && activeTiles.Peek().transform.position.z < playerTransform.position.z - tileLength)
        {
            GameObject oldTile = activeTiles.Dequeue();
            SeaTilePool.ReturnSeaTile(oldTile);
            SpawnTile();
        }
        
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// spawns a new tile
    /// </summary>
    private void SpawnTile()
    {
        // Get the new tile from SeaTilePool and spawn it ahead of the player
        GameObject newTile = SeaTilePool.GetSeaTile();
        
        // Starting Z position if there are no tiles
        float spawnZPosition = 0f;
        
        // If there are active tiles, take the Z position of the last tile
        if (activeTiles.Count > 0)
        {
            GameObject lastTile = activeTiles.Last(); 
            spawnZPosition = lastTile.transform.position.z + tileLength; 
        }
        
        newTile.transform.position = new Vector3(0, 0, spawnZPosition);
        newTile.SetActive(true);
        activeTiles.Enqueue(newTile);
    }

    #endregion 
}
