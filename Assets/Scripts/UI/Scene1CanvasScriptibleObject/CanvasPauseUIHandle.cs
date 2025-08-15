using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-100)] //Run before typical gameplay scripts
public class CanvasPauseUIHandle : MonoBehaviour
{
    [Header("Scriptable Object to write into (drag asset from Project)")]
    [SerializeField] private PauseUIRefs pauseUIRefs;

    [Header("Children under this Canvas (drag from the same Canvas)")]
    [SerializeField] private GameObject pauseUI;           // // e.g. the Pause panel root
    [SerializeField] private GameObject pauseUIBackground; // // e.g. dim/blur overlay

    private void Awake()
    {
        if (pauseUIRefs == null)
        {
            Debug.LogError($"{nameof(CanvasPauseUIHandle)}: PauseUIRefs is not assigned.");
            return;
        }
        if (pauseUI == null || pauseUIBackground == null)
        {
            Debug.LogError($"{nameof(CanvasPauseUIHandle)}: PauseUI or Background is not assigned.");
            return;
        }

        // // Push scene refs into the ScriptableObject
        pauseUIRefs.Assign(pauseUI, pauseUIBackground);
    }
}
