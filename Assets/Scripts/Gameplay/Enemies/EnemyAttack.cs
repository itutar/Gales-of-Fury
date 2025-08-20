using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    #region Fields

    [SerializeField] EnemyType enemyType;
    
    // Regular pirate2 fields
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float shootInterval = 5f;
    [SerializeField] private float spacing = 1.0f;
    [SerializeField] private float offsetZ = 1f;
    private int pirate2AttackCount = 0;

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
                case EnemyType.RegularPirate2:
                    RegularPirate2Attack();
                    break;
                case EnemyType.RegularPirate3:
                    RegularPirate3Attack(); 
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
    #region RegularPirate2

    private void RegularPirate2Attack()
    {
        pirate2AttackCount++;
        FireArrowWaves();

        if (pirate2AttackCount < 2)
        {
            Invoke(nameof(RegularPirate2Attack), shootInterval);
        }
        else
        {
            if (Random.Range(0f, 100f) < 10f)
            {
                EnemyEventManager.Instance.OnEnemyCallTokenDrop.Invoke(gameObject);
            }
            EnemyEventManager.Instance.OnEnemyDisappear.Invoke(gameObject);
        }
    }

    private void FireArrowWaves()
    {
        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Arrow prefab or fire point not assigned.");
            return;
        }

        Vector3 shipRight = transform.right;
        Vector3 fireOrigin = firePoint.position;

        for (int i = 0; i < 5; i++)
        {
            // Saða
            Vector3 rightPos = fireOrigin + shipRight * (i + 1) * spacing + transform.forward * offsetZ;
            SpawnArrow(rightPos, -transform.forward);

            // Sola
            Vector3 leftPos = fireOrigin - shipRight * (i + 1) * spacing + transform.forward * offsetZ;
            SpawnArrow(leftPos, -transform.forward);
        }
    }

    private void SpawnArrow(Vector3 position, Vector3 direction)
    {
        GameObject arrow = Instantiate(arrowPrefab, position, Quaternion.LookRotation(direction));
    }

    #endregion

    #region RegularPirate3

    private void RegularPirate3Attack()
    {

    }

    #endregion
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
