using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLifecycleStateMachine : MonoBehaviour
{
    #region Fields

    // Current state of the enemy
    public EnemyState currentState;

    // Reference to attack component and movement component
    [SerializeField] private MonoBehaviour attackComponentRaw;
    private IAttackController attackComponent;
    [SerializeField] private MonoBehaviour moveComponentRaw;
    private IMoveToLane moveComponent;

    // Enemy health
    public float health = 1f;

    // Escape speed
    public Vector3 escapeDirection = new Vector3(1f, 1f, 0f); // Move top-right by default
    public float escapeSpeed = 3f;

    #endregion

    #region Unity Methods   

    private void Awake()
    {
        // cast move component to IMoveToLane interface
        moveComponent = moveComponentRaw as IMoveToLane;
        if (moveComponent == null)
            Debug.LogError("moveComponentRaw IMoveToLane implement etmiyor!", this);


        // cast attack component to IAttackController interface
        attackComponent = attackComponentRaw as IAttackController;
        if (attackComponent == null)
        {
            Debug.LogError("Attack component does not implement IAttackController interface.");
        }
    }

    // Initialization
    private void Start()
    {
        currentState = EnemyState.Idle;
        attackComponent?.DisableAttack();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                // check lane arrival
                if (moveComponent.IsFinished)
                {
                    StartAttacking();
                }
                break;

            case EnemyState.Attacking:
                if (!attackComponentRaw.enabled)
                {
                    // Transition back to Idle if attack is disabled
                    currentState = EnemyState.Escaping;
                }
                break;

            case EnemyState.Escaping:
                
                break;

            case EnemyState.Dying:
                
                break;

            case EnemyState.Dead:
                
                break;
        }
    }

    #endregion

    #region Private Methods

    ///// <summary>
    ///// Checks if the enemy has reached its target lane.
    ///// </summary>
    //private void CheckLaneArrival()
    //{
    //    if (moveComponent != null && !moveComponent.enabled)
    //    {
    //        // Enemy has arrived at the lane
    //        StartAttacking();
    //    }
    //}

    /// <summary>
    /// Transitions to Attacking state and enables the attack component.
    /// </summary>
    private void StartAttacking()
    {
        currentState = EnemyState.Attacking;

        attackComponent?.EnableAttack();
    }

    #endregion
}
