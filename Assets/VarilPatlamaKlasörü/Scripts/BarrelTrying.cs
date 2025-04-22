using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BarrelTrying : MonoBehaviour
{
    [SerializeField] private ParticleSystem barrelFuse;
    [SerializeField] private ParticleSystem barrelExplosion;
    [SerializeField] private float fuseStartDelay = 5f;
    // Start is called before the first frame update
    void Start()
    {
        barrelExplosion = barrelExplosion.GetComponent<ParticleSystem>();
        StartFuse();
        barrelExplosion.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartFuse()
    {
        barrelExplosion.Play(true);
        StartCoroutine(StopFuse());
    }

    //private IEnumerator StartFuse()
    //{
    //    yield return new WaitForSeconds(fuseStartDelay);
    //    barrelFuse.Play();
    //    StartCoroutine(StopFuse());
    //}

    private IEnumerator StopFuse()
    {
        yield return new WaitForSeconds(fuseStartDelay);
        barrelFuse.Stop();
        barrelExplosion.Play();

    }
}
