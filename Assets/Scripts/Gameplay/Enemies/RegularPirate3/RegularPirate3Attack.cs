using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularPirate3Attack : MonoBehaviour, IAttackController
{
    #region IAttackController Implementation

    public void EnableAttack()
    {
        this.enabled = true;
    }

    public void DisableAttack()
    {
        this.enabled = false;
    }

    #endregion

    #region Fields

    [SerializeField]
    RegularPirate3Human3Animation animComponent;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void OnEnable()
    {
        if (animComponent == null)
        {
            Debug.LogError("RegularPirate3Human3Animation component is not assigned in RegularPirate3Attack.");
        }

        StartCoroutine(AttackRoutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// manages the attack routine for Regular Pirate 3.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackRoutine()
    {
        float attackInterval = 5f;
        int maxAttacks = 2;

        // Wait first
        yield return new WaitForSeconds(attackInterval);

        for (int attackCount = 0; attackCount < maxAttacks; attackCount++)
        {
            animComponent.OnAttack();
            // dont wait on the last attack
            if (attackCount < maxAttacks - 1)
            {
                yield return new WaitForSeconds(attackInterval);
            }
        }

        // After the last attack, wait for the animation to finish
        yield return new WaitForSeconds(2f);

        // After the last attack, trigger disappear with chosen type
        EnemyDisappear disappearComponent = GetComponent<EnemyDisappear>();
        if (disappearComponent != null)
        {
            disappearComponent.SetDisappearType(DisappearType.MoveToTopCornerAndDisappear);
        }

        EnemyEventManager.Instance.OnEnemyDisappear.Invoke(gameObject);

        // attack is finished
        DisableAttack();
    }

    #endregion
}
