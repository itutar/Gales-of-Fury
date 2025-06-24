using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    #region Singleton Pattern
    public static ScoreManager Instance { get; private set; }

    #endregion

    #region Fields

    [Header("Continuous Scoring")]
    [SerializeField] float continuousRate = 5f;        // Base puan/saniye
    [SerializeField] float speedMultiplier = 1f;       // H�zlan�nca �arpan
    [SerializeField] int speedStepScore = 10000; // Ka� puanda bir art��
    [SerializeField] float speedStepAmount = 0.1f;  // Her ad�mda +0.1f

    float _accumulatedTime;
    int _nextSpeedIncreaseThreshold;

    #endregion

    #region Unity Methods

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);


        // make sure the score starts at 0
        Blackboard.Instance.SetValue(BlackboardKey.Score, 0);

        Blackboard.Instance.SetValue(BlackboardKey.SpeedMultiplier, speedMultiplier);

        // first speed increase threshold
        _nextSpeedIncreaseThreshold = speedStepScore;
    }

    void Update()
    {
        // 1) Zaman� topla
        _accumulatedTime += Time.deltaTime;

        // 2) Her 1 saniyede bir puan ekle
        if (_accumulatedTime >= 1f)
        {
            float multiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
            int toAdd = Mathf.FloorToInt(continuousRate * multiplier);

            Add(toAdd);
            _accumulatedTime -= 1f;
        }
    }

    #endregion

    #region Public Methods

    public void Add(int amount)
    {
        // G�ncel skoru Blackboard'dan �ek
        int current = Blackboard.Instance.GetValue<int>(BlackboardKey.Score);
        // Yeni skoru hesapla
        int updated = current + amount;
        // Blackboard'a yaz
        Blackboard.Instance.SetValue(BlackboardKey.Score, updated);

        // E�er e�i�e ula�t�ysa �arpan� art�r ve Blackboard�a yaz
        if (updated >= _nextSpeedIncreaseThreshold)
        {
            speedMultiplier += speedStepAmount;
            Blackboard.Instance.SetValue(BlackboardKey.SpeedMultiplier, speedMultiplier);

            // Bir sonraki e�i�i haz�rlayal�m
            _nextSpeedIncreaseThreshold += speedStepScore;
        }
    }

    #endregion
}
