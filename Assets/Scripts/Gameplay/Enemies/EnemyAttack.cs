using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    #region Fields

    [SerializeField] EnemyType enemyType;
    
    // Indicates whether the attack animation has finished
    private bool isSharkAttackAnimationFinished = false;
    private bool isPirateAttackAnimationFinished = false;
    private bool isKrakenAttackAnimationFinished = false;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        EnemyEventManager.Instance.OnEnemyAttack.AddListener(OnAttack);
    }

    private void OnDisable()
    {
        EnemyEventManager.Instance?.OnEnemyAttack.RemoveListener(OnAttack);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles the attack event based on the enemy type.
    /// </summary>
    /// <param name="enemy">The enemy performing the attack</param>
    private void OnAttack(GameObject enemy)
    {
        if (enemy == this.gameObject)
        {
            switch (enemyType)
            {
                case EnemyType.Shark:
                    SharkAttack();
                    break;
                case EnemyType.RegularPirate1:
                    PirateAttack();
                    break;
                case EnemyType.Kraken:
                    KrakenAttack();
                    break;
            }
        }
    }

    private void SharkAttack()
    {
        // Shark'ýn saldýrý kodlarý
        isSharkAttackAnimationFinished = true;
        // Initiate the EnemyCallToken event after the attack is finished
        // Initiate the disappear event after the attack is finished
        if (isSharkAttackAnimationFinished)
        {
            GameObject attachedObject = gameObject;
            // Invoke the OnEnemyCallTokenDrop event with a 10% probability
            if (Random.Range(0f, 100f) < 10f)
            {
                EnemyEventManager.Instance.OnEnemyCallTokenDrop.Invoke(attachedObject);
            }
            EnemyEventManager.Instance.OnEnemyDisappear.Invoke(attachedObject);
            isSharkAttackAnimationFinished = false;
        }
    }

    private void PirateAttack()
    {
        // Pirate'ýn saldýrý kodlarý
        isPirateAttackAnimationFinished = true;
        // Initiate the EnemyCallToken event after the attack is finished
        // Initiate the disappear event after the attack is finished
        if (isPirateAttackAnimationFinished)
        {
            GameObject attachedObject = gameObject;
            // Invoke the OnEnemyCallTokenDrop event with a 10% probability
            if (Random.Range(0f, 100f) < 10f)
            {
                EnemyEventManager.Instance.OnEnemyCallTokenDrop.Invoke(attachedObject);
            }
            EnemyEventManager.Instance.OnEnemyDisappear.Invoke(attachedObject);
            isPirateAttackAnimationFinished = false;
        }
    }

    private void KrakenAttack()
    {
        // Kraken'in saldýrý kodlarý
        isKrakenAttackAnimationFinished = true;
        // Initiate the EnemyCallToken event after the attack is finished
        // Initiate the disappear event after the attack is finished
        if (isKrakenAttackAnimationFinished)
        {
            GameObject attachedObject = gameObject;
            // Invoke the OnEnemyCallTokenDrop event with a 10% probability
            if (Random.Range(0f, 100f) < 10f)
            {
                EnemyEventManager.Instance.OnEnemyCallTokenDrop.Invoke(attachedObject);
            }
            EnemyEventManager.Instance.OnEnemyDisappear.Invoke(attachedObject);
            isKrakenAttackAnimationFinished = false;
        }
    }


    #endregion
}
