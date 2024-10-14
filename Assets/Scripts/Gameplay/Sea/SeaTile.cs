using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaTile : MonoBehaviour
{
    #region Fields
    
    // movement support
    [SerializeField] float moveSpeed = 5f;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        ResetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }

    void OnBecameInvisible()
    {
        // When Tile is out of camera view, send it back to the pool
        SeaTilePool.ReturnSeaTile(gameObject);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Resets the starting position of the SeaTile
    /// </summary>
    private void ResetPosition()
    {
        transform.position = new Vector3(0, 0, 60f);
    }

    #endregion
}
