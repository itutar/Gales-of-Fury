using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackGrimBehaviour : MonoBehaviour
{
    #region Fields

    private float laneChangeCooldown = 2f;
    private float forceStrength = 20f;
    private float rotationSpeed = 1f;
    private int currentLane;

    Rigidbody rb;

    // Projectile support
    [SerializeField] PlayerReference player;
    [SerializeField] private Transform frontLaunchPoint;
    [SerializeField] private Transform backLaunchPoint;
    [SerializeField] private GameObject projectilePrefab;
    float launchForce = 40f;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        currentLane = 1;
        StartCoroutine(LaneChangeLoop());
        rb = GetComponent<Rigidbody>();
    }

    #endregion

    #region Private Methods

    private IEnumerator LaneChangeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(laneChangeCooldown);

            int targetLane = GetTargetLane(currentLane);
            if (targetLane == currentLane)
                continue;

            bool right = targetLane > currentLane;

            // 1. Rotate çapraza
            Quaternion diagonalRot = Quaternion.Euler(0, right ? 45f : -45f, 0);
            yield return StartCoroutine(RotateTo(diagonalRot));

            // 2. Attack
            DoAttack();

            // 3. Move to new lane
            yield return StartCoroutine(MoveToLane(targetLane));

            // 4. Düz hizaya dön
            yield return StartCoroutine(RotateTo(Quaternion.Euler(0, 0, 0)));

            currentLane = targetLane;
        }
    }

    private IEnumerator RotateTo(Quaternion targetRot)
    {
        while (Quaternion.Angle(transform.rotation, targetRot) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
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
        Vector3 v = rb.velocity;
        rb.velocity = new Vector3(0f, v.y, v.z);
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
    }

    private void DoAttack()
    {
        Debug.Log("Jack Grim DOATTACK METODU ÇALIÞTI!");
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
        switch(lane)
            {
            case 0: return 2; 
            case 1: return 3;
            case 2: return 0;
            case 3: return 1;
            default: return lane; // fallback
        }
    }

    #endregion
}
