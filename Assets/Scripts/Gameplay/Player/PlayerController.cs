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

    private bool isPaused = false;

    private int extraJumpsRemaining;   // runtime counter

    float jumpForce = 55f; // Force applied when jumping
    float swipeThreshold = 20f; // Minimum distance to consider a swipe (25f baþlangýçtaki deðer)
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

        Blackboard.Instance.Subscribe<int>(BlackboardKey.ExtraJumps, val =>
        {
            extraJumpsRemaining = val;   // whenever boost starts / ends
        });
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
        // check if swipe down
        else if (deltaY < -swipeThreshold)
        {
            SwipeDownAction();
            hasSwipedThisTouch = true;
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
            Blackboard.Instance.SetValue(BlackboardKey.PlayerLane, current);

        }
        else if (dir == Direction.Left && current > 0)
        {
            current--;
            LaneManager.instance.CurrentLane = current;
            MovePlayerToLane(current);
            Blackboard.Instance.SetValue(BlackboardKey.PlayerLane, current);
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
        /*
        // Prevent jumping if not grounded
        if (!IsGrounded)
            return;
        // Stop downward movement if the player is moving up
        if (rb.velocity.y < 0)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0;
            rb.velocity = velocity;
        }
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        */

        if (IsGrounded)
        {
            // Stop downward movement if the player is moving up
            if (rb.velocity.y < 0)
            {
                Vector3 velocity = rb.velocity;
                velocity.y = 0;
                rb.velocity = velocity;
            }
            // normal jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // refresh counter every time we leave the ground
            extraJumpsRemaining = Blackboard.Instance.GetValue<int>(BlackboardKey.ExtraJumps);
            return;
        }

        // allow mid-air jump if boost is active
        if (extraJumpsRemaining > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // cancel downward momentum
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            extraJumpsRemaining--;
        }
    }
    /// <summary>
    /// makes the player go down.
    /// </summary>
    void SwipeDownAction()
    {
        // prevent swiping down if grounded
        if (IsGrounded)
        {
            return;
        }

        // Stop upward movement if the player is moving up
        if (rb.velocity.y > 0)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0;
            rb.velocity = velocity;
        }
        rb.AddForce(Vector3.down * jumpForce / 2, ForceMode.Impulse);
    }

    /// <summary>
    /// Executes the action associated with a double-tap gesture.
    /// </summary>
    void DoubleTapAction()
    {
        // test
        //ScoreManager.Instance.Add(10000);
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
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
