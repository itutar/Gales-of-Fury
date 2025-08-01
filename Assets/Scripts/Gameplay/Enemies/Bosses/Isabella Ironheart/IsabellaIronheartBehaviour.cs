using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsabellaIronheartBehaviour : MonoBehaviour
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

    [Header("Archer Setup")]
    [SerializeField] private Transform leftArcherProjectileSpawnPoint;
    [SerializeField] private Transform rightArcherProjectileSpawnPoint;
    [SerializeField] private GameObject isabellaArrowPrefab;

    [Header("Cannon Setup")]
    [SerializeField] private Transform cannonProjectileSpawnPoint;
    [SerializeField] private GameObject backCannonballPrefab;
    float launchForce = 25f;

    [Header("VFX")]
    [SerializeField] private GameObject cannonMuzzleVFX;

    [Header("Animation")]
    [SerializeField] IsabellaIronheartArcherAnimation archerAnimationR1;
    [SerializeField] IsabellaIronheartArcherAnimation archerAnimationR2;
    [SerializeField] IsabellaIronheartArcherAnimation archerAnimationL1;
    [SerializeField] IsabellaIronheartArcherAnimation archerAnimationL2;
    [SerializeField] IsabellaIronheartBackCannonHumanAnimation backCannonHumanAnimation;

    float approachTargetZ = 25f;
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

            if (right)
            {
                archerAnimationR1?.PlayAttack();
                archerAnimationR2?.PlayAttack();
            }
            else
            {
                archerAnimationL1?.PlayAttack();
                archerAnimationL2?.PlayAttack();
            }
                
            yield return new WaitForSeconds(0.5f);

            Quaternion diagonalRot = Quaternion.Euler(0, right ? 45f : -45f, 0);
            if (rotationStabilizer != null)
                rotationStabilizer.enabled = false;
            yield return StartCoroutine(RotateTo(diagonalRot));

            DoAttack(right);

            yield return StartCoroutine(MoveToLane(targetLane));

            yield return StartCoroutine(RotateTo(Quaternion.Euler(0, 0, 0)));
            if (rotationStabilizer != null)
                rotationStabilizer.enabled = true;

            currentLane = targetLane;

            // attack back cannon
            BackCannonAttack();
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
        //transform.position = new Vector3(targetX, transform.position.y, 18f);
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

    private void DoAttack(bool movingRight)
    {
        GameObject playerObject = player.player;
        if (playerObject == null) return;

        Vector3 adjustedTarget = playerObject.transform.position + Vector3.up;

        // Yön bilgisine göre doðru spawn noktasý seç
        Transform spawnPoint = movingRight ? rightArcherProjectileSpawnPoint : leftArcherProjectileSpawnPoint;

        // spawn arrows
        Vector3 dir1 = (adjustedTarget - spawnPoint.position).normalized;
        GameObject arrow1 = Instantiate(isabellaArrowPrefab, spawnPoint.position, Quaternion.LookRotation(dir1));
        Rigidbody rbArrow1 = arrow1.GetComponent<Rigidbody>();
        rbArrow1.AddForce(dir1 * launchForce, ForceMode.Impulse);

        Vector3 secondArrowPos = spawnPoint.position + new Vector3(1, 0, 1) * 1f;
        Vector3 dir2 = (adjustedTarget - secondArrowPos).normalized;
        GameObject arrow2 = Instantiate(isabellaArrowPrefab, secondArrowPos, Quaternion.LookRotation(dir2));
        Rigidbody rbArrow2 = arrow2.GetComponent<Rigidbody>();
        rbArrow2.AddForce(dir2 * launchForce, ForceMode.Impulse);
    }

    private void BackCannonAttack()
    {
        // play cannon muzzle VFX
        if (cannonMuzzleVFX != null)
        {
            // VFX'i cannon spawn noktasýnda instantiate et
            GameObject vfx = Instantiate(cannonMuzzleVFX, cannonProjectileSpawnPoint.position, cannonProjectileSpawnPoint.rotation);
            Destroy(vfx, 2f); // 2 saniye sonra otomatik olarak yok et
        }

        // Topçu animasyonunu tetikle
        backCannonHumanAnimation?.PlayAttack();

        // Hedef (player) pozisyonu al
        GameObject playerObject = player.player;
        if (playerObject == null) return;

        Vector3 adjustedTarget = playerObject.transform.position + Vector3.up * 2f;

        // Ýlk cannon mermisini spawnla
        Vector3 dir1 = (adjustedTarget - cannonProjectileSpawnPoint.position).normalized;
        GameObject cannonball1 = Instantiate(backCannonballPrefab, cannonProjectileSpawnPoint.position, Quaternion.LookRotation(dir1));
        Rigidbody rbCannon1 = cannonball1.GetComponent<Rigidbody>();
        rbCannon1.AddForce(dir1 * launchForce, ForceMode.Impulse);

        // Ýkinci cannon mermisi (küçük offset ile X ekseninde veya hafif zaman gecikmesi ile)

        Vector3 secondCannonPos = cannonProjectileSpawnPoint.position + Vector3.right * 0.5f; // X ekseninde 0.5 birim offset
        Vector3 dir2 = (adjustedTarget - secondCannonPos).normalized;
        GameObject cannonball2 = Instantiate(backCannonballPrefab, secondCannonPos, Quaternion.LookRotation(dir2));
        Rigidbody rbCannon2 = cannonball2.GetComponent<Rigidbody>();
        
        rbCannon2.AddForce(dir2 * launchForce, ForceMode.Impulse);
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
