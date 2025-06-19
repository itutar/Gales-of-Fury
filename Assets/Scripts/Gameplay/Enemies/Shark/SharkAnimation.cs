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

    #endregion

    #region Public Methods

    /// <summary>
    /// Called from animation event to start the shark attack VFX
    /// </summary>
    public void StartSharkAttackVfx()
    {
        motionLines.Play();
    }

    /// <summary>
    /// triggers the shark attack animation.
    /// </summary>
    public void TriggerAttackAnimation()
    {
        animator.SetTrigger("SharkAttackTrigger");
    }

    #endregion
}
