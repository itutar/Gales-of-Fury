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
        float multiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
        transform.position += Vector3.back * moveSpeed * multiplier * Time.deltaTime;
    }
    
    #endregion

    #region Private Methods
    
    #endregion
}
