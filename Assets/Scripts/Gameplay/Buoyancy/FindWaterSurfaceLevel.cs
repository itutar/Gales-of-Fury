using Pinwheel.Poseidon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWaterSurfaceLevel : MonoBehaviour
{
    [SerializeField] private SeaTileReference seaTileReference;

    // EndlessTiles reference to get the list of the tiles
    private EndlessTiles endlessTiles;
    private bool applyRipple = true;

    private void Awake()
    {
        if (seaTileReference == null)
        {
            Debug.LogError("FindWaterSurfaceLevel:  SeaTileReference bulunamadý! Lütfen bir SeaTileReference ekleyin veya elle atayýn.", this);
            // if we can't find the reference, disable this script
            enabled = false;
            return;
        }

        // get the EndlessTiles component from the seaTileReference object
        endlessTiles = seaTileReference.seaTileReferenceObject.GetComponent<EndlessTiles>();
        if (endlessTiles == null)
        {
            Debug.LogError("FindWaterSurfaceLevel: EndlessTiles bileþeni bulunamadý! SeaTileReference.seaTileReferenceObject üzerine EndlessTiles ekli mi?", this);
            enabled = false;
        }
    }

    /// <summary>
    /// Returns the Y value of the water surface at the position where this GameObject is located.
    /// </summary>
    /// <returns>The Y (level) value of the water surface in world space.</returns>
    public float GetWaterSurfaceY()
    {
        PWater waterTile = GetClosestTile();
        if (waterTile == null)
        {
            // If the tile is not found, we return the Y of its own position
            Debug.LogWarning("FindWaterSurfaceLevel: En yakýn su tile'ý bulunamadý! Bu GameObject'in su yüzeyine yakýn bir yerde olduðundan emin olun.", this);
            return transform.position.y;
        }


        Vector3 localPos = waterTile.transform.InverseTransformPoint(transform.position);
        localPos.y = 0;
        localPos = waterTile.GetLocalVertexPosition(localPos, applyRipple);
        Vector3 worldPos = waterTile.transform.TransformPoint(localPos);
        return worldPos.y;
    }

    /// <summary>
    /// Finds the nearest PWater (water tile) object to this GameObject.
    /// </summary>
    private PWater GetClosestTile()
    {
        float closestDistance = float.MaxValue;
        PWater closestTile = null;

        // EndlessTiles.GetSpawnedTiles() gives us all the active water tiles in the scene.
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
}
