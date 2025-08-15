using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PauseUIRefs", menuName = "References/Pause UI Refs")]
public class PauseUIRefs : ScriptableObject
{
    [NonSerialized] public GameObject PauseUI;           // // Scene object; set at runtime
    [NonSerialized] public GameObject PauseUIBackground; // // Scene object; set at runtime

    // // Notify listeners when refs are (re)assigned at runtime
    public event Action OnAssigned;

    // // Called by the Canvas handle once it resolves the children
    public void Assign(GameObject pauseUI, GameObject pauseBG)
    {
        PauseUI = pauseUI;
        PauseUIBackground = pauseBG;
        OnAssigned?.Invoke();
    }

    // // Helper: indicate whether both refs are ready
    public bool IsReady() => PauseUI != null && PauseUIBackground != null;
}