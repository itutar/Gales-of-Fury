using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistoleerHelpShipPistoleerAnim : MonoBehaviour, IAttackAnimator
{
    Animator animator;
    [SerializeField] ParticleSystem muzzleEffect;

    void Start()
    {
        animator = this.GetComponent<Animator>();
    }
    void Update()
    {
        // for test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAttackAnimation();
        }
    }

    #region Public Methods

    /// <summary>
    /// Called from animation event to start the pistoleer attack muzzle VFX
    /// </summary>
    public void StartPistoleerAttackVfx()
    {
        muzzleEffect.Play();
    }

    public void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("EricBladesPistoleerAttackTrigger");
        }
    }

    #endregion
}
