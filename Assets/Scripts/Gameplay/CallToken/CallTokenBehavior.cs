using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallTokenBehavior : MonoBehaviour
{
    #region Fields

    // Token specific help ship prefab
    [SerializeField] private GameObject helpShipPrefab;
    float moveSpeed = 5f;

    // Water surface finder
    [SerializeField] private FindWaterSurfaceLevel waterSurfaceLevelFinder;

    #endregion

    #region Unity Methods

    private void Start()
    {
        if (waterSurfaceLevelFinder == null)
        {
            waterSurfaceLevelFinder = GetComponent<FindWaterSurfaceLevel>();
        }

        // destroy the token after 5 seconds to prevent clutter
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        // movement on z axis
        transform.Translate(Vector3.back * moveSpeed * Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier) *Time.deltaTime);
        // adjust position to water surface level
        if (waterSurfaceLevelFinder != null)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = waterSurfaceLevelFinder.GetWaterSurfaceY();
            transform.position = newPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HelpShipManager.instance?.GrantHelpShip(helpShipPrefab);
            HelpShipManager.instance?.CallHelpShip();
        }
    }

    #endregion
}
