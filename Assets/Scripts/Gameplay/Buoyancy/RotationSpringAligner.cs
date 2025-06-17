using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies torque to align the object's forward direction towards a target direction using a spring-damper system.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RotationSpringAligner : MonoBehaviour
{
    [Header("Rotation Alignment Settings")]
    [SerializeField] private Vector3 targetForward = Vector3.forward;
    [SerializeField] private float rotationThreshold = 2f; // degrees

    [SerializeField] private float angleSpring = 30f;    // Spring force to pull toward target direction
    [SerializeField] private float angleDamping = 10f;   // Damping force to stabilize rotation

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 currentForward = transform.forward;
        Vector3 rotAxis = Vector3.Cross(currentForward, targetForward).normalized;
        float angle = Vector3.SignedAngle(currentForward, targetForward, Vector3.up);

        if (rotAxis == Vector3.zero)
            rotAxis = Vector3.up;

        float currentAngularVel = rb.angularVelocity.y;

        if (Mathf.Abs(angle) >= rotationThreshold)
        {
            float lerp = Mathf.Clamp01(Mathf.Abs(angle) / 90f);
            float spring = angleSpring * lerp;

            float springTorque = angle * spring;
            float dampingTorque = -currentAngularVel * angleDamping;
            float totalTorque = springTorque + dampingTorque;

            rb.AddTorque(Vector3.up * totalTorque, ForceMode.Force);
        }
    }

    /// <summary>
    /// Allows changing the target direction at runtime.
    /// </summary>
    /// <param name="newTargetForward">New target forward direction in world space.</param>
    public void SetTargetForward(Vector3 newTargetForward)
    {
        targetForward = newTargetForward.normalized;
    }
}
