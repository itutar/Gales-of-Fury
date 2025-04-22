using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlay : MonoBehaviour
{
    [SerializeField] private ParticleSystem myParticle;
    // Start is called before the first frame update
    void Start()
    {
        myParticle = GetComponent<ParticleSystem>();
        myParticle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
