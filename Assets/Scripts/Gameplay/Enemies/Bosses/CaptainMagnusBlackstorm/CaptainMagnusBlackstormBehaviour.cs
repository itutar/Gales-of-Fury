using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainMagnusBlackstormBehaviour : MonoBehaviour
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

    [Header("VFX")]
    [SerializeField] private GameObject cannonSmokeVFX;

    [Header("Side Cannon Setup")]
    [SerializeField] private Transform LeftSideCannonsFirePoint1;
    [SerializeField] private Transform LeftSideCannonsFirePoint2;
    [SerializeField] private Transform RightSideCannonsFirePoint1;
    [SerializeField] private Transform RightSideCannonsFirePoint2;

    [Header("Back Attacks")]
    [SerializeField] private CaptainMagnusBlackstormBackBarrelHumanAnim backBarrelAnim;
    [SerializeField] private CaptainMagnusBlackstormBackCannonAnimation backCannonAnim1;
    [SerializeField] private CaptainMagnusBlackstormBackCannonAnimation backCannonAnim2;
    [SerializeField] private Transform BackCannonFirePoint1;
    [SerializeField] private Transform BackCannonFirePoint2;

    [SerializeField] private GameObject cannonballPrefab;

    float launchForce = 25f;

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

            // Yan top saldýrýsý (geminin yönüne göre)
            yield return StartCoroutine(DoSideAttack(right));

            // Düz hale dönüp arka saldýrý
            yield return StartCoroutine(RotateTo(Quaternion.Euler(0, 0, 0)));
            if (rotationStabilizer != null) rotationStabilizer.enabled = true;

            yield return new WaitForSeconds(0.5f);

            // straight back attacks
            BackBarrelAttack();
            yield return new WaitForSeconds(0.8f);
            BackCannonAttack();

            // Yeni lane'e geçiþ
            yield return StartCoroutine(MoveToLane(targetLane));
            currentLane = targetLane;
        }
    }

    private IEnumerator DoSideAttack(bool movingRight)
    {
        Quaternion diagonalRot = Quaternion.Euler(0, movingRight ? 45f : -45f, 0);
        if (rotationStabilizer != null) rotationStabilizer.enabled = false;
        yield return StartCoroutine(RotateTo(diagonalRot));

        FireSideCannons(movingRight);
    }

    private void FireSideCannons(bool right)
    {
        GameObject playerObject = player.player;
        if (playerObject == null) return;

        // Ýki fire point'ten de ateþleme yapýlmalý
        Transform firePoint1 = right ? RightSideCannonsFirePoint1 : LeftSideCannonsFirePoint1;
        Transform firePoint2 = right ? RightSideCannonsFirePoint2 : LeftSideCannonsFirePoint2;

        Vector3 adjustedTarget = playerObject.transform.position + Vector3.up;

        FireCannonFromPoint(firePoint1, adjustedTarget);
        FireCannonFromPoint(firePoint2, adjustedTarget);
    }

    private void FireCannonFromPoint(Transform firePoint, Vector3 target)
    {
        Vector3 dir = (target - firePoint.position).normalized;
        GameObject cannonball = Instantiate(cannonballPrefab, firePoint.position, Quaternion.LookRotation(dir));
        cannonball.GetComponent<Rigidbody>().AddForce(dir * launchForce, ForceMode.Impulse);

        // smoke VFX
        PlayCannonSmokeVFX(firePoint);
    }

    private void BackBarrelAttack()
    {
        backBarrelAnim?.PlayAttack();
    }

    private void BackCannonAttack()
    {
        backCannonAnim1?.PlayAttack();
        backCannonAnim2?.PlayAttack();

        GameObject playerObject = player.player;
        if (playerObject == null) return;

        Vector3 adjustedTarget = playerObject.transform.position + Vector3.up;

        FireCannonFromPoint(BackCannonFirePoint1, adjustedTarget);
        FireCannonFromPoint(BackCannonFirePoint2, adjustedTarget);
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
        //transform.position = new Vector3(targetX, transform.position.y, approachTargetZ);
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

    private void PlayCannonSmokeVFX(Transform firePoint)
    {
        if (cannonSmokeVFX != null)
        {
            GameObject vfx = Instantiate(cannonSmokeVFX, firePoint.position, firePoint.rotation);
            Destroy(vfx, 2f); // 2 saniye sonra yok et
        }
    }

    #endregion
}
