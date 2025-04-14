using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelTrying : MonoBehaviour
{
    [SerializeField] private ParticleSystem barrelFuse;
    [SerializeField] private float fuseStartDelay = 5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartFuse());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator StartFuse()
    {
        yield return new WaitForSeconds(fuseStartDelay);
        barrelFuse.Play();
        StartCoroutine(StopFuse());
    }

    private IEnumerator StopFuse()
    {
        yield return new WaitForSeconds(fuseStartDelay);
        barrelFuse.Stop();
    }
}
