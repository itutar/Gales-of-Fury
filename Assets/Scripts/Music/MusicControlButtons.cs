using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControlButtons : MonoBehaviour
{
    /// <summary>
    /// Call this method from the LEFT button (OnClick).
    /// It will play the previous track from the MusicManager playlist.
    /// </summary>
    public void OnPreviousButtonPressed()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayPrevious();
        }
        else
        {
            Debug.LogWarning("[MusicControlButtons] MusicManager.Instance not found.");
        }
    }

    /// <summary>
    /// Call this method from the RIGHT button (OnClick).
    /// It will play the next track from the MusicManager playlist.
    /// </summary>
    public void OnNextButtonPressed()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayNext();
        }
        else
        {
            Debug.LogWarning("[MusicControlButtons] MusicManager.Instance not found.");
        }
    }
}
