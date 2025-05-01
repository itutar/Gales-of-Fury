using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkAnimation : MonoBehaviour
{
    #region Fields

    Animator animator;
    [SerializeField] ParticleSystem motionLines;

    #endregion

    #region Unity Methods

    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        // for test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("SharkAttackTrigger");
        }
    }


    #endregion

    #region Public Methods

    /// <summary>
    /// Called from animation event to start the shark attack VFX
    /// </summary>
    public void StartSharkAttackVfx()
    {
        motionLines.Play();
    }

    #endregion
}
