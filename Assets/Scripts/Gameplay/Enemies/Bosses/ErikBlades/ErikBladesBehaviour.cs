using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErikBladesBehaviour : MonoBehaviour
{
    #region Fields

    private float laneChangeCooldown = 5f;
    private float forceStrength = 20f;
    private int currentLane;

    Rigidbody rb;

    [Header("Rotation Stabilizer")]
    [SerializeField] RotationStabilizer rotationStabilizer;

    [Header("Player Reference")]
    [SerializeField] PlayerReference player;

    [Header("Pistoleer Setup")]
    [SerializeField] private Transform leftPistoleerProjectileSpawnPoint;
    [SerializeField] private Transform rightPistoleerProjectileSpawnPoint;
    [SerializeField] private GameObject pistoleerBulletPrefab;

    [Header("Back Barrel Setup")]
    [SerializeField] private ErikBladesBackHumanAnimation backHumanAnimation;

    [Header("Animation")]
    [SerializeField] ErikBladesPistoleerAnimation pistoleerAnimationR1;
    [SerializeField] ErikBladesPistoleerAnimation pistoleerAnimationR2;
    [SerializeField] ErikBladesPistoleerAnimation pistoleerAnimationL1;
    [SerializeField] ErikBladesPistoleerAnimation pistoleerAnimationL2;

    private float projectileLaunchForce = 25f;

    float approachTargetZ = 30f;
    float approachSpeed = 25f;

    #endregion

    #region Unity Methods

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

            // Pistoleer Animations
            if (right)
            {
                pistoleerAnimationR1?.PlayAttack();
                pistoleerAnimationR2?.PlayAttack();
            }
            else
            {
                pistoleerAnimationL1?.PlayAttack();
                pistoleerAnimationL2?.PlayAttack();
            }

            yield return new WaitForSeconds(0.5f);

            Quaternion diagonalRot = Quaternion.Euler(0, right ? 45f : -45f, 0);
            if (rotationStabilizer != null)
                rotationStabilizer.enabled = false;
            yield return StartCoroutine(RotateTo(diagonalRot));

            DoPistoleerAttack(right);

            yield return StartCoroutine(MoveToLane(targetLane));

            yield return StartCoroutine(RotateTo(Quaternion.Euler(0, 0, 0)));
            if (rotationStabilizer != null)
                rotationStabilizer.enabled = true;

            currentLane = targetLane;

            // Back barrel attack
            BackBarrelAttack();
        }
    }

    private IEnumerator RotateTo(Quaternion targetRot, float duration = 0.5f)
    {
        Quaternion startRot = transform.rotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
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

        rb.velocity = Vector3.zero;
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

    private void DoPistoleerAttack(bool movingRight)
    {
        GameObject playerObject = player.player;
        if (playerObject == null) return;

        Vector3 adjustedTarget = playerObject.transform.position + Vector3.up * 4.5f;

        Transform spawnPoint = movingRight ? rightPistoleerProjectileSpawnPoint : leftPistoleerProjectileSpawnPoint;

        // First shot
        Vector3 gravityAdjustment = new Vector3(0, 2f, 0); // Adjust for gravity
        Vector3 dir1 = (adjustedTarget - spawnPoint.position + gravityAdjustment).normalized;
        GameObject bullet1 = Instantiate(pistoleerBulletPrefab, spawnPoint.position, Quaternion.LookRotation(dir1));
        Rigidbody rbBullet1 = bullet1.GetComponent<Rigidbody>();
        rbBullet1.AddForce(dir1 * projectileLaunchForce, ForceMode.Impulse);

        // Second shot (offset)
        Vector3 secondShotPos = spawnPoint.position + new Vector3(0.5f, 0, 0.5f);
        Vector3 dir2 = (adjustedTarget - secondShotPos + gravityAdjustment).normalized;
        GameObject bullet2 = Instantiate(pistoleerBulletPrefab, secondShotPos, Quaternion.LookRotation(dir2));
        Rigidbody rbBullet2 = bullet2.GetComponent<Rigidbody>();
        rbBullet2.AddForce(dir2 * projectileLaunchForce, ForceMode.Impulse);
    }

    private void BackBarrelAttack()
    {
        backHumanAnimation?.PlayAttack();
        //backHumanAnimation?.SpawnOnBetweenHands(); // Animasyondan barrel çýkarma kontrolü
    }

    private int GetTargetLane(int lane)
    {
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

    IEnumerator ApproachToPlayArea()
    {
        while (transform.position.z > approachTargetZ)
        {
            float distanceToTarget = transform.position.z - approachTargetZ;
            float mult = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);

            float slowDownStartDistance = 40f;
            float t = Mathf.Clamp01(distanceToTarget / slowDownStartDistance);

            Vector3 vel = rb.velocity;
            vel.z = -approachSpeed * mult * t;
            rb.velocity = vel;
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector3.zero;
        transform.position = new Vector3(transform.position.x, transform.position.y, approachTargetZ);

        StartCoroutine(LaneChangeLoop());
    }

    #endregion
}
