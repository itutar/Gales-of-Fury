using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns the correct Player prefab at scene start based on saved board-skin selection:
/// - 0  -> Player (single-skin)
/// - 1..16 -> PlayerWithSwapableSkins (atlas)
/// </summary>
public class PlayerPrefabSelectorSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerSingleSkinPrefab;        // "Player"
    [SerializeField] private GameObject playerSwapableSkinsPrefab;     // "PlayerWithSwapableSkins"

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint; // optional; if null, uses this.transform

    private const string KEY_SELECTED = "selectedBoardSkin";

    private void Awake()
    {
        int selected = ES3.Load(KEY_SELECTED, 0);
        GameObject prefab = selected == 0 ? playerSingleSkinPrefab : playerSwapableSkinsPrefab;

        if (prefab == null)
        {
            Debug.LogError("PlayerPrefabSelectorSpawner: Missing prefab reference(s).");
            return;
        }

        Transform t = spawnPoint ? spawnPoint : transform;
        var instance = Instantiate(prefab, t.position, t.rotation);

        // Optional: if we want to force the atlas prefab to the right tile immediately:
        if (selected >= 1)
        {
            var swapper = instance.GetComponentInChildren<WindsurfBoardMeshSwapper>();
            if (swapper != null)
                swapper.SetSelectionIndex(selected, persist: false);
        }
    }
}
