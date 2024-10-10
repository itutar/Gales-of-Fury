using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Private Methods

    void HandleSwipeRight()
    {
        Debug.Log("Sa�a Kayd�r�ld�");
    }

    void HandleSwipeLeft()
    {
        Debug.Log("Sola Kayd�r�ld�");
    }

    void HandleSwipeUp()
    {
        Debug.Log("Yukar� Kayd�r�ld�");
    }

    void HandleSwipeDown()
    {
        Debug.Log("A�a�� Kayd�r�ld�");
    }

    void HandleDoubleTap()
    {
        Debug.Log("�ift T�klama Alg�land�");
    }

    #endregion
}
