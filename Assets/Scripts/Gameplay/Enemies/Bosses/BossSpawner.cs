using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    #region Fields

    [Header("Boss Prefabs")]
    [SerializeField] GameObject captainMagnusBlackstormPrefab;
    [SerializeField] GameObject erikBladesPrefab;
    [SerializeField] GameObject isabellaIronheartPrefab;
    [SerializeField] GameObject jackGrimPrefab;

    private int scorePerBoss = 50000;

    private int _nextBossThreshold = 50000;
    private BossType _currentBossType;

    #endregion

    #region Unity Methods

    private void Start()
    {
        Blackboard.Instance.Subscribe<int>(BlackboardKey.Score, OnScoreChanged);
        Blackboard.Instance.SetValue(BlackboardKey.IsBossActive, false);
    }

    #endregion

    #region Private Methods

    private void OnScoreChanged(int newScore)
    {
        bool isBossActive = Blackboard.Instance.GetValue<bool>(BlackboardKey.IsBossActive);

        if (!isBossActive && newScore >= _nextBossThreshold)
        {
            SpawnBoss();
            _nextBossThreshold += scorePerBoss;
        }
    }

    private void SpawnBoss()
    {
        // Reset boss active state in the blackboard
        Blackboard.Instance.SetValue(BlackboardKey.IsBossActive, true);

        // Rastgele boss seç
        BossType[] bosses = (BossType[])System.Enum.GetValues(typeof(BossType));
        //_currentBossType = bosses[Random.Range(0, bosses.Length)];
        _currentBossType = BossType.JackGrim; // For testing

        // select boss prefab
        GameObject prefabToSpawn = GetBossPrefab(_currentBossType);

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"No prefab assigned for boss: {_currentBossType}");
            return;
        }

        // Get the current lane from the LaneManager
        int currentLane = LaneManager.instance.CurrentLane;
        float x = LaneManager.instance.GetLanePosition(currentLane);

        // Set the spawn position based on the current lane
        Vector3 spawnPosition = new Vector3(x, -21f, 120f);

        // Instantiate the boss prefab
        GameObject bossInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // Set the current boss type in the blackboard
        Blackboard.Instance.SetValue(BlackboardKey.CurrentBossType, _currentBossType);
        // Set the boss instance transform in the blackboard
        Blackboard.Instance.SetValue(BlackboardKey.CurrentBossTransform, bossInstance.transform);

        // Notify the boss event manager
        BossEventManager.Instance.BossSpawned(_currentBossType);
    }

    private GameObject GetBossPrefab(BossType bossType)
    {
        return bossType switch
        {
            BossType.CaptainMagnusBlackstorm => captainMagnusBlackstormPrefab,
            BossType.ErikBlades => erikBladesPrefab,
            BossType.IsabellaIronheart => isabellaIronheartPrefab,
            BossType.JackGrim => jackGrimPrefab,
            _ => null,
        };
    }

    #endregion
}
