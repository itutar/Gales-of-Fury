using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    #region Fields

    [SerializeField] float coinSpacing = 4.5f;
    [SerializeField] int coinsInRow = 6;
    [SerializeField] float checkInterval = 5f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        //SpawnCoinsInRandomLane();
        StartCoroutine(CheckForActiveCoins());
    }

    #endregion

    #region Private Methods

    private void SpawnCoinsInRandomLane()
    {
        // pick a random lane 
        int randomLane = Random.Range(0, LaneManager.instance.NumberOfLanes);

        // x position of the picked lane
        float lanePositionX = LaneManager.instance.GetLanePosition(randomLane);

        // position of the first coin
        Vector3 startPosition = new Vector3(lanePositionX, 1, 20);

        // place the remaining coins
        for (int i = 0; i < coinsInRow; i++)
        {
            GameObject coin = CoinObjectPool.GetCoin();
            coin.transform.position = startPosition + new Vector3(0, 0, i * coinSpacing);
            coin.SetActive(true);
        }
    }

    /// <summary>
    /// Periodically checks if there are any active coins in the scene. 
    /// If no active coins are found, it triggers the coin spawning process.
    /// </summary>
    /// <returns>Coroutine enumerator for timed checks.</returns>
    private IEnumerator CheckForActiveCoins()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            // Check if there are any active coin objects in the pool
            if (!AreCoinsActive())
            {
                SpawnCoinsInRandomLane();
            }
        }
    }

    /// <summary>
    /// Checks if there are any active coin objects in the pool.
    /// Returns true if at least one coin is active in the scene, otherwise false.
    /// </summary>
    /// <returns>Boolean indicating the presence of active coins.</returns>
    private bool AreCoinsActive()
    {
        // Sahnedeki tüm aktif coinleri kontrol et
        foreach (GameObject coin in CoinObjectPool.ActiveCoins)
        {
            if (coin.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    #endregion
}
