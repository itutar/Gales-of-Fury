using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    #region Singleton
    public static MusicManager Instance { get; private set; }

    #endregion

    #region Fields

    [Header("Playlist (15 elements fixed)")]
    [SerializeField] private AudioClip[] playlist = new AudioClip[15];

    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    // When false, the manager won't auto-jump to next track on end (used after Stop)
    private bool autoAdvance = true;

    #endregion

    #region Unity Methods

    void Awake()
    {
        // Singleton guard
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Safety: add one if missing
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure the AudioSource for BGM usage
        audioSource.playOnAwake = false;     // we'll start manually
        audioSource.loop = false;            // we handle track sequencing
        audioSource.spatialBlend = 0f;       // 2D sound for BGM

        // Start with first valid track if available
        currentTrackIndex = GetNextValidIndex(fromIndex: -1, step: +1);
        if (currentTrackIndex != -1)
        {
            PlayTrack(currentTrackIndex);
        }
    }

    void Update()
    {
        // Auto-advance only if enabled, a clip is assigned, and playback has actually ended
        if (autoAdvance && audioSource.clip != null && !audioSource.isPlaying)
        {
            NextTrackInternal();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Plays the next available track in the playlist (skips null slots).
    /// </summary>
    public void PlayNext()
    {
        autoAdvance = true; // resume normal behavior
        NextTrackInternal();
    }

    /// <summary>
    /// Plays the previous available track in the playlist (skips null slots).
    /// </summary>
    public void PlayPrevious()
    {
        autoAdvance = true; // resume normal behavior
        int prev = GetNextValidIndex(currentTrackIndex, -1);
        if (prev != -1)
        {
            currentTrackIndex = prev;
            PlayTrack(currentTrackIndex);
        }
    }

    /// <summary>
    /// Continues music playback: if paused, unpauses; if stopped or not started, (re)starts.
    /// </summary>
    public void ResumeMusic()
    {
        autoAdvance = true;

        // If there is a clip and we were paused (time > 0, not playing), unpause
        if (audioSource.clip != null && !audioSource.isPlaying && audioSource.time > 0f)
        {
            audioSource.UnPause(); // resume from paused position
            return;
        }

        // If there is a clip but either time is 0 or we were stopped, start playing
        if (audioSource.clip != null)
        {
            audioSource.Play();
            return;
        }

        // If no clip is assigned yet, pick the first valid track and start
        int first = GetNextValidIndex(fromIndex: -1, step: +1);
        if (first != -1)
        {
            currentTrackIndex = first;
            PlayTrack(currentTrackIndex);
        }
    }

    /// <summary>
    /// Stops music playback and disables auto-advance until resumed.
    /// </summary>
    public void StopMusic()
    {
        autoAdvance = false;  // do not auto jump to next after stopping
        audioSource.Stop();   // resets time to 0
    }

    #endregion

    #region Private Methods

    // -----------------------------
    // Internal Helpers
    // -----------------------------

    // Play a specific index safely (skips nulls)
    private void PlayTrack(int index)
    {
        if (index < 0 || index >= playlist.Length) return;

        // If target slot is null, try to find a valid alternative in the forward direction
        if (playlist[index] == null)
        {
            int next = GetNextValidIndex(index, +1);
            if (next == -1) return; // nothing to play
            currentTrackIndex = next;
        }
        else
        {
            currentTrackIndex = index;
        }

        audioSource.clip = playlist[currentTrackIndex];
        audioSource.time = 0f;      // start from beginning
        audioSource.Play();
    }

    // Advance to next valid clip
    private void NextTrackInternal()
    {
        int next = GetNextValidIndex(currentTrackIndex, +1);
        if (next != -1)
        {
            currentTrackIndex = next;
            PlayTrack(currentTrackIndex);
        }
    }

    // Finds the next valid (non-null) index in given step direction, wrapping around.
    // Returns -1 if no valid entries exist.
    private int GetNextValidIndex(int fromIndex, int step)
    {
        if (playlist == null || playlist.Length == 0) return -1;

        int length = playlist.Length;
        // Start probing from the next position relative to fromIndex
        int probe = fromIndex;

        for (int i = 0; i < length; i++)
        {
            probe = (probe + step + length) % length;
            if (playlist[probe] != null)
                return probe;
        }

        // No valid clips found
        return -1;
    }

    #endregion
}
