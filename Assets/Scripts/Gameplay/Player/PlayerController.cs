using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Fields

    // MoveToLane support
    [SerializeField] float laneSwitchSpeed = 5f;

    // SwipeUp support
    [SerializeField] float jumpForce = 5f;

    // Child object reference
    [SerializeField] GameObject windSurfBoard;
    Rigidbody windSurfBoardRigidbody;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        InputManager.OnSwipeRight += HandleSwipeRight;
        InputManager.OnSwipeLeft += HandleSwipeLeft;
        InputManager.OnSwipeUp += HandleSwipeUp;
        InputManager.OnSwipeDown += HandleSwipeDown;
        InputManager.OnDoubleTap += HandleDoubleTap;
    }

    private void OnDisable()
    {
        InputManager.OnSwipeRight -= HandleSwipeRight;
        InputManager.OnSwipeLeft -= HandleSwipeLeft;
        InputManager.OnSwipeUp -= HandleSwipeUp;
        InputManager.OnSwipeDown -= HandleSwipeDown;
        InputManager.OnDoubleTap -= HandleDoubleTap;
    }

    private void OnDestroy()
    {
        InputManager.OnSwipeRight -= HandleSwipeRight;
        InputManager.OnSwipeLeft -= HandleSwipeLeft;
        InputManager.OnSwipeUp -= HandleSwipeUp;
        InputManager.OnSwipeDown -= HandleSwipeDown;
        InputManager.OnDoubleTap -= HandleDoubleTap;
    }

    // Start is called before the first frame update
    void Start()
    {
        windSurfBoardRigidbody = windSurfBoard.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Private Methods

    void HandleSwipeRight()
    {
        if (LaneManager.instance.CurrentLane < LaneManager.instance.NumberOfLanes - 1)
        {
            LaneManager.instance.CurrentLane++;
            StartCoroutine(MoveToLaneCoroutine(LaneManager.instance.CurrentLane));
            Debug.Log("Saða Kaydýrýldý, Yeni Koridor: " + LaneManager.instance.CurrentLane);
        }
    }

    void HandleSwipeLeft()
    {
        if (LaneManager.instance.CurrentLane > 0)
        {
            LaneManager.instance.CurrentLane--;
            StartCoroutine(MoveToLaneCoroutine(LaneManager.instance.CurrentLane)); 
            Debug.Log("Sola Kaydýrýldý, Yeni Koridor: " + LaneManager.instance.CurrentLane);
        }
    }

    void HandleSwipeUp()
    {
        if (IsGrounded())
        {
            windSurfBoardRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        Debug.Log("Yukarý Kaydýrýldý");
    }

    void HandleSwipeDown()
    {
        Debug.Log("Aþaðý Kaydýrýldý");
    }

    void HandleDoubleTap()
    {
        Debug.Log("Çift Týklama Algýlandý");
    }

    /// <summary>
    /// Moves the player to the specified lane
    /// </summary>
    /// <param name="laneIndex">The index of the lane (0-based)
    /// starting from the left</param>
    IEnumerator MoveToLaneCoroutine(int laneIndex)
    {
        // Get target position from LaneManager
        float targetPositionX = LaneManager.instance.GetLanePosition(laneIndex);

        // Take current position
        Vector3 currentPosition = transform.position;

        // Set target position, Y and Z axis remain unchanged
        Vector3 targetPosition = new Vector3(targetPositionX, currentPosition.y, currentPosition.z);

        // Move to target position
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSwitchSpeed * Time.deltaTime);
            yield return null; 
        }
    }

    /// <summary>
    /// Checks if the player is grounded
    /// </summary>
    /// <returns>Returns true if the player is touching the ground, otherwise false</returns>
    bool IsGrounded()
    {
        return Physics.Raycast(windSurfBoard.transform.position, Vector3.down, 1.1f);
    }

    #endregion
}
