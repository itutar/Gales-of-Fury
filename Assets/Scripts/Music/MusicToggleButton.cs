using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicToggleButton : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] private GameObject iconOn;   // Shown when music is playing
    [SerializeField] private GameObject iconOff;  // Shown when music is stopped

    // inital state of the music
    private bool startAsPlaying = true; // True = start with music playing

    // Internal toggle state: true means music is currently stopped
    private bool isStopped;

    private void Start()
    {
        // Initialize internal state
        isStopped = !startAsPlaying;

        // Try to sync with MusicManager at startup
        if (MusicManager.Instance == null)
        {
            Debug.LogWarning("[MusicToggleButton] MusicManager.Instance not found in scene.");
        }
        else
        {
            if (isStopped)
                MusicManager.Instance.StopMusic();   // Ensure music is stopped on start if requested
            else
                MusicManager.Instance.ResumeMusic(); // Ensure music is playing on start
        }

        ApplyIcons();
    }

    // Hook this to the Button's OnClick event in the Inspector
    public void OnToggleButtonPressed()
    {
        Toggle();
    }

    // Core toggle behavior: Stop -> Resume or Resume -> Stop
    private void Toggle()
    {
        if (MusicManager.Instance == null)
        {
            Debug.LogWarning("[MusicToggleButton] MusicManager.Instance not found; cannot toggle.");
            return;
        }

        if (isStopped)
        {
            // Currently stopped -> resume playing
            MusicManager.Instance.ResumeMusic();
            isStopped = false;
        }
        else
        {
            // Currently playing -> stop
            MusicManager.Instance.StopMusic();
            isStopped = true;
        }

        ApplyIcons();
    }

    // Reflect the state to UI icons
    private void ApplyIcons()
    {
        // When music is stopped: IconOff = active, IconOn = inactive
        if (iconOn != null) iconOn.SetActive(!isStopped);
        if (iconOff != null) iconOff.SetActive(isStopped);
    }
}
