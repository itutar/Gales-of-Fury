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

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Private Methods

    private IEnumerator StopFuse()
    {
        yield return new WaitForSeconds(fuseBurningTime);
        barrelFuseParticle.Stop();
        transform.rotation = Quaternion.identity;
        barrelExplosionParticle.Play();
        BarrelExplosion.transform.parent = null;
        Destroy(gameObject);

    }

    #endregion
}
