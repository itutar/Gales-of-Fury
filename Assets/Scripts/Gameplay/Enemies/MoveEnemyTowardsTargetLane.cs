using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveEnemyTowardsTargetLane : MonoBehaviour, IMoveToLane
{
    Rigidbody rb;
    float targetXPosition;
    public float forceStrength = 20f;
    public float stopThreshold = 0.1f;

    // IMoveToLane
    public bool IsFinished { get; private set; }

    /// <summary>
    /// Tells the enemy to move towards the target lane's X position.
    /// </summary>
    /// <param name="targetXPosition">the lane's x position for enemy to move towards</param>
    public void Initialize(float targetXPosition)
    {
        this.targetXPosition = targetXPosition;
        IsFinished = false;
        enabled = true;
    }

    private void OnEnable()
    {
        // Move horizontally first when the script is started
        IsFinished = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float deltaX = targetXPosition - transform.position.x;
        if (Mathf.Abs(deltaX) > stopThreshold)
        {
            // Force direction: towards the target 1 or -1
            Vector3 force = Vector3.right * Mathf.Sign(deltaX) * forceStrength;
            rb.AddForce(force, ForceMode.Force);
        }
        else
        {
            Vector3 v = rb.velocity;
            rb.velocity = new Vector3(0f, v.y, v.z); // stop horizontal movement
            IsFinished = true;
            enabled = false; // stop this script from running
        }
    }
}
