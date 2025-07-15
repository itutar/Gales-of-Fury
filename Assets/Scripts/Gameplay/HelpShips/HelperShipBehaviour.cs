using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperShipBehaviour : MonoBehaviour
{
    #region Fields

    float zMoveSpeed = 5f;
    private Transform bossTransform;
    private bool reachedZOffset = false;
    private const float zOffset = 30f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // get the boss transform from the blackboard if it exists
        bossTransform = Blackboard.Instance.GetValue<Transform>(BlackboardKey.CurrentBossTransform);
        // Subscribe to the boss transform updates Blackboard.Subscribe ile dinamik olarak boss spawn’ýný dinlemek,
        // helper’larýn boss’tan önce de yaratýlmasý durumunda bile referansýn gelince çalýþmaya baþlamasýný saðlar.
        // Blackboard.Instance.Subscribe<Transform>(BlackboardKey.CurrentBossTransform, t => bossTransform = t);
    }

    void Update()
    {
        if (bossTransform == null) return;

        if (!reachedZOffset)
        {
            // go to the Z offset position relative to the boss
            float targetZ = bossTransform.position.z + zOffset;
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, targetZ);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, zMoveSpeed * Time.deltaTime);

            if (Mathf.Abs(transform.position.z - targetZ) < 0.1f)
                reachedZOffset = true;
        }
        else
        {
            // follow the boss on the X axis
            Vector3 newPos = transform.position;
            newPos.x = bossTransform.position.x;
            transform.position = newPos;
        }
    }

    #endregion
}
