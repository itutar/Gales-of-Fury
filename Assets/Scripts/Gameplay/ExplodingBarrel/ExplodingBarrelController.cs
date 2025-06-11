using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrelController : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject BarrelExplosion;
    [SerializeField] private GameObject BarrelFuse;
    private ParticleSystem barrelExplosionParticle;
    private ParticleSystem barrelFuseParticle;
    [SerializeField] private float fuseBurningTime = 5f;
    [SerializeField] private float explosionRadius = 2.5f;

    [SerializeField] Rigidbody barrelRigidbody;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        barrelExplosionParticle = BarrelExplosion.GetComponent<ParticleSystem>();
        barrelFuseParticle = BarrelFuse.GetComponent<ParticleSystem>();
        barrelFuseParticle.Play();
        StartCoroutine(StopFuse());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void FixedUpdate()
    {
        barrelRigidbody?.AddForce(Vector3.back * 10f, ForceMode.Force);
    }

    #endregion

    #region Private Methods

    private IEnumerator StopFuse()
    {
        yield return new WaitForSeconds(fuseBurningTime);
        barrelFuseParticle.Stop();
        transform.rotation = Quaternion.identity;
        barrelExplosionParticle.Play();

        // explosion moment damage check
        var hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player hit by explosion!");
                var health = hit.GetComponent<PlayerHealth>();
                if (health != null)
                    health.TakeDamage();
            }
        }

        BarrelExplosion.transform.parent = null;
        Destroy(gameObject);

    }

    #endregion
}
