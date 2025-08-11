using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Swaps the character's body and hair meshes by index. Materials are preserved
/// (sharedMaterials are not modified). Index 0 is the base look; subsequent
/// indices map to alternative styles. Loads the saved selected index on startup
/// and applies it automatically.
/// </summary>
public class CharacterMeshSwapper : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private SkinnedMeshRenderer bodyRenderer;
    [SerializeField] private SkinnedMeshRenderer hairRenderer;

    [Header("Meshes (0 = base, keep orders aligned)")]
    [SerializeField] private Mesh[] bodyMeshes;
    [SerializeField] private Mesh[] hairMeshes;

    private const string KEY_SELECTED = "selectedCharacter";

    private void Awake()
    {
        if (!bodyRenderer || !hairRenderer)
        {
            Debug.LogError("CharacterMeshSwapper: Missing target renderers.");
            enabled = false; return;
        }
    }

    private void Start()
    {
        int index = ES3.Load(KEY_SELECTED, 0);
        SwapMesh(index);
    }

    /// <summary>
    /// Applies body/hair meshes for the given index. Materials are left unchanged.
    /// </summary>
    public void SwapMesh(int index)
    {
        if (bodyMeshes == null || hairMeshes == null)
        {
            Debug.LogError("CharacterMeshSwapper: Mesh arrays are not assigned.");
            return;
        }
        if (bodyMeshes.Length != hairMeshes.Length)
        {
            Debug.LogError("CharacterMeshSwapper: bodyMeshes and hairMeshes must have the same length.");
            return;
        }
        if (bodyMeshes.Length == 0)
        {
            Debug.LogWarning("CharacterMeshSwapper: Mesh arrays are empty.");
            return;
        }
        if (index < 0 || index >= bodyMeshes.Length)
        {
            Debug.LogWarning($"CharacterMeshSwapper: index {index} out of range (0..{bodyMeshes.Length - 1}).");
            return;
        }

        // Use sharedMesh to preserve material asset sharing.
        bodyRenderer.sharedMesh = bodyMeshes[index];
        hairRenderer.sharedMesh = hairMeshes[index];
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (bodyMeshes != null && hairMeshes != null &&
            bodyMeshes.Length != hairMeshes.Length)
        {
            Debug.LogWarning("CharacterMeshSwapper: bodyMeshes and hairMeshes length mismatch.");
        }
    }
#endif
}
