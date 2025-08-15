using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls atlas tiling/offset via MaterialPropertyBlock (4x4 by default).
/// Designed to work with total 17 skins: selection 1..16 map to atlas 0..15.
/// Selection 0 belongs to the single-skin Player prefab (not handled here).
/// </summary>
[DisallowMultipleComponent]
public class WindsurfBoardMeshSwapper : MonoBehaviour
{
    [Header("Atlas Grid")]
    [SerializeField] private int columns = 4;
    [SerializeField] private int rows = 4;

    [Header("Selection Mapping")]
    [Tooltip("Total selectable skins including index 0 reserved for Player prefab.")]
    [SerializeField] private int totalSkins = 17;   // 0..16 (17 skins total)
    [Tooltip("If true, selection 0 will preview atlas tile 0 for convenience in menu.")]
    [SerializeField] private bool mapZeroToFirstTileForPreview = false;

    [Header("Apply Options")]
    [SerializeField] private bool uvAlreadyFitsSingleCell = true;
    [SerializeField] private bool yFlipRows = false;
    [SerializeField] private bool applyEveryFrame = false;

    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private static readonly int ID_BaseMap_ST = Shader.PropertyToID("_BaseMap_ST");
    private static readonly int ID_MainTex_ST = Shader.PropertyToID("_MainTex_ST");

    // Persist keys shared with selection manager
    public const string KEY_SELECTED_BOARD_SKIN = "selectedBoardSkin";

    private int selectionIndex = 0; // 0..16 (0 reserved for Player prefab)

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (!rend) { enabled = false; return; }
        mpb = new MaterialPropertyBlock();
    }

    private void Start()
    {
        // Load last selection; default 0
        selectionIndex = ES3.Load(KEY_SELECTED_BOARD_SKIN, 0);
        ApplyFromSelection(selectionIndex);
    }

    private void OnEnable() => ApplyFromSelection(selectionIndex);

#if UNITY_EDITOR
    private void OnValidate()
    {
        columns = Mathf.Max(1, columns);
        rows = Mathf.Max(1, rows);
        totalSkins = Mathf.Max(1, totalSkins);
        if (isActiveAndEnabled) ApplyFromSelection(selectionIndex);
    }
#endif

    private void Update()
    {
        if (applyEveryFrame) ApplyFromSelection(selectionIndex);
    }

    /// <summary>
    /// Public API for external managers. selection = 0..(totalSkins-1).
    /// selection 1..16 map to atlas 0..15. selection 0 usually not mapped.
    /// </summary>
    public void SetSelectionIndex(int selection, bool persist = false)
    {
        selectionIndex = Mathf.Clamp(selection, 0, totalSkins - 1);
        ApplyFromSelection(selectionIndex);

        if (persist)
            ES3.Save(KEY_SELECTED_BOARD_SKIN, selectionIndex);
    }

    public int GetSelectionIndex() => selectionIndex;

    /// <summary>
    /// Calculates and writes ST vector for current selection.
    /// </summary>
    private void ApplyFromSelection(int selection)
    {
        if (!rend) return;

        // Map selection to atlas tile index:
        // selection 1..16 -> tile 0..15
        // selection 0 -> either do nothing (for Player prefab) or map to 0 for menu preview
        int atlasIndex;
        if (selection == 0)
        {
            if (!mapZeroToFirstTileForPreview)
            {
                // Keep current material state for index 0 (no change)
                return;
            }
            atlasIndex = 0;
        }
        else
        {
            atlasIndex = selection - 1;
        }

        int maxTile = columns * rows - 1;
        atlasIndex = Mathf.Clamp(atlasIndex, 0, maxTile);

        Vector2 cell = new Vector2(1f / columns, 1f / rows);
        int x = atlasIndex % columns;
        int y = atlasIndex / columns;
        if (yFlipRows) y = rows - 1 - y;

        Vector2 tiling = uvAlreadyFitsSingleCell ? Vector2.one : cell;
        Vector2 offset = new Vector2(x * cell.x, y * cell.y);
        Vector4 st = new Vector4(tiling.x, tiling.y, offset.x, offset.y);

        rend.GetPropertyBlock(mpb);
        mpb.SetVector(ID_BaseMap_ST, st);
        mpb.SetVector(ID_MainTex_ST, st);
        rend.SetPropertyBlock(mpb);
    }
}
