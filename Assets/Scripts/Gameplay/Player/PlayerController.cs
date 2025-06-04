using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerController : MonoBehaviour
{
    #region Events

    public event Action<float> OnMoveToLaneRequested;

    #endregion

    #region Fields

    float jumpForce = 55f; // Force applied when jumping
    float swipeThreshold = 25f; // Minimum distance to consider a swipe
    private bool hasSwipedThisTouch = false;
    int activeFingerId = -1; // Track the active finger ID

    [SerializeField] Rigidbody rb;

    // reference to get the water surface level
    FindWaterSurfaceLevel waterFinder;
    float currentWaterSurfaceY;

    // double tap detection
    private float lastTapTime = 0f;
    private float myDoubleTapDelay = 0.3f;

    // Check if the player is grounded
    bool IsGrounded;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        Touch.onFingerMove += OnFingerMove;
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();

        Touch.onFingerMove -= OnFingerMove;
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerUp -= OnFingerUp;
    }

    void Start()
    {
        waterFinder = GetComponent<FindWaterSurfaceLevel>();
        if (waterFinder == null)
        {
            Debug.LogError("PlayerController: FindWaterSurfaceLevel component not found! Please add it to the player GameObject.", this);
        }
    }

    void Update()
    {
        // Update the current water surface level
        if (waterFinder != null)
        {
            currentWaterSurfaceY = waterFinder.GetWaterSurfaceY();
            CheckGrounded();
        }
    }

    #endregion

    #region Private Methods

    void OnFingerDown(Finger finger)
    {
        // Check if the user using one finger at a time
        if (activeFingerId == -1)
        {
            activeFingerId = finger.index;
            hasSwipedThisTouch = false; // Reset swipe state for the new touch
        }
    }

    void OnFingerUp(Finger finger)
    {
        if (finger.index == activeFingerId)
        {
            // SADECE swipe yapýlmadýysa double tap algýla
            float currentTime = Time.time;
            if (!hasSwipedThisTouch && (currentTime - lastTapTime < myDoubleTapDelay))
            {
                DoubleTapAction();
            }
            lastTapTime = currentTime;

            // touch has ended. reset for the next touch
            activeFingerId = -1;
            hasSwipedThisTouch = false; 
        }
    }

    void OnFingerMove(Finger finger)
    {
        // only interested in the active finger and If we have already changed lanes once in this touch, skip it.
        if (finger.index != activeFingerId || hasSwipedThisTouch)
            return;
        
        var touch = finger.currentTouch;
        // if the touch is not in progress, skip it.
        if (!touch.isInProgress)
            return;
        
        float deltaX = touch.delta.x;
        float deltaY = touch.delta.y;
        // check if swipe right or left
        if (deltaX > swipeThreshold)
        {
            TryMoveToLane(Direction.Right);
            hasSwipedThisTouch = true; // Mark that we have swiped this touch
            
        }
        else if (deltaX < -swipeThreshold)
        {
            TryMoveToLane(Direction.Left);
            hasSwipedThisTouch = true;
            
        }

        // check if swipe up
        if (deltaY > swipeThreshold)
        {
            Jump();
            hasSwipedThisTouch = true; // Mark that we have swiped this touch
            
        }
    }

    /// <summary>
    /// Tries to move the player to the next lane in the specified direction.
    /// </summary>
    /// <param name="dir">direction enum that defined in PlayerController script</param>
    void TryMoveToLane(Direction dir)
    {
        int current = LaneManager.instance.CurrentLane;
        int maxIndex = LaneManager.instance.NumberOfLanes - 1;

        if (dir == Direction.Right && current < maxIndex)
        {
            current++;
            LaneManager.instance.CurrentLane = current;
            MovePlayerToLane(current);

        }
        else if (dir == Direction.Left && current > 0)
        {
            current--;
            LaneManager.instance.CurrentLane = current;
            MovePlayerToLane(current);
        }
        // if the player is already at the edge of the lanes, do nothing
        
    }

    /// <summary>
    /// Moves the player to the specified lane index.
    /// </summary>
    /// <param name="laneIndex">the lane that player will move towards</param>
    private void MovePlayerToLane(int laneIndex)
    {
        float targetX = LaneManager.instance.GetLanePosition(laneIndex);
        OnMoveToLaneRequested?.Invoke(targetX);
    }

    /// <summary>
    /// Makes the player jump.
    /// </summary>
    void Jump()
    {
        // Prevent jumping if not grounded
        if (!IsGrounded)
            return;
        
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Executes the action associated with a double-tap gesture.
    /// </summary>
    void DoubleTapAction()
    {
        Debug.Log("Double tap detected! Executing action.");
        // Call the help ship manager to spawn a help ship
        HelpShipManager.instance?.CallHelpShip();
    }

    /// <summary>
    /// checks if the player is grounded by comparing the player's Y position with the water surface level.
    /// </summary>
    void CheckGrounded()
    {
        IsGrounded = transform.position.y <= currentWaterSurfaceY + 0.2f;
    }

    #endregion

    #region Enums

    private enum Direction
    {
        Left,
        Right
    }

    #endregion

}
