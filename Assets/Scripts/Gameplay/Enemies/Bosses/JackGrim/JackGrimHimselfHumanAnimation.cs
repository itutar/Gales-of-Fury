using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackGrimHimselfHumanAnimation : MonoBehaviour
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
    /// Called by JackGrimBehaviour
    /// </summary>
    public void PlayAttack()
    {
        animator.SetTrigger("JackGrimHimselfHumanAttackOrderTrigger");
        // The animation event will still call StartJackGrimHimselfHumanAttackVfx()
    }

    /// <summary>
    /// Called from animation event to start the Jack Grim Himself Human attack muzzle VFX
    /// </summary>
    public void StartJackGrimHimselfHumanAttackVfx()
    {
        muzzleEffect.Play();
    }

    #endregion
}
