using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A basic Blackboard system for storing key-value pairs and notifying on changes.
/// </summary>
public class Blackboard : MonoBehaviour
{
    #region Singleton

    private static Blackboard _instance;
    
    /// <summary>
    /// Globally accessible instance of the Blackboard. If none exists in the scene,
    /// a new GameObject with this component will be created automatically.
    /// </summary>
    public static Blackboard Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Blackboard>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("Blackboard");
                    _instance = obj.AddComponent<Blackboard>();
                }
            }
            return _instance;
        }
    }


    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    private Dictionary<BlackboardKey, object> _data = new Dictionary<BlackboardKey, object>();

    /// <summary>
    /// Event fired when any value changes. Subscribers receive the key and new value.
    /// </summary>
    public event Action<BlackboardKey, object> OnValueChanged;

    /// <summary>
    /// Sets a value for a given key. If the key exists, it overwrites.
    /// Fires OnValueChanged only if the value actually changed.
    /// </summary>
    public void SetValue<T>(BlackboardKey key, T value)
    {
        if (_data.TryGetValue(key, out object oldValue) && Equals(oldValue, value))
            return;

        _data[key] = value;
        OnValueChanged?.Invoke(key, value);
    }

    /// <summary>
    /// Tries to get a value of type T for the given key.
    /// Returns default(T) if not found or wrong type.
    /// </summary>
    public T GetValue<T>(BlackboardKey key)
    {
        if (_data.TryGetValue(key, out object raw) && raw is T typed)
            return typed;
        return default;
    }

    /// <summary>
    /// Returns true if the key exists in the blackboard.
    /// </summary>
    public bool HasKey(BlackboardKey key)
    {
        return _data.ContainsKey(key);
    }

    /// <summary>
    /// Removes the entry with the given key, if it exists.
    /// </summary>
    public void RemoveKey(BlackboardKey key)
    {
        if (_data.ContainsKey(key))
            _data.Remove(key);
    }

    /// <summary>
    /// Subscribes to changes for a specific key of type T.
    /// Only triggers when the value type matches and key matches.
    /// </summary>
    public void Subscribe<T>(BlackboardKey key, Action<T> callback)
    {
        OnValueChanged += (changedKey, newValue) =>
        {
            if (changedKey == key && newValue is T casted)
            {
                callback(casted);
            }
        };
    }
}
