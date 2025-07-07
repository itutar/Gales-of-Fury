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

    // Update is called once per frame
    void Update()
    {
        // For testing purposes
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("JackGrimFrontHumanAttackTrigger");
        }
    }

    #endregion
}
