using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoostUIManager : MonoBehaviour
{
    public static BoostUIManager Instance { get; private set; }

    [SerializeField] private BoostUI iconPrefab;
    [SerializeField] private Sprite twoxSprite, dolphinSprite, doubleJumpSprite;

    private readonly Dictionary<BoostType, BoostUI> activeIcons = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);              // sahne deðiþince kaybolmasýn
    }

    public void ShowBoost(BoostType type, float duration)
    {
        // Ýkon zaten varsa sadece süresini yenile
        if (activeIcons.TryGetValue(type, out var icon) && icon != null)
        {
            icon.Refresh(duration);
            return;
        }

        // Yoksa prefab instantiate et
        BoostUI newIcon = Instantiate(iconPrefab, transform);
        newIcon.Init(GetSpriteForType(type), duration);
        activeIcons[type] = newIcon;
    }

    private Sprite GetSpriteForType(BoostType type) => type switch
    {
        BoostType.TwoX => twoxSprite,
        BoostType.Dolphin => dolphinSprite,
        BoostType.DoubleJump => doubleJumpSprite,
        _ => null
    };
}

public enum BoostType { TwoX, Dolphin, DoubleJump }
