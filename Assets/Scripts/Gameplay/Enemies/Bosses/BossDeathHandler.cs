using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathHandler : MonoBehaviour
{
    private int _lastKnownHealth;
    private bool _isDead = false;

    private void Start()
    {
        // Ba�lang��ta mevcut sa�l�k de�erini al
        _lastKnownHealth = Blackboard.Instance.GetValue<int>(BlackboardKey.BossHealth);

        // Sa�l�k de�i�imlerini dinle
        Blackboard.Instance.Subscribe<int>(BlackboardKey.BossHealth, OnHealthChanged);
    }

    private void OnHealthChanged(int newHealth)
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

        BossType bossType = Blackboard.Instance.GetValue<BossType>(BlackboardKey.CurrentBossType);
        BossEventManager.Instance.BossDefeated(bossType);

        Destroy(gameObject);
    }
}
