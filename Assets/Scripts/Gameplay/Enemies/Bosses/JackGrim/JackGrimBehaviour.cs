using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackGrimBehaviour : MonoBehaviour
{
    #region Fields

    private float laneChangeCooldown = 5f;
    private float forceStrength = 20f;
    private int currentLane;

    Rigidbody rb;

    // Rotation correction support
    [Header("Rotation Stabilizer")]
    [SerializeField] RotationStabilizer rotationStabilizer;

    // Projectile support
    [SerializeField] PlayerReference player;
    [SerializeField] private Transform frontLaunchPoint;
    [SerializeField] private Transform backLaunchPoint;
    [SerializeField] private GameObject projectilePrefab;
    float launchForce = 40f;

    // Animation support
    [Header("Animation")]
    [SerializeField] JackGrimFrontHumanAnimation frontAnimation;
    [SerializeField] JackGrimFrontHumanAnimation backAnimation;
    [SerializeField] JackGrimHimselfHumanAnimation selfAnimation;

    // Approach support
    float approachTargetZ = 25f;
    float approachSpeed = 25f;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentLane = 1;
        StartCoroutine(ApproachToPlayArea());
    }

    #endregion

    #region Private Methods

    private IEnumerator LaneChangeLoop()
    {
        while (true)
        {
            laneChangeCooldown = Random.Range(3f, 6f);
            yield return new WaitForSeconds(laneChangeCooldown);

            int targetLane = GetTargetLane(currentLane);
            if (targetLane == currentLane)
                continue;

            bool right = targetLane > currentLane;

            // Trigger all animations
            frontAnimation?.PlayAttack();
            backAnimation?.PlayAttack();
            selfAnimation?.PlayAttack();
            yield return new WaitForSeconds(0.5f); // Wait for the attack animation to start

            // 1. Rotate çapraza
            Quaternion diagonalRot = Quaternion.Euler(0, right ? 45f : -45f, 0);
            if (rotationStabilizer != null)
                rotationStabilizer.enabled = false; // Disable rotation stabilizer during the attack
            yield return StartCoroutine(RotateTo(diagonalRot));

            // 2. Attack
            DoAttack();

            // 3. Move to new lane
            yield return StartCoroutine(MoveToLane(targetLane));

            // 4. Düz hizaya dön
            yield return StartCoroutine(RotateTo(Quaternion.Euler(0, 0, 0)));
            if (rotationStabilizer != null)
                rotationStabilizer.enabled = true; // Re-enable rotation stabilizer

            currentLane = targetLane;
        }
    }

    private IEnumerator RotateTo(Quaternion targetRot, float duration = 0.5f)
    {
        Quaternion startRot = transform.rotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;     // 0–1 arasý
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        transform.rotation = targetRot;
    }

    private IEnumerator MoveToLane(int lane)
    {
        float targetX = LaneManager.instance.GetLanePosition(lane);

        while (Mathf.Abs(transform.position.x - targetX) > 0.1f)
        {
            float multiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
            Vector3 force = Vector3.right * Mathf.Sign(targetX - transform.position.x) * forceStrength * multiplier;
            rb.AddForce(force, ForceMode.Force);
            yield return null;
        }

        // stop movement and set final position
        rb.velocity = new Vector3(0f, 0f, 0f);
        yield return StartCoroutine(SmoothMoveToLane(targetX, approachTargetZ));
    }

    /// <summary>
    /// The goal is to prevent the forward movement bug and ensure that the object stays properly in the desired position.
    /// </summary>
    /// <remarks>This method interpolates the object's position from its current position to the target
    /// position using linear interpolation (Lerp). The movement is performed over the specified duration and is
    /// frame-rate independent. The final position is explicitly set to ensure precise alignment with the
    /// target.</remarks>
    /// <param name="targetX">The target X-coordinate to move to.</param>
    /// <param name="targetZ">The target Z-coordinate to move to.</param>
    /// <param name="duration">The duration, in seconds, over which the movement should occur. Defaults to 0.6 seconds.</param>
    /// <returns>An enumerator that performs the smooth movement when used in a coroutine.</returns>
    private IEnumerator SmoothMoveToLane(float targetX, float targetZ, float duration = 0.6f)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(targetX, transform.position.y, targetZ);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // Hedefte tam hizala
    }

    private void DoAttack()
    {
        GameObject playerObject = player.player;
        if (playerObject == null || projectilePrefab == null) return;
        // Adjust the target position to be slightly above the player to avoid collision issues(gravity)
        Vector3 adjustedTarget = playerObject.transform.position + Vector3.up;

        // FRONT projectile
        GameObject frontProj = Instantiate(projectilePrefab, frontLaunchPoint.position, Quaternion.identity);
        Rigidbody frontRb = frontProj.GetComponent<Rigidbody>();
        Vector3 dirFront = (adjustedTarget - frontLaunchPoint.position).normalized;
        frontRb.AddForce(dirFront * launchForce, ForceMode.Impulse);

        // BACK projectile
        GameObject backProj = Instantiate(projectilePrefab, backLaunchPoint.position, Quaternion.identity);
        Rigidbody backRb = backProj.GetComponent<Rigidbody>();
        Vector3 dirBack = (adjustedTarget - backLaunchPoint.position).normalized;
        backRb.AddForce(dirBack * launchForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Determines the target lane based on the specified input lane.
    /// </summary>
    /// <param name="lane">The input lane, represented as an integer.</param>
    /// <returns>The target lane corresponding to the input lane. If the input lane is not recognized,  the method returns the
    /// input lane as a fallback.</returns>
    private int GetTargetLane(int lane)
    {
        //switch (lane)
        //{
        //    case 0: return 2;
        //    case 1: return 3;
        //    case 2: return 0;
        //    case 3: return 1;
        //    default: return lane; // fallback
        //}
        switch (lane)
        {
            case 0:
                if (Random.value < 0.5f)
                    return 2;
                else
                    return 3;
            case 1: return 3;
            case 2: return 0;
            case 3:
                if (Random.value < 0.5f)
                    return 1;
                else
                    return 0;
            default: return lane;
        }
    }

    /// <summary>
    /// Moves the Jack Grim to the play area at a constant speed.
    /// Then starts the lane change loop.
    /// </summary>
    /// <returns></returns>
    IEnumerator ApproachToPlayArea()
    {
        while (transform.position.z > approachTargetZ)
        {
            float distanceToTarget = transform.position.z - approachTargetZ;
            float mult = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);

            // slow down when 40 units away
            float slowDownStartDistance = 40f; 
            float t = Mathf.Clamp01(distanceToTarget / slowDownStartDistance);

            Vector3 vel = rb.velocity;
            vel.z = -approachSpeed * mult * t; // sabit hýz
            rb.velocity = vel;
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector3.zero;
        // Hedefe tam hizala (18 f kullanýyorsan ona çek)
        transform.position = new Vector3(transform.position.x, transform.position.y, approachTargetZ);
        
        StartCoroutine(LaneChangeLoop());
    }

    #endregion
}
