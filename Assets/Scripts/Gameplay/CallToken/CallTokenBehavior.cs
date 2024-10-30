using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallTokenBehavior : MonoBehaviour
{
    #region Fields

    // Token specific help ship prefab
    [SerializeField] private GameObject helpShipPrefab;
    [SerializeField] float moveSpeed = 5f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HelpShipManager.instance?.GrantHelpShip(helpShipPrefab);
        }
    }

    #endregion
}
