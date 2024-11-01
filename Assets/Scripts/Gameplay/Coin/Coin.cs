using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    #region Fields

    // Movement support
    [SerializeField] float moveSpeed = 5f;
    // Player collect flag
    private bool isCollected = false;

    #endregion

    #region Unity Methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCollected = true;
            CoinObjectPool.ReturnCoin(gameObject);
        }
    }

    private void Update()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    private void OnBecameInvisible()
    {
        // When Coin is out of camera view, send it back to the pool
        if (!isCollected)
        {
            CoinObjectPool.ReturnCoin(gameObject);
        }
    }

    /// <summary>
    /// Called each time the coin object is enabled, resetting its state for reuse.
    /// </summary>
    private void OnEnable()
    {
        isCollected = false;
    }

    #endregion

    #region Private Methods



    #endregion

}
