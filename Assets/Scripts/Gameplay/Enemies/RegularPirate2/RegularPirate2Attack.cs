using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularPirate2Attack : MonoBehaviour, IAttackController
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

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float shootInterval = 1.5f;
    [SerializeField] private float spacing = 0.7f;
    [SerializeField] private float offsetZ = -4f;
    // Height offset for arrow spawn position
    private const float heightOffset = 0.6f;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogError("Arrow prefab or fire point not assigned in RegularPirate2Attack.");
        }

        StartCoroutine(AttackRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Manages the attack routine for Regular Pirate 2.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackRoutine()
    {
        int maxAttacks = 2;
        float attackInterval = shootInterval; // for consistency

        // Wait first
        yield return new WaitForSeconds(0.5f);

        for (int attackCount = 0; attackCount < maxAttacks; attackCount++)
        {
            FireArrowWaves();

            // Don't wait on the last attack
            if (attackCount < maxAttacks - 1)
            {
                yield return new WaitForSeconds(attackInterval);
            }
        }

        // Trigger call token drop chance (10%)
        if (Random.Range(0f, 100f) < 10f)
        {
            EnemyEventManager.Instance.OnEnemyCallTokenDrop.Invoke(gameObject);
        }

        // Trigger disappear
        EnemyDisappear disappearComponent = GetComponent<EnemyDisappear>();
        if (disappearComponent != null)
        {
            disappearComponent.SetDisappearType(DisappearType.MoveToTopCornerAndDisappear);
        }

        EnemyEventManager.Instance.OnEnemyDisappear.Invoke(gameObject);

        // attack is finished
        DisableAttack();
    }

    /// <summary>
    /// Fires waves of arrows
    /// </summary>
    private void FireArrowWaves()
    {
        Vector3 shipRight = transform.right;
        Vector3 fireOrigin = firePoint.position;

        for (int i = 0; i < 2; i++)
        {
            // Right
            Vector3 rightPos = fireOrigin + shipRight * (i + 1) * spacing + transform.forward * offsetZ;
            rightPos.y += heightOffset; // Adjust height if needed
            SpawnArrow(rightPos, -transform.forward);

            // Left
            Vector3 leftPos = fireOrigin - shipRight * (i + 1) * spacing + transform.forward * offsetZ;
            leftPos.y += heightOffset; // Adjust height if needed
            SpawnArrow(leftPos, -transform.forward);
        }
    }

    /// <summary>
    /// Spawns a single arrow at a given position and direction.
    /// </summary>
    private void SpawnArrow(Vector3 position, Vector3 direction)
    {
        Instantiate(arrowPrefab, position, Quaternion.LookRotation(direction));
    }

    #endregion
}
