using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the animation for the Archer help ship archer human.
/// </summary>
public class ArcherHelpShipArcherAnim : MonoBehaviour, IAttackAnimator
{
    #region Fields

    Animator animator;

    #endregion

    #region Unity Methods

    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }
    private void Update()
    {
        // For testing purposes, you can uncomment the following line to trigger the attack animation with the space key.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAttackAnimation();
        }
    }
    #endregion

    #region Public Methods

    public void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("IsabellaIronheartArcherAttackTrigger");
        }
    }

    #endregion
}
