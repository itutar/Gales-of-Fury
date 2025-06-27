using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenAttack : MonoBehaviour
{
    private bool canTrigger = false;

    private void Start()
    {
        // 0.7 saniye sonra kontrol etmeye baþla
        StartCoroutine(EnableTriggerAfterDelay(0.7f));
    }

    private IEnumerator EnableTriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canTrigger = true;
        yield return new WaitForSeconds(0.5f); 
        canTrigger = false; 
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canTrigger) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.GameOver();
                canTrigger = false; // bir kere tetiklenmesini saðla
            }
        }
    }
}
