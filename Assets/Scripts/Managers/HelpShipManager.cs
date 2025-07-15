using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpShipManager : MonoBehaviour
{
    #region Fields

    // Singleton
    public static HelpShipManager instance;

    public GameObject cannonHelpShipPrefab;
    public GameObject catapultHelpShipPrefab;
    public GameObject archerHelpShipPrefab;
    public GameObject pistoleerHelpShipPrefab;

    private GameObject currentHelpShipPrefab;
    private bool hasHelpShipAvailable = false;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        //InputManager.OnDoubleTap += CallHelpShip;
        // doubletap event 
    }

    private void OnDisable()
    {
        //InputManager.OnDoubleTap -= CallHelpShip;
        // doubletap event
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Grants the player the ability to call a specific HelpShip. 
    /// Sets the HelpShip prefab that will be instantiated when the player uses their available call.
    /// </summary>
    /// <param name="helpShipPrefab">The HelpShip prefab to be assigned for the next call.</param>
    public void GrantHelpShip(GameObject helpShipPrefab)
    {
        currentHelpShipPrefab = helpShipPrefab;
        hasHelpShipAvailable = true;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Instantiates the assigned HelpShip prefab.
    /// This method is triggered by a double-tap gesture if a HelpShip call is available.
    /// </summary>
    public void CallHelpShip()
    {
        if (hasHelpShipAvailable && currentHelpShipPrefab != null)
        {
            // Get the current lane from the LaneManager
            int currentLane = LaneManager.instance.CurrentLane;
            float x = LaneManager.instance.GetLanePosition(currentLane);

            // Set the spawn position based on the current lane
            Vector3 spawnPosition = new Vector3(x, -21f, 120f);
            Quaternion rotation = Quaternion.Euler(0f, 180f, 0f);
            // Instantiate the HelpShip at the specified position
            Instantiate(currentHelpShipPrefab, spawnPosition, rotation);
            hasHelpShipAvailable = false; 
        }
    }

    #endregion
}
