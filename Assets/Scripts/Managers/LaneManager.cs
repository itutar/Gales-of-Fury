using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour
{

    #region Fields

    // singelton instance
    public static LaneManager instance;
    
    // lane support
    int numberOfLanes = 4;
    float[] lanePositions;

    #endregion

    #region Properties

    /// <summary>
    /// Returns the x-coordinate of the first lane 
    /// starting from the left
    /// </summary>
    public float FirstLane
    {
        get
        {
            return lanePositions[0];
        }
    }

    /// <summary>
    /// Returns the x-coordinate of the second lane 
    /// starting from the left
    /// </summary>
    public float SecondLane
    {
        get
        {
            return lanePositions[1];
        }
    }

    /// <summary>
    /// Returns the x-coordinate of the third lane 
    /// starting from the left
    /// </summary>
    public float ThirdLane
    {
        get
        {
            return lanePositions[2];
        }
    }

    /// <summary>
    /// Returns the x-coordinate of the fourth lane 
    /// starting from the left
    /// </summary>
    public float FourthLane
    {
        get
        {
            return lanePositions[3];
        }
    }




    #endregion

    #region Private Methods

    /// <summary>
    /// Calculates lane positions
    /// </summary>
    void CalculateLanePositions()
    {
        float cameraDistance = Mathf.Abs(Camera.main.transform.position.z);
        float screenWidth = Screen.width;
        float laneWidth = screenWidth / numberOfLanes;
        lanePositions = new float[numberOfLanes];

        for (int i = 0; i < numberOfLanes; i++)
        {
            float laneCenterX = (laneWidth * i) + (laneWidth / 2);
            float worldX = Camera.main.ScreenToWorldPoint(new Vector3(laneCenterX, 0, cameraDistance)).x;
            lanePositions[i] = worldX;
        }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        CalculateLanePositions();
    }

    #endregion

}
