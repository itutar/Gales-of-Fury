using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErikBladesPistoleerAnimation : MonoBehaviour
{
    #region Fields

    Animator animator;
    [SerializeField] GameObject Parent;
    [SerializeField] ParticleSystem muzzleEffect;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        EnemyEventManager.Instance.OnEnemyAttack.AddListener(OnAttack);
    }

    // Update is called once per frame
    void Update()
    {
        // for test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("EricBladesPistoleerAttackTrigger");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles the attack event based on the enemy type.
    /// </summary>
    /// <param name="enemy">The enemy performing the attack</param>
    private void OnAttack(GameObject enemy)
    {
        if (enemy == Parent)
        {
            animator.SetTrigger("EricBladesPistoleerAttackTrigger");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Called from animation event to start the pistoleer attack muzzle VFX
    /// </summary>
    public void StartPistoleerAttackVfx()
    {
        muzzleEffect.Play();
    }

    #endregion
}
