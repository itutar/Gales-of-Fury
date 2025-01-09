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

    

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
    }
    
    #endregion

    #region Private Methods
    
    #endregion
}
