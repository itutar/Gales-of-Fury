using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperShipBehaviourTest : MonoBehaviour
{
    #region Fields

    private float sideOffset = 25f;   // X offset relative to the boss
    private float moveSpeed = 10f;  // Units/second
    private float rotationSpeed = 15f;  // Slerp factor

    private float positionTolerance = 0.1f; // When we consider “arrived”

    private Transform bossTransform;

    #endregion


    #region Unity Methods

    private void Start()
    {
        // Try to grab the boss reference from the Blackboard
        bossTransform = Blackboard.Instance.GetValue<Transform>(BlackboardKey.CurrentBossTransform);

        // (Optional) subscribe in case the boss spawns later
        //Blackboard.Instance.Subscribe<Transform>(
        //    BlackboardKey.CurrentBossTransform, t => bossTransform = t);

        // Make sure the flag starts as FALSE every time a helper ship is spawned
        Blackboard.Instance.SetValue(BlackboardKey.HelperShipReachedZOffset, false);
    }

    private void Update()
    {
        if (bossTransform == null) return;

        // ---------- 1. Move to target position ----------
        Vector3 targetPos = bossTransform.position + bossTransform.right * sideOffset;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime);

        // ---------- 2. Always face the boss ----------
        Vector3 lookDir = bossTransform.position - transform.position;
        lookDir.y = 0f;                        // Keep the ship upright
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime);
        }

        // ---------- 3. Update the Blackboard flag ----------
        bool reached = Vector3.Distance(transform.position, targetPos) < positionTolerance;
        Blackboard.Instance.SetValue(BlackboardKey.HelperShipReachedZOffset, reached);
    }

    #endregion
}
