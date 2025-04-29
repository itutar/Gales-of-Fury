using Pinwheel.Poseidon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBuoyancy : MonoBehaviour
{
    [SerializeField] private Transform frontPoint;
    [SerializeField] private Transform backPoint;
    [SerializeField] private PWater water; // Poseidon Water reference
    [SerializeField] private float buoyancyStrength = 10f;
    [SerializeField] private float damping = 0.5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplyBuoyancyAtPoint(frontPoint);
        ApplyBuoyancyAtPoint(backPoint);
    }

    private void ApplyBuoyancyAtPoint(Transform point)
    {
        Vector3 worldPos = point.position;
        Vector3 localPos = water.transform.InverseTransformPoint(worldPos);

        Vector3 waterLocalPos = water.GetLocalVertexPosition(localPos);
        float waterHeight = water.transform.TransformPoint(waterLocalPos).y;

        float depthDifference = waterHeight - worldPos.y;

        if (depthDifference > 0)
        {
            // Nokta suyun altýnda -> Yukarý kuvvet uygula
            Vector3 force = Vector3.up * depthDifference * buoyancyStrength;
            // Hafif bir damping ekleyelim (isteðe baðlý)
            force -= rb.GetPointVelocity(worldPos) * damping;

            rb.AddForceAtPosition(force, worldPos);
        }
    }
}
