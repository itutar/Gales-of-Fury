using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErikBladesPistoleerAnimation : MonoBehaviour
{
    #region Fields

    Animator animator;
    [SerializeField] ParticleSystem muzzleEffect;

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
    /// Called by ErikBladesBehaviour
    /// </summary>
    public void PlayAttack()
    {
        animator.SetTrigger("EricBladesPistoleerAttackTrigger");
    }


    /// <summary>
    /// Called from animation event to start the pistoleer attack muzzle VFX
    /// </summary>
    public void StartPistoleerAttackVfx()
    {
        muzzleEffect.Play();
    }

    #endregion
}
