using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveEnemyTowardsTargetLane : MonoBehaviour, IMoveToLane
{
    float forwardForce = 75f;   // push strength on -Z
    float stopAheadDistance = 25f;   // how far in front of player to stop
    float stopThreshold = 0.2f;  // precision on Z check

    [Header("Player Reference")]
    [SerializeField] private PlayerReference player;

    Rigidbody rb;
    Transform playerT;
    float targetZ;
    public bool IsFinished { get; private set; }

    // Flag to track collision state
    private bool isCollidingWithEnemy = false;

    /*  We keep the old parameter so other code (sharks etc.) compiles,
        but X-movement is no longer needed. */
    public void Initialize(float _ignoredTargetX)
    {
        GameObject playerObject = player?.player;
        if (playerObject == null)
        {
            Debug.LogWarning($"{name}: PlayerReference boş!");
            enabled = false;
            return;
        }

        playerT = playerObject.transform;
        UpdateTargetZ();

        IsFinished = false;
        enabled = true;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    //void OnDestroy()
    //{
        // Reset collision matrix layer adjustment so enemy can collide with other enemies again
        //AdjustCollisionLayer(false);
    //}

    void FixedUpdate()
    {
        if (IsFinished || playerT == null) return;

        // If colliding with another enemy, stop Z movement
        if (isCollidingWithEnemy)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);
            return;
        }

        UpdateTargetZ();                      // keep target fresh – player is moving

        float deltaZ = transform.position.z - targetZ; // +ve when still ahead
        if (deltaZ > stopThreshold)
        {
            float mult = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
            rb.AddForce(Vector3.back * forwardForce * mult, ForceMode.Force);
        }
        else
        {
            // reached attack point → stop and hand over to attack controller
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);
            IsFinished = true;

            // collision matrix layer adjusment so enemy doesn't collide with other enemies
            //AdjustCollisionLayer(true);

            enabled = false;
        }
    }

    void UpdateTargetZ()
    {
        if (playerT != null)
            targetZ = playerT.position.z + stopAheadDistance;
    }

    /// <summary>
    /// Enables or disables collision detection between objects in the "Enemy" layer.
    /// </summary>
    /// <param name="ignoreCollision">
    /// A value indicating whether collisions between objects in the "Enemy" layer should be ignored.
    /// true to ignore collisions; otherwise, false.
    /// </param>
    void AdjustCollisionLayer(bool ignoreCollision)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics.IgnoreLayerCollision(enemyLayer, enemyLayer, ignoreCollision);
    }

    // Detect collision start with another "Enemy"
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Compare Z positions: the one with greater Z (further back) stops
            if (transform.position.z > collision.transform.position.z)
            {
                isCollidingWithEnemy = true; // This one is behind -> stop
            }
            else
            {
                isCollidingWithEnemy = false; // This one is ahead -> keep moving
            }
        }
    }

    // Detect collision end with another "Enemy"
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isCollidingWithEnemy = false;
        }
    }
}
