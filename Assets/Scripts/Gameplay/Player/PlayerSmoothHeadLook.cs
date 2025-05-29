using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerSmoothHeadLook : MonoBehaviour
{
    #region Fields

    public MultiAimConstraint headConstraint;
    [SerializeField] float blendSpeed = 2f;

    float targetWeight = 0f;

    // bool to rotate the head
    bool rotateHead = true;

    #endregion

    #region Unity Methods

    private void Update()
    {
        headConstraint.weight = Mathf.Lerp(headConstraint.weight, targetWeight, blendSpeed * Time.deltaTime);
    }

    #endregion

    #region Public Methods

    public void ChangeRotateHeadBool()
    {
        rotateHead = !rotateHead;
        if (rotateHead)
        {
            targetWeight = 1f;
        }
        else
        {
            targetWeight = 0f;
        }
    }

    #endregion
}
