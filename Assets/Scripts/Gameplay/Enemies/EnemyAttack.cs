using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    #region Fields

    [SerializeField] EnemyType enemyType;

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
    /// <param name="target">The target being attacked by the enemy</param>
    private void OnAttack(GameObject enemy, GameObject target)
    {
        if (enemy == this.gameObject)
        {
            switch (enemyType)
            {
                case EnemyType.Shark:
                    SharkAttack(target);
                    break;
                case EnemyType.RegularPirate1:
                    PirateAttack(target);
                    break;
                case EnemyType.Kraken:
                    KrakenAttack(target);
                    break;
            }
        }
    }

    private void SharkAttack(GameObject target)
    {
        // Shark'ýn saldýrý kodlarý
        Debug.Log("Shark is attacking " + target.name);
    }

    private void PirateAttack(GameObject target)
    {
        // Pirate'ýn saldýrý kodlarý
        Debug.Log("Regular Pirate is attacking " + target.name);
    }

    private void KrakenAttack(GameObject target)
    {
        // Kraken'in saldýrý kodlarý
        Debug.Log("Kraken is attacking " + target.name);
    }


    #endregion
}
