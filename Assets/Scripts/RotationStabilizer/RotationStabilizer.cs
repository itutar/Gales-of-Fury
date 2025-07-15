using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic component that corrects the rotation of an object with a Rigidbody,
/// applying a spring-damping force above a certain threshold based on 
/// the desired forward and up directions.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RotationStabilizer : MonoBehaviour
{
    #region Fields

    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Target Rotation")]
    [Tooltip("Where is the target forward?")]
    [SerializeField] private Vector3 targetForward = Vector3.forward;
    [Tooltip("Which axis should be considered 'up'?")]
    [SerializeField] private Vector3 targetUp = Vector3.up;

    private float rotationThreshold = 10f;
    private float angleSpring = 30f;
    private float angleDamping = 10f;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Hedef rotasyonu olu�tur
        Quaternion targetRotation = Quaternion.LookRotation(targetForward, targetUp);
        // Mevcut rotasyona g�re fark� hesapla
        Quaternion delta = targetRotation * Quaternion.Inverse(transform.rotation);
        delta.ToAngleAxis(out float angle, out Vector3 axis);
        // 0�360 --> -180�180 aral���na �ek
        if (angle > 180f) angle -= 360f;

        // E�ikten b�y�kse d�zeltme uygulans�n
        if (Mathf.Abs(angle) > rotationThreshold)
        {
            // Sapman�n a��sal b�y�kl���ne g�re yay kuvveti �l�e�i
            float lerp = Mathf.Clamp01(Mathf.Abs(angle) / 90f);
            float spring = angleSpring * lerp;

            // Yay ve damping kuvvetlerini hesapla
            Vector3 torque = axis.normalized * (angle * spring);
            Vector3 damping = -rb.angularVelocity * angleDamping;

            // Rigidbody'ye uygula
            rb.AddTorque(torque + damping, ForceMode.Force);
        }
    }

    #endregion
}
