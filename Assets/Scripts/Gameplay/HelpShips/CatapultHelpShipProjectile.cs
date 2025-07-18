using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultHelpShipProjectile : MonoBehaviour
{
    #region Fields

    private float lifetime = 5f;
    private float launchForce = 40f;

    [Tooltip("The projectile will be launched toward the target Transform. E�er bo� b�rak�l�rsa forward y�n�nde f�rlat�r.")]
    [SerializeField] private Transform targetTransform;

    private Rigidbody rb;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("CatapultHelpShipProjectile: Rigidbody bulunamad�!", this);
    }

    private void Start()
    {
        // destroy the projectile after a certain lifetime
        Destroy(gameObject, lifetime);

        // Hedef atanm��sa ona, atanmam��sa forward y�n�ne do�ru kuvvet uygula
        Vector3 direction = targetTransform != null
            ? (targetTransform.position - transform.position).normalized
            : transform.forward;

        rb.AddForce(direction * launchForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        Destroy(gameObject);
    }

    #endregion
}
