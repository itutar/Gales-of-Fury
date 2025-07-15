using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackGrimFrontHumanAnimation : MonoBehaviour
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
    /// Called by JackGrimBehaviour
    /// </summary>
    public void PlayAttack()
    {
        animator.SetTrigger("JackGrimFrontHumanAttackTrigger");
    }

    #endregion

}
