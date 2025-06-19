using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SharkAttack : MonoBehaviour, IAttackController
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

    [SerializeField] private SharkAnimation sharkAnimation;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float burstForce = 10f;
    [SerializeField] private float verticalForce = 2f;
    [SerializeField] private float attackDelay = 2f;

    [Header("Player Reference")]
    [SerializeField] private PlayerReference playerReference;

    private Transform playerTransform;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        if (sharkAnimation == null)
        {
            Debug.LogError("SharkAnimation component is not assigned.");
        }

        playerTransform = playerReference.player.transform;
        StartCoroutine(AttackRoutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
            else
            {
                Debug.LogWarning("PlayerHealth component is missing on the player GameObject.");
            }
        }
    }

    #endregion

    #region Private Methods

    IEnumerator AttackRoutine()
    {
        // İlk saldırıdan önce bekle
        yield return new WaitForSeconds(attackDelay);

        // Shark, player'ın Z değerinin üstündeyken saldırmaya devam et
        while (transform.position.z > playerTransform.position.z)
        {
            // 3) Sonraki saldırı için bekle
            yield return new WaitForSeconds(attackDelay);

            // 1) Burst kuvvet + VFX
            Vector3 burstDir = (Vector3.back * 3f + Vector3.up).normalized;
            rb.AddForce(burstDir * burstForce + Vector3.up * verticalForce, ForceMode.Impulse);
            sharkAnimation?.StartSharkAttackVfx();

            // 2) Saldırı animasyonu
            sharkAnimation?.TriggerAttackAnimation();

            
        }

        // Z değeri artık player'ın Z'sine inmiş → disappear
        var disappear = GetComponent<EnemyDisappear>();
        if (disappear != null)
            disappear.SetDisappearType(DisappearType.StayBehind);

        EnemyEventManager.Instance.OnEnemyDisappear?.Invoke(gameObject);
        DisableAttack();
    }

    #endregion

}
