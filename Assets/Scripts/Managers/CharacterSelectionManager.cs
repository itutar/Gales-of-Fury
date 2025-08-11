using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles character browsing, buying, selection, and persistence.
/// Shows a preview instance at the given spawn point, manages coin-based purchases,
/// and saves/loads owned flags, selected index, and coin balance via ES3.
/// Ensures data stays consistent with prefab count (first character always owned),
/// and clamps the selected index to a valid range. 
/// </summary>
public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Character Prefabs (order matters)")]
    [SerializeField] private GameObject[] characterPrefabs;

    [Header("Character Prices (same order as prefabs)")]
    [SerializeField] private int[] characterPrices;

    [Header("UI References")]
    [SerializeField] private Transform spawnPoint;   // preview spawn parent
    [SerializeField] private TMP_Text buttonLabel;   // "BUY" / "SELECT" / "SELECTED"
    [SerializeField] private TMP_Text priceLabel;    // "123 Gold"
    [SerializeField] private Button confirmButton;

    // ES3 keys
    private const string KEY_SELECTED = "selectedCharacter";
    private const string KEY_OWNED = "ownedCharacters";
    private const string KEY_COINS = "coins";

    // Runtime state
    private int currentIndex;           // index being previewed in the shop UI
    private GameObject currentInstance; // preview GO
    private int selectedIndex;          // the one currently equipped
    private bool[] owned;               // purchase flags aligned with prefabs
    private int coins;                  // cached coin balance

    private void Awake()
    {
        // Minimal sanity checks to fail fast in Editor
        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogError("CharacterSelectionManager: No characterPrefabs assigned.");
            enabled = false; return;
        }
        if (spawnPoint == null || buttonLabel == null || priceLabel == null || confirmButton == null)
        {
            Debug.LogError("CharacterSelectionManager: Missing UI references.");
            enabled = false; return;
        }
    }

    private void Start()
    {
        LoadData();
        SpawnCharacter(currentIndex);
        RefreshUI();
    }

    /// <summary>Go to next character (wraps around).</summary>
    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characterPrefabs.Length;
        SpawnCharacter(currentIndex);
        RefreshUI();
    }

    /// <summary>Go to previous character (wraps around).</summary>
    public void PreviousCharacter()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = characterPrefabs.Length - 1;
        SpawnCharacter(currentIndex);
        RefreshUI();
    }

    /// <summary>
    /// If the current character is already owned, selects it and persists the choice.
    /// Otherwise tries to buy it using coins; on success, marks as owned, selects, and persists.
    /// Also notifies the mesh swapper to apply the selected look immediately.
    /// </summary>
    public void ConfirmOrBuy()
    {
        // Guard: index and arrays must be valid
        if (currentIndex < 0 || currentIndex >= characterPrefabs.Length)
        {
            Debug.LogWarning($"ConfirmOrBuy: currentIndex {currentIndex} is out of range.");
            return;
        }
        if (characterPrices == null || characterPrices.Length != characterPrefabs.Length)
        {
            Debug.LogError("ConfirmOrBuy: characterPrices length must match characterPrefabs length.");
            return;
        }
        if (owned == null || owned.Length != characterPrefabs.Length)
        {
            Debug.LogError("ConfirmOrBuy: owned array is null or has mismatched length.");
            return;
        }

        // Already owned -> just select and persist
        if (owned[currentIndex])
        {
            selectedIndex = currentIndex;
            ES3.Save(KEY_SELECTED, selectedIndex);

            RefreshUI();
            return;
        }

        // Try to buy
        int price = characterPrices[currentIndex];
        if (coins < price)
        {
            // Not enough coins; UI will already reflect this state
            RefreshUI();
            return;
        }

        // Spend coins and persist
        coins -= price;
        ES3.Save(KEY_COINS, coins);
        // Notify listeners via scene-based event
        OnCoinChangeEvent.Instance?.InvokeCoinChangeEvent();

        // Mark as owned and persist
        owned[currentIndex] = true;
        ES3.Save(KEY_OWNED, owned);

        // Auto-select newly purchased character and persist
        selectedIndex = currentIndex;
        ES3.Save(KEY_SELECTED, selectedIndex);

        // Update UI
        RefreshUI();
    }

    /// <summary>Destroys current preview and spawns the character prefab at the spawn point.</summary>
    private void SpawnCharacter(int index)
    {
        if (currentInstance != null) Destroy(currentInstance);

        currentInstance = Instantiate(
            characterPrefabs[index],
            spawnPoint);

        // Make preview deterministic in local space
        currentInstance.transform.localPosition = Vector3.zero;
        currentInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        currentInstance.transform.localScale = Vector3.one;
    }

    /// <summary>Refreshes button state and price label based on ownership/selection.</summary>
    private void RefreshUI()
    {
        if (owned[currentIndex])
        {
            // Make sure the label is visible when character is owned
            buttonLabel.gameObject.SetActive(true);
            bool isSelected = currentIndex == selectedIndex;
            buttonLabel.text = isSelected ? "SELECTED" : "SELECT"; // show proper state
            priceLabel.gameObject.SetActive(false);
            confirmButton.interactable = !isSelected; // cannot re-select an already selected one
        }
        else
        {
            int price = characterPrices[currentIndex];
            priceLabel.gameObject.SetActive(true);
            priceLabel.text = price + " Gold";

            // Hide the label so "BUY" text doesn't appear
            buttonLabel.gameObject.SetActive(false);
            confirmButton.interactable = coins >= price; // allow buying if enough coins
        }
    }

    /// <summary>
    /// Loads coins, owned flags, and selected index from ES3 and sanitizes them
    /// against the current prefab list. Ensures the first character is owned and
    /// clamps the selected index. Writes back to ES3 if a repair was needed.
    /// </summary>
    private void LoadData()
    {
        coins = ES3.Load(KEY_COINS, 0);
        owned = ES3.Load(KEY_OWNED, new bool[characterPrefabs.Length]);
        selectedIndex = ES3.Load(KEY_SELECTED, 0);

        bool fixedOwned = false;

        // Align owned[] length with prefab count
        if (owned == null || owned.Length != characterPrefabs.Length)
        {
            var newOwned = new bool[characterPrefabs.Length];

            if (owned != null)
                System.Array.Copy(owned, newOwned, Mathf.Min(owned.Length, newOwned.Length));

            if (newOwned.Length > 0)
                newOwned[0] = true; // guarantee first is owned

            owned = newOwned;
            fixedOwned = true;
        }
        else
        {
            // Even if lengths match, guarantee first is owned
            if (owned.Length > 0 && !owned[0])
            {
                owned[0] = true;
                fixedOwned = true;
            }
        }

        // Clamp indices safely
        currentIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, characterPrefabs.Length - 1));
        selectedIndex = currentIndex;

        if (fixedOwned)
            ES3.Save(KEY_OWNED, owned);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (characterPrefabs != null && characterPrices != null &&
            characterPrefabs.Length != characterPrices.Length)
        {
            Debug.LogWarning("CharacterSelectionManager: Prices count should match prefabs count.");
        }
    }
#endif
}

