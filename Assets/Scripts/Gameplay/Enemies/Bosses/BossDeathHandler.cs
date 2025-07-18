using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathHandler : MonoBehaviour
{
    #region Fields

    private float _lastKnownHealth;
    private bool _isDead = false;

    // disappearing support
    private float moveToCornerForce = 12f;
    private float screenOffsetX = 6f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Başlangıçta mevcut sağlık değerini al
        _lastKnownHealth = Blackboard.Instance.GetValue<float>(BlackboardKey.BossHealth);

        // Sağlık değişimlerini dinle
        Blackboard.Instance.Subscribe<float>(BlackboardKey.BossHealth, OnHealthChanged);
    }

    #endregion

    #region Private Methods

    private void OnHealthChanged(float newHealth)
    {
        _lastKnownHealth = newHealth;

        if (!_isDead && newHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        _isDead = true;

        // event
        BossType bossType = Blackboard.Instance.GetValue<BossType>(BlackboardKey.CurrentBossType);
        BossEventManager.Instance.BossDefeated(bossType);

        // Destroy the boss game object
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            StartCoroutine(MoveToTopCornerAndDestroy(rb));
        }
        else
        {
            Destroy(gameObject); // Rigidbody yoksa direkt yok et
        }
    }

    /// <summary>
    /// disappears the boss by moving it to a corner of the screen and then destroying it.
    /// </summary>
    /// <param name="rb"></param>
    /// <returns></returns>
    private IEnumerator MoveToTopCornerAndDestroy(Rigidbody rb)
    {
        // 1. Rastgele yön seç
        Vector3 direction = (Random.value < 0.5f)
            ? new Vector3(-1f, 0f, 1f)
            : new Vector3(1f, 0f, 1f);
        direction.Normalize();

        Camera cam = Camera.main;
        bool outOfScreen = false;

        while (!outOfScreen)
        {
            float multiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
            rb.AddForce(direction * moveToCornerForce * multiplier, ForceMode.Force);

            float z = transform.position.z;
            float leftX = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, z)).x - screenOffsetX;
            float rightX = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, z)).x + screenOffsetX;

            float x = transform.position.x;
            if (x < leftX || x > rightX)
                outOfScreen = true;

            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion
}
