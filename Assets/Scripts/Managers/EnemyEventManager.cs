using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemyEventManager : MonoBehaviour
{
    #region UnityEvents Classes

    // UnityEvents
    [System.Serializable] public class EnemySpawnEvent : UnityEvent<EnemyType> { }
    [System.Serializable] public class EnemyDeathEvent : UnityEvent<GameObject> { }
    [System.Serializable] public class EnemyAttackEvent : UnityEvent<GameObject> { }
    [System.Serializable] public class EnemyDisappearEvent : UnityEvent<GameObject> { }

    #endregion

    #region Events

    // Events
    public EnemySpawnEvent OnEnemySpawned;
    public EnemyDeathEvent OnEnemyDied;
    public EnemyAttackEvent OnEnemyAttack;
    public EnemyDisappearEvent OnEnemyDisappear;

    #endregion

    #region Fields

    // singleton
    private static EnemyEventManager instance;

    #endregion

    #region Properties

    /// <summary>
    /// Singleton instance property. If no instance exists, it attempts to find one in the scene.
    /// </summary>
    public static EnemyEventManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Attempt to find an existing instance in the scene
                instance = FindObjectOfType<EnemyEventManager>();

                // If no instance is found, log an error
                if (instance == null)
                {
                    //Debug.Log("No instance of EnemyEventManager found in the scene!");
                }
            }
            return instance;
        }
    }

    #endregion

    #region Public Methods

    

    #endregion

    #region Private Methods

    

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    #endregion
}
