using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jack Grim's projectile behavior.
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player VURULDU!");

            other.GetComponent<PlayerHealth>()?.GameOver();

            Destroy(gameObject);
        }
    }
}
