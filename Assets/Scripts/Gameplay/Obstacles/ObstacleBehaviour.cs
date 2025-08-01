using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehaviour : MonoBehaviour
{
    private float speed = 15f;

    [Header("Player Reference")]
    [SerializeField] private PlayerReference playerReference;

    private bool hasPassedPlayer = false;

    private Rigidbody rb;

    private float speedMultiplier = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Subscribe to Blackboard SpeedMultiplier changes
        Blackboard.Instance.Subscribe<float>(BlackboardKey.SpeedMultiplier, OnSpeedMultiplierChanged);

        // Initialize from Blackboard
        speedMultiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
        if (speedMultiplier <= 0) speedMultiplier = 1f;
    }

    private void FixedUpdate()
    {
        // Apply forward motion with SpeedMultiplier
        float currentSpeed = Mathf.Clamp(speedMultiplier / 1.5f, 0.5f, 3f) * speed;
        rb.AddForce(Vector3.back * currentSpeed, ForceMode.Acceleration);

        // Early-out if we already triggered the destroy coroutine
        if (hasPassedPlayer) return;

        // Safety checks
        if (playerReference == null || playerReference.player == null) return;

        // Has the log moved behind the player?
        if (transform.position.z < playerReference.player.transform.position.z)
        {
            hasPassedPlayer = true;
            StartCoroutine(DestroyAfterDelay(3f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If collides with player, call TakeDamage
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
        }
    }

    /// <summary>
    /// Waits 'delay' seconds, then destroys this GameObject.
    /// </summary>
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnSpeedMultiplierChanged(float newMultiplier)
    {
        speedMultiplier = newMultiplier;
    }
}
