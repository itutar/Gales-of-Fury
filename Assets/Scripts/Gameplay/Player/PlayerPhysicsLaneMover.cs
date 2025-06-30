using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the player's movement between lanes using physics.
/// Contols the player's rotation
/// </summary>
public class PlayerPhysicsLaneMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private float springStrength = 500f;
    [SerializeField] private float positionThreshold = 0.1f;
    [SerializeField] private float rotationThreshold = 10f; // degrees

    [SerializeField] private float angleSpring = 30f;    // Yönü hedefe çekme kuvveti (artarsa daha hýzlý döner)
    [SerializeField] private float angleDamping = 10f;   // Fazla dönmeyi bastýran kuvvet (artarsa fazla sarkmaz, daha çabuk durur)

    private float? targetX = null;
    private bool isMoving = false;
    private PlayerController playerController;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        if (playerController != null)
            playerController.OnMoveToLaneRequested += SetTargetX;
    }

    private void OnDisable()
    {
        if (playerController != null)
            playerController.OnMoveToLaneRequested -= SetTargetX;
    }

    /// <summary>
    /// gets the target X position to move the player to a specific lane.
    /// </summary>
    /// <param name="laneX"></param>
    private void SetTargetX(float laneX)
    {
        targetX = laneX;
        isMoving = true;
    }

    private void FixedUpdate()
    {
        // 1. ROTASYON düzelt – tam 3D rotasyon (yaw + pitch + roll)
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up); // hedef düz rotasyon
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(transform.rotation);

        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f; // 0–180 yerine -180–180 aralýðýna çek

        if (Mathf.Abs(angle) > rotationThreshold)
        {
            float lerp = Mathf.Clamp01(Mathf.Abs(angle) / 90f);
            float spring = angleSpring * lerp;

            float springTorque = angle * spring;
            Vector3 torque = axis.normalized * springTorque;

            Vector3 damping = -rb.angularVelocity * angleDamping;

            rb.AddTorque(torque + damping, ForceMode.Force);
        }

        if (targetX.HasValue)
        {
            // 2. X eksenine yay kuvvetiyle hareket et
            Vector3 pos = transform.position;
            float diff = targetX.Value - pos.x;

            if (Mathf.Abs(diff) > positionThreshold)
            {
                Vector3 force = new Vector3(diff, 0f, 0f) * springStrength;
                rb.AddForce(force * Time.fixedDeltaTime, ForceMode.Force);
                isMoving = true;
            }
            else
            {
                // Hedefe ulaþýldý
                targetX = null;
                isMoving = false;
                rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z); // X hýzýný sýfýrla, opsiyonel
            }
        }
    }

    /// <summary>
    /// Halen hareket ediyor mu?
    /// </summary>
    public bool IsMoving()
    {
        return isMoving;
    }
}
