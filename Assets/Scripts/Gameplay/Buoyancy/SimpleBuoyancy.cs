using Pinwheel.Poseidon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBuoyancy : MonoBehaviour, IBuoyancy
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

    #region Fields

    PWater waterTile;
    bool applyRipple = true;
    Rigidbody rb;
    // EndlessTiles reference to get the list of the tiles
    [SerializeField] SeaTileReference seaTileReference;
    EndlessTiles endlessTiles;

    // Four buoyancy points frontleft, frontright, backleft, backright
    [SerializeField] GameObject[] buoyancyPoints = new GameObject[4];
    // water surface position frontleft, frontright, backleft, backright
    Vector3[] waterSurfacePositions = new Vector3[4];

    // Buoyancy parameters
    public float depthBeforeSubmerged = 2f;
    public float displacementAmount = 3f;

    // Water drag parameters
    [SerializeField] float waterDrag = 0.99f;
    [SerializeField] float waterAngularDrag = 0.5f;

    // if zero, the object will sink, if one no effect will be applied
    private float sinkFactor = 1f;
    #endregion

    #region Unity Methods

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        endlessTiles = seaTileReference.seaTileReferenceObject.GetComponent<EndlessTiles>();
        if (endlessTiles == null)
        {
            Debug.LogError("EndlessTiles script bulunamadý!");
            
        }
    }

    private void Update()
    {
        if (seaTileReference != null)
        {
            waterTile = GetClosestTile();
            if (waterTile != null)
            {
                // get the water position for each buoyancy point
                for (int i = 0; i < buoyancyPoints.Length; i++)
                {
                    Vector3 localPos = waterTile.transform.InverseTransformPoint(buoyancyPoints[i].transform.position);
                    localPos.y = 0;
                    localPos = waterTile.GetLocalVertexPosition(localPos, applyRipple);

                    Vector3 worldPos = waterTile.transform.TransformPoint(localPos);

                    // Add CPU-side curved world transformation to match water shader bending (CurvedWorldUtility)
                    worldPos = CurvedWorldUtility.BendPosition(worldPos);

                    waterSurfacePositions[i] = worldPos;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb == null || waterTile == null)
            return;
        for (int i = 0; i < buoyancyPoints.Length; i++)
        {
            Vector3 point = buoyancyPoints[i].transform.position;
            float waterLevel = waterSurfacePositions[i].y;

            rb.AddForceAtPosition(Physics.gravity / buoyancyPoints.Length,
                         buoyancyPoints[i].transform.position, ForceMode.Acceleration);

            // Sadece bu nokta suyun altýndaysa kaldýrýcý kuvvet uygula
            if (point.y < waterLevel)
            {
                float depth = waterLevel - point.y;
                // Nokta ne kadar batýk, 1 birimden fazla batarsa kuvvet maksimuma çýkar.
                float displacementMultiplier = Mathf.Clamp01(depth / depthBeforeSubmerged) * displacementAmount * sinkFactor;
                // Yukarý yönlü kuvvet (F = m * g * çarpan)
                Vector3 force = Vector3.up * Mathf.Abs(Physics.gravity.y) * displacementMultiplier;
                // Kuvveti noktaya uygula (dönme momenti oluþur, daha gerçekçi)
                rb.AddForceAtPosition(force, point, ForceMode.Acceleration);

                // Linear drag
                rb.AddForce(displacementMultiplier * -rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                // Angular drag
                rb.AddTorque(displacementMultiplier * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the closest tile to the attached object
    /// </summary>
    /// <returns>the closest tile</returns>
    private PWater GetClosestTile()
    {
        float closestDistance = float.MaxValue;
        PWater closestTile = null;

        foreach (GameObject tile in endlessTiles.GetSpawnedTiles())
        {
            float distance = Vector3.Distance(transform.position, tile.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile.GetComponent<PWater>();
            }
        }

        return closestTile;
    }

    #endregion
}
