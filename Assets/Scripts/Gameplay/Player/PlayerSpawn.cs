using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    #region Fields

    // start position
    Vector3 startPosition;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        startPosition = Vector3.zero;
        startPosition.y = 5f; // Set a height above the water surface
        startPosition.x = LaneManager.instance.GetLanePosition(LaneManager.instance.CurrentLane);
        transform.position = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion
}
