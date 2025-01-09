using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    #region Fields

    // input support
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private float swipeThreshold = 50f;

    private float doubleTapTime = 0.3f;
    private bool oneTap = false;

    private Coroutine doubleTapCoroutine;

    private Vector2 firstTapPosition;
    // The offset of the first touch relative to the second touch(pixels)
    private float positionThreshold = 20f; 

    #endregion

    #region Events

    // Events for gestures
    public static event Action OnSwipeRight;
    public static event Action OnSwipeLeft;
    public static event Action OnSwipeUp;
    public static event Action OnSwipeDown;
    public static event Action OnDoubleTap;

    #endregion

    #region Unity Methods
    void Update()
    {
        DetectSwipe();
        DetectDoubleTap();
        // Mouseclick denemesi
        //DetectMouseSwipe();
        //DetectMouseDoubleClick();
    }

    #endregion

    #region Gesture Detection Methods

    /// <summary>
    /// Detects swipes
    /// </summary>
    void DetectSwipe()
    {
        if (Input.touchCount != 1)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = touch.position;
                break;

            case TouchPhase.Ended:
                touchEndPos = touch.position;
                Vector2 direction = touchEndPos - touchStartPos;

                if (direction.magnitude >= swipeThreshold)
                {
                    direction.Normalize();

                    // Swipe Up
                    if (Vector2.Dot(Vector2.up, direction) > 0.7f)
                    {
                        OnSwipeUp?.Invoke();
                    }
                    // Swipe Down
                    else if (Vector2.Dot(Vector2.down, direction) > 0.7f)
                    {
                        OnSwipeDown?.Invoke();
                    }
                    // Swipe Right
                    else if (Vector2.Dot(Vector2.right, direction) > 0.7f)
                    {
                        OnSwipeRight?.Invoke();
                    }
                    // Swipe Left
                    else if (Vector2.Dot(Vector2.left, direction) > 0.7f)
                    {
                        OnSwipeLeft?.Invoke();
                    }
                }
                break;
        }
    }

    /// <summary>
    /// detects double taps
    /// </summary>
    void DetectDoubleTap()
    {
        if (Input.touchCount != 1)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Ended)
            return;

        if (!oneTap)
        {
            oneTap = true;
            firstTapPosition = touch.position; 

            // Stop if you have previous coroutine
            if (doubleTapCoroutine != null)
            {
                StopCoroutine(doubleTapCoroutine);
            }

            // Start new coroutine and save reference
            doubleTapCoroutine = StartCoroutine(DoubleTapDelay());
        }
        else
        {
            // check the position of the second touch
            if (Vector2.Distance(firstTapPosition, touch.position) <= positionThreshold)
            {
                oneTap = false;
                OnDoubleTap?.Invoke(); 
            }
            else
            {
                oneTap = true; 
                // record the new position
                firstTapPosition = touch.position; 
            }
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Regulates double tap delay
    /// </summary>
    /// <returns></returns>
    IEnumerator DoubleTapDelay()
    {
        float timer = 0f;
        while (timer < doubleTapTime)
        {
            if (Input.touchCount > 0 && Vector2.Distance(firstTapPosition, Input.GetTouch(0).position) > positionThreshold)
            {
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
        oneTap = false;
        doubleTapCoroutine = null;
    }

    #endregion

    #region MouseClick Deneme
    /*
    void DetectMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchEndPos = Input.mousePosition;
            Vector2 direction = touchEndPos - touchStartPos;

            if (direction.magnitude >= swipeThreshold)
            {
                direction.Normalize();

                // Swipe Up
                if (Vector2.Dot(Vector2.up, direction) > 0.7f)
                {
                    OnSwipeUp?.Invoke();
                }
                // Swipe Down
                else if (Vector2.Dot(Vector2.down, direction) > 0.7f)
                {
                    OnSwipeDown?.Invoke();
                }
                // Swipe Right
                else if (Vector2.Dot(Vector2.right, direction) > 0.7f)
                {
                    OnSwipeRight?.Invoke();
                }
                // Swipe Left
                else if (Vector2.Dot(Vector2.left, direction) > 0.7f)
                {
                    OnSwipeLeft?.Invoke();
                }
            }
        }
    }

    void DetectMouseDoubleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!oneTap)
            {
                oneTap = true;

                if (doubleTapCoroutine != null)
                {
                    StopCoroutine(doubleTapCoroutine);
                }

                doubleTapCoroutine = StartCoroutine(DoubleTapDelay());
            }
            else
            {
                oneTap = false;
                OnDoubleTap?.Invoke();
            }
        }
    }
    */
    #endregion
}
