using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperShipAttack : MonoBehaviour
{
    #region Fields

    // Helper ship type
    [SerializeField] private HelperShipType shipType;

    // animation support
    private IAttackAnimator attackAnimator;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private HelperShipDisappear disappear;
    private bool hasAttacked = false;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // get the attack animator from the child component
        attackAnimator = GetComponentInChildren<IAttackAnimator>();

        disappear = GetComponent<HelperShipDisappear>();

        // Eðer zaten offset'e ulaþýldýysa hemen saldýr
        if (Blackboard.Instance.GetValue<bool>(BlackboardKey.HelperShipReachedZOffset))
            OnReachedOffset(true);

        // Aksi halde ulaþýldýðýnda tetikle
        Blackboard.Instance.Subscribe<bool>(
            BlackboardKey.HelperShipReachedZOffset,
            OnReachedOffset
        );
    }

    #endregion

    #region Private Methods

    private void OnReachedOffset(bool reached)
    {
        if (!reached || hasAttacked)
            return;

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // wait before attack
        yield return new WaitForSeconds(1f);

        // set state and stop movement
        hasAttacked = true;

        // disable the HelperShipBehaviour component to correctly stop the x axis constraint
        var behaviour = GetComponent<HelperShipBehaviour>();
        if (behaviour != null)
            behaviour.enabled = false;

        // play attack animation
        attackAnimator?.PlayAttackAnimation();

        Fire();

        // wait before disappearing
        yield return new WaitForSeconds(1f);
        disappear?.StartDisappearance();
    }


    private void Fire()
    {
        float damage = shipType switch
        {
            HelperShipType.Cannon => 2f,
            HelperShipType.Catapult => 1f,
            HelperShipType.Archer => 0.75f,
            HelperShipType.Pistoleer => 0.5f,
            _ => 1f
        };

        BossEventManager.Instance.OnBossTakeDamage.Invoke(damage);

        if (bulletPrefab != null && firePoint != null)
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    #endregion
}
