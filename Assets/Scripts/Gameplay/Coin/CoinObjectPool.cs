using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoinObjectPool 
{
    #region Fields

    static GameObject prefabCoin;
    static List<GameObject> pool;
    static List<GameObject> activeCoins;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the list of active Coin objects
    /// </summary>
    public static List<GameObject> ActiveCoins
    {
        get { return activeCoins; }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the Coin pool
    /// </summary>
    public static void Initialize()
    {
        // Load the Coin prefab and initialize the pool
        prefabCoin = Resources.Load<GameObject>("Coin");
        pool = new List<GameObject>(18);
        activeCoins = new List<GameObject>(18);

        for (int i = 0; i < pool.Capacity; i++)
        {
            pool.Add(CreateNewCoin());
        }
    }

    /// <summary>
    /// Gets a Coin object from the pool
    /// </summary>
    /// <returns></returns>
    public static GameObject GetCoin()
    {
        if (pool.Count > 0)
        {
            GameObject coin = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            activeCoins.Add(coin);
            coin.SetActive(true);
            return coin;
        }
        else
        {
            Debug.Log("Expanding Coin pool...");
            GameObject newCoin = CreateNewCoin();
            activeCoins.Add(newCoin);
            pool.Add(newCoin);
            return newCoin;
        }
    }

    /// <summary>
    /// Returns a Coin object to the pool
    /// </summary>
    /// <param name="coin"></param>
    public static void ReturnCoin(GameObject coin)
    {
        coin.SetActive(false);
        activeCoins.Remove(coin);
        pool.Add(coin);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a new coin object
    /// </summary>
    /// <returns></returns>
    static GameObject CreateNewCoin()
    {
        GameObject coin = GameObject.Instantiate(prefabCoin);
        coin.SetActive(false);
        //GameObject.DontDestroyOnLoad(coin);
        return coin;
    }

    #endregion
}
