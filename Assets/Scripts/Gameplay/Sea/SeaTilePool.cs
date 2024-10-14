using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object pool for SeaTile objects
/// </summary>
public static class SeaTilePool
{
    static GameObject prefabSeaTile;
    static List<GameObject> pool;

    /// <summary>
    /// Initializes the SeaTile pool
    /// </summary>
    public static void Initialize()
    {
        // Create and fill the pool
        prefabSeaTile = Resources.Load<GameObject>("SeaTile"); 
        pool = new List<GameObject>(10); 

        for (int i = 0; i < pool.Capacity; i++)
        {
            pool.Add(CreateNewSeaTile());
        }
    }

    /// <summary>
    /// Gets a SeaTile object from the pool
    /// </summary>
    /// <returns>SeaTile object</returns>
    public static GameObject GetSeaTile()
    {
        // Check for available object in the pool
        if (pool.Count > 0)
        {
            GameObject seaTile = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            seaTile.SetActive(true);
            return seaTile;
        }
        else
        {
            Debug.Log("Expanding SeaTile pool...");
            return CreateNewSeaTile();
        }
    }

    /// <summary>
    /// Returns a SeaTile object to the pool
    /// </summary>
    /// <param name="seaTile">SeaTile object</param>
    public static void ReturnSeaTile(GameObject seaTile)
    {
        seaTile.SetActive(false);
        pool.Add(seaTile);
    }

    /// <summary>
    /// Creates a new SeaTile object
    /// </summary>
    /// <returns>New SeaTile object</returns>
    static GameObject CreateNewSeaTile()
    {
        GameObject seaTile = GameObject.Instantiate(prefabSeaTile);
        seaTile.SetActive(false);
        GameObject.DontDestroyOnLoad(seaTile);
        return seaTile;
    }
}
