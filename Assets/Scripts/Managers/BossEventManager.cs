using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BossSpawnedEvent : UnityEvent<BossType> { }

[System.Serializable]
public class BossDamageEvent : UnityEvent<int> { }

[System.Serializable]
public class BossDefeatedEvent : UnityEvent<BossType> { }

public class BossEventManager : MonoBehaviour
{
    public static BossEventManager Instance { get; private set; }

    public BossSpawnedEvent OnBossSpawned = new BossSpawnedEvent();
    public BossDamageEvent OnBossTakeDamage = new BossDamageEvent();
    public BossDefeatedEvent OnBossDefeated = new BossDefeatedEvent();

    #region Unity Methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Boss Spawn Event Methods

    /// <summary>
    /// Call this when a new boss is spawned.
    /// </summary>
    public void BossSpawned(BossType bossType)
    {
        Debug.Log($"Boss spawned: {bossType}");
        Blackboard.Instance.SetValue(BlackboardKey.IsBossActive, true);
        OnBossSpawned?.Invoke(bossType);
    }

    #endregion

    #region Boss Damage Event Methods

    public void BossTookDamage(int damage)
    {
        Debug.Log($"took {damage} damage. BossEventManager BossTookDamageMethod Çalýþtý");
        OnBossTakeDamage?.Invoke(damage);
    }

    #endregion

    #region Boss Death Event Methods

    /// <summary>
    /// Call this when boss is defeated
    /// </summary>
    public void BossDefeated(BossType bossType)
    {
        Blackboard.Instance.SetValue(BlackboardKey.IsBossActive, false);
        OnBossDefeated?.Invoke(bossType);
    }

    #endregion
}
