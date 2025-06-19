using Pinwheel.Poseidon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkBuoyancy : MonoBehaviour, IBuoyancy
{
    #region IBuoyancy Implementation

    public float SinkFactor
    {
        get { return sinkFactor; }
        set
        {
            sinkFactor = value;
        }
    }

    #endregion

    [Header("Buoyancy Settings")]
    [SerializeField] private Transform buoyancyPoint;
    [SerializeField] private SeaTileReference seaTileReference;
    [SerializeField] private float displacementAmount = 3f;
    [SerializeField] private float depthBeforeSubmerged = 1.5f;

    [Header("Alignment Settings")]
    [SerializeField] private float alignmentTorque = 10f;

    private Rigidbody rb;
    private EndlessTiles endlessTiles;
    private bool applyRipple = true;

    private float sinkFactor = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("SharkBuoyancy: Rigidbody bulunamad�!", this);

        if (seaTileReference == null)
            Debug.LogError("SharkBuoyancy: SeaTileReference atanmam��!", this);

        // EndlessTiles referans� al
        endlessTiles = seaTileReference?.seaTileReferenceObject.GetComponent<EndlessTiles>();
        if (endlessTiles == null)
            Debug.LogError("SharkBuoyancy: EndlessTiles bile�eni bulunamad�!", this);
    }

    private void FixedUpdate()
    {
        if (rb == null || buoyancyPoint == null || endlessTiles == null)
            return;

        // En yak�n su tile'�n� bul
        PWater waterTile = GetClosestTile();
        if (waterTile != null)
        {
            // 1) BUOYANCY
            Vector3 worldPos = buoyancyPoint.position;

            // Water surface y�ksekli�ini hesapla
            Vector3 localPos = waterTile.transform.InverseTransformPoint(worldPos);
            localPos.y = 0f;
            localPos = waterTile.GetLocalVertexPosition(localPos, applyRipple);
            Vector3 worldSurfacePos = waterTile.transform.TransformPoint(localPos);
            float waterLevel = worldSurfacePos.y;

            if (worldPos.y < waterLevel)
            {
                // batma derinli�i
                float depth = waterLevel - worldPos.y;
                float dispMult = Mathf.Clamp01(depth / depthBeforeSubmerged) * displacementAmount * sinkFactor;

                // yukar�ya kald�r�c� kuvvet
                Vector3 buoyancyForce = Vector3.up * Mathf.Abs(Physics.gravity.y) * dispMult;
                rb.AddForceAtPosition(buoyancyForce, worldPos, ForceMode.Acceleration);
            }

            // 2) ALIGNMENT TORQUE
            // transform.up ile d�nya up vekt�r� aras�ndaki a��sal fark� torkla kapat
            Vector3 up = transform.up;
            Vector3 worldUp = Vector3.up;

            // Dot ve cross hesapla
            float dot = Mathf.Clamp(Vector3.Dot(up, worldUp), -1f, 1f);
            Vector3 torqueAxis = Vector3.Cross(up, worldUp);

            // A�� fark�n� bulun
            float angleError = Mathf.Acos(dot);
            if (torqueAxis.sqrMagnitude > 1e-6f && angleError > 0.001f)
            {
                // Tork b�y�kl��� = a�� fark� * kuvvet katsay�s�
                Vector3 correctiveTorque = torqueAxis.normalized * angleError * alignmentTorque;
                rb.AddTorque(correctiveTorque, ForceMode.Acceleration);
            }
        }
    }

    /// <summary>
    /// En yak�n PWater (su tile) objesini bulur.
    /// </summary>
    private PWater GetClosestTile()
    {
        float closestDist = float.MaxValue;
        PWater closest = null;

        foreach (GameObject tile in endlessTiles.GetSpawnedTiles())
        {
            float d = Vector3.Distance(transform.position, tile.transform.position);
            if (d < closestDist)
            {
                closestDist = d;
                closest = tile.GetComponent<PWater>();
            }
        }

        return closest;
    }
}
