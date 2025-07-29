using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainMagnusBlackstormBackCannonAnimation : MonoBehaviour
{
    #region Fields

    Animator animator;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// called from CaptainMagnusBlackstormBehaviour script.
    /// </summary>
    public void PlayAttack()
    {
        animator.SetTrigger("CaptainMagnusBlackstormBackCannonHumanTrigger");
    }

    #endregion
}
