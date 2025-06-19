using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularPirate1Attack : MonoBehaviour, IAttackController
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

    [SerializeField] private  ParticleSystem burstSpeedVFX;
    [SerializeField] private float shootInterval = 2.5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float burstForce = 250f;

    #endregion

    #region Unity Methods

    void Awake()
    {
        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(AttackRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // player collision control
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.GameOver();
            }
        }
    }

    #endregion

    #region Private Methods

    private IEnumerator AttackRoutine()
    {
        // Initial delay
        yield return new WaitForSeconds(shootInterval);

        if (rb != null)
        {
            Vector3 burstDirection = Vector3.back;
            rb.AddForce(burstDirection * burstForce, ForceMode.Impulse);
        }

        // VFX çalýþtýr
        if (burstSpeedVFX != null)
        {
            burstSpeedVFX.Play();
        }

        yield return new WaitForSeconds(1f); // bitiþ animasyon süresi

        // Disappear ayarý
        EnemyDisappear disappearComponent = GetComponent<EnemyDisappear>();
        if (disappearComponent != null)
        {
            disappearComponent.SetDisappearType(DisappearType.StayBehind);
        }

        EnemyEventManager.Instance.OnEnemyDisappear.Invoke(gameObject);

        // Kendini kapat
        DisableAttack();
    }

    #endregion
}
