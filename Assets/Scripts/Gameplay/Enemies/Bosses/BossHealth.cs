using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    float currentHealth;

    #region Unity Methods

    private void OnEnable()
    {
        BossEventManager.Instance.OnBossTakeDamage.AddListener(TakeDamage);
    }

    private void OnDisable()
    {
        BossEventManager.Instance.OnBossTakeDamage.RemoveListener(TakeDamage);
    }

    private void Start()
    {
        BossType bossType = Blackboard.Instance.GetValue<BossType>(BlackboardKey.CurrentBossType);
        currentHealth = GetHealthForBoss(bossType);
        Blackboard.Instance.SetValue(BlackboardKey.BossHealth, currentHealth);
    }

    #endregion

    private void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Blackboard.Instance.SetValue(BlackboardKey.BossHealth, currentHealth);

        BossType bossType = Blackboard.Instance.GetValue<BossType>(BlackboardKey.CurrentBossType);
    }

    private int GetHealthForBoss(BossType bossType)
    {
        switch (bossType)
        {
            case BossType.CaptainMagnusBlackstorm: return 5;
            case BossType.ErikBlades: return 4;
            case BossType.IsabellaIronheart: return 4;
            case BossType.JackGrim: return 3;
            default: return 1;
        }
    }
}
