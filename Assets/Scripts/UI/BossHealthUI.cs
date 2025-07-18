using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("Prefab / Parent")]
    [SerializeField] private Image heartPrefab;       // sadece *bir* tane, içinde Filled ayarı var
    [SerializeField] private Transform heartsParent;  // boş bırakılırsa = this.transform

    private readonly List<Image> hearts = new();
    private float maxHearts;

    private void Awake()
    {
        if (heartsParent == null) heartsParent = transform;
        //gameObject.SetActive(false);  // ilk sahne yüklenirken görünmesin
    }

    private void OnEnable()
    {
        if (BossEventManager.Instance != null)
        {
            Debug.Log("[BossHealthUI] BossEventManager.Instance mevcut, listener ekleniyor");
            BossEventManager.Instance.OnBossSpawned.AddListener(OnBossSpawned);
        }
        else
        {
            Debug.LogWarning("[BossHealthUI] BossEventManager.Instance NULL!");
        }

        if (Blackboard.Instance != null)
            Blackboard.Instance.Subscribe<float>(BlackboardKey.BossHealth, UpdateHearts);
    }

    private void OnDisable()
    {
        BossEventManager.Instance.OnBossSpawned.RemoveListener(OnBossSpawned);
    }

    // 1) Boss spawn olduğunda gerekli sayıda kalbi inşa et
    private void OnBossSpawned(BossType bossType)
    {
        BuildHearts(bossType);
        float current = Blackboard.Instance.GetValue<float>(BlackboardKey.BossHealth);
        UpdateHearts(current);
        gameObject.SetActive(true);
    }

    // 2) Sağlığı değiştikçe her kalbin “fillAmount” değerini güncelle
    private void UpdateHearts(float currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            float amount = Mathf.Clamp01(currentHealth - i);  // 0–1 arası
            hearts[i].fillAmount = amount;
        }

        if (currentHealth <= 0f)   // boss öldü → UI gizle
            gameObject.SetActive(false);
    }

    // 3) Eski kalpleri sil, yenilerini oluştur
    private void BuildHearts(BossType bossType)
    {
        Debug.Log($"[BossHealthUI] BuildHearts çağrıldı. Kalp sayısı: {GetHealthForBoss(bossType)}");

        foreach (var img in hearts) Destroy(img.gameObject);
        hearts.Clear();

        maxHearts = GetHealthForBoss(bossType);

        for (int i = 0; i < maxHearts; i++)
        {
            Image img = Instantiate(heartPrefab, heartsParent);
            img.type = Image.Type.Filled;         // emin ol
            img.fillMethod = Image.FillMethod.Horizontal;  
            img.fillAmount = 1f;                  // tam dolu
            hearts.Add(img);
        }
    }

    // same switch as in BossHealth.cs
    private static float GetHealthForBoss(BossType bossType) =>
        bossType switch
        {
            BossType.CaptainMagnusBlackstorm => 5f,
            BossType.ErikBlades => 4f,
            BossType.IsabellaIronheart => 4f,
            BossType.JackGrim => 3f,
            _ => 1f
        };
}
