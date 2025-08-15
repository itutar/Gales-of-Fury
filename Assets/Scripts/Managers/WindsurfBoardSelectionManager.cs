using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Selector for 17 skins:
/// - index 0 -> Player (single-skin prefab)
/// - index 1..16 -> PlayerWithSwapableSkins (atlas tiles 0..15)
/// Saves selection & ownership via ES3. Updates preview roots accordingly.
/// </summary>
public class WindsurfBoardSelectionManager : MonoBehaviour
{
    [Header("BoardParent Setup")]
    [Tooltip("Parent object that contains EXACTLY two children:\n" +
             " - SingleBoardChild (index 0 skin)\n" +
             " - SwapableBoardChild (atlas)")]
    [SerializeField] private Transform boardParent;

    [Tooltip("Child that shows the single board (index 0). If null, will try to auto-detect as boardParent.GetChild(0).")]
    [SerializeField] private GameObject singleBoardChild;

    [Tooltip("Child that shows the swapable board (index 1..16). If null, will try to auto-detect as boardParent.GetChild(1).")]
    [SerializeField] private GameObject swapableBoardChild;

    [Tooltip("Swapper that controls atlas tile on the swapable board child.")]
    [SerializeField] private WindsurfBoardMeshSwapper swapableBoardSwapper;

    [Header("Economy / Catalog")]
    [SerializeField, Tooltip("Always 17: index 0 + 16 atlas skins.")]
    private int totalSkins = 17; // 0..16
    [SerializeField, Tooltip("Length must be 17. Price per skin. Index 0 will be forced to 0 and owned.")]
    private int[] skinPrices = new int[17];

    [Header("UI")]
    [SerializeField] private TMP_Text buttonLabel; // "BUY" / "SELECT" / "SELECTED"
    [SerializeField] private TMP_Text priceLabel;  // "123 Gold"
    [SerializeField] private Button confirmButton;

    // ES3 keys
    private const string KEY_SELECTED = "selectedBoardSkin";  // int 0..16
    private const string KEY_OWNED = "ownedBoardSkins";    // bool[17]
    private const string KEY_COINS = "coins";              // int

    // Runtime state
    private int currentIndex;   // browsing index (0..16)
    private int selectedIndex;  // equipped index (0..16)
    private bool[] owned;       // length 17
    private int coins;

    private void Awake()
    {
        // Auto-wire children if not assigned
        if (boardParent != null)
        {
            if (!singleBoardChild && boardParent.childCount >= 1)
                singleBoardChild = boardParent.GetChild(0).gameObject;
            if (!swapableBoardChild && boardParent.childCount >= 2)
                swapableBoardChild = boardParent.GetChild(1).gameObject;
        }

        if (!boardParent || !singleBoardChild || !swapableBoardChild)
        {
            Debug.LogError("WindsurfBoardSelectionManager: BoardParent or its two children are not assigned properly.");
            enabled = false; return;
        }

        if (!swapableBoardSwapper)
        {
            swapableBoardSwapper = swapableBoardChild.GetComponentInChildren<WindsurfBoardMeshSwapper>(true);
            if (!swapableBoardSwapper)
            {
                Debug.LogError("WindsurfBoardSelectionManager: Missing WindsurfBoardMeshSwapper on swapableBoardChild.");
                enabled = false; return;
            }
        }

        if (skinPrices == null || skinPrices.Length != totalSkins)
        {
            Debug.LogError("WindsurfBoardSelectionManager: skinPrices length must be 17.");
            enabled = false; return;
        }

        if (!buttonLabel || !priceLabel || !confirmButton)
        {
            Debug.LogError("WindsurfBoardSelectionManager: Missing UI references (buttonLabel/priceLabel/confirmButton).");
            enabled = false; return;
        }
    }

    private void Start()
    {
        LoadData();

        // Ensure defaults
        skinPrices[0] = 0; // default skin is free
        if (!owned[0]) { owned[0] = true; ES3.Save(KEY_OWNED, owned); }

        // Start from previously selected
        currentIndex = selectedIndex;

        UpdatePreview();
        RefreshUI();
    }

    /// <summary>Next skin with wrap-around.</summary>
    public void NextSkin()
    {
        currentIndex = (currentIndex + 1) % totalSkins;
        UpdatePreview();
        RefreshUI();
    }

    /// <summary>Previous skin with wrap-around.</summary>
    public void PreviousSkin()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = totalSkins - 1;
        UpdatePreview();
        RefreshUI();
    }

    /// <summary>
    /// BUY or SELECT depending on state. Persists coins, ownership, and selection.
    /// </summary>
    public void ConfirmOrBuy()
    {
        if (owned[currentIndex])
        {
            // Just select
            selectedIndex = currentIndex;
            ES3.Save(KEY_SELECTED, selectedIndex);

            // When swapable is visible, also reflect in its swapper (selection 1..16 -> tiles 0..15)
            if (currentIndex >= 1 && swapableBoardSwapper)
                swapableBoardSwapper.SetSelectionIndex(selectedIndex, persist: false);

            RefreshUI();
            return;
        }

        // Try to buy
        int price = skinPrices[currentIndex];
        if (coins < price)
        {
            RefreshUI();
            return;
        }

        // Spend
        coins -= price;
        ES3.Save(KEY_COINS, coins);
        OnCoinChangeEvent.Instance?.InvokeCoinChangeEvent();

        // Own + select
        owned[currentIndex] = true;
        ES3.Save(KEY_OWNED, owned);

        selectedIndex = currentIndex;
        ES3.Save(KEY_SELECTED, selectedIndex);

        if (currentIndex >= 1 && swapableBoardSwapper)
            swapableBoardSwapper.SetSelectionIndex(selectedIndex, persist: false);

        RefreshUI();
    }

    private void UpdatePreview()
    {
        // Index 0 -> show single board; hide swapable board.
        bool showSingle = (currentIndex == 0);

        singleBoardChild.SetActive(showSingle);
        swapableBoardChild.SetActive(!showSingle);

        // When swapable is shown, drive the atlas index immediately.
        if (!showSingle && swapableBoardSwapper)
        {
            // selection 1..16 -> mapped by the swapper to tiles 0..15 internally
            swapableBoardSwapper.SetSelectionIndex(currentIndex, persist: false);
        }
    }

    private void RefreshUI()
    {
        if (owned[currentIndex])
        {
            bool isSelected = (currentIndex == selectedIndex);
            buttonLabel.gameObject.SetActive(true);
            buttonLabel.text = isSelected ? "SELECTED" : "SELECT";
            priceLabel.gameObject.SetActive(false);
            confirmButton.interactable = !isSelected;
        }
        else
        {
            int price = skinPrices[currentIndex];
            priceLabel.gameObject.SetActive(true);
            priceLabel.text = price + " Gold";
            buttonLabel.gameObject.SetActive(false);
            confirmButton.interactable = coins >= price;
        }
    }

    private void LoadData()
    {
        coins = ES3.Load(KEY_COINS, 0);
        selectedIndex = ES3.Load(KEY_SELECTED, 0);
        owned = ES3.Load(KEY_OWNED, new bool[totalSkins]);

        selectedIndex = Mathf.Clamp(selectedIndex, 0, totalSkins - 1);

        // Repair owned array and force base ownership for index 0
        if (owned == null || owned.Length != totalSkins)
        {
            var fixedOwned = new bool[totalSkins];
            if (owned != null)
                System.Array.Copy(owned, fixedOwned, Mathf.Min(owned.Length, fixedOwned.Length));

            fixedOwned[0] = true; // default skin always owned
            owned = fixedOwned;
            ES3.Save(KEY_OWNED, owned);
        }
        else if (!owned[0])
        {
            owned[0] = true;
            ES3.Save(KEY_OWNED, owned);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (skinPrices != null && skinPrices.Length != totalSkins)
        {
            Debug.LogWarning("WindsurfBoardSelectionManager: skinPrices length should be 17.");
        }
    }
#endif
}
