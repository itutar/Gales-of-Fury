using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularPirate3Human3Animation : MonoBehaviour
{
    #region Fields

    public GameObject explodingBarrel;
    [SerializeField] private Transform spine;
    
    Animator animator;
    [SerializeField] GameObject Parent;

    //barrel variable
    private GameObject currentBarrel;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        EnemyEventManager.Instance.OnEnemyAttack.AddListener(OnAttack);
    }

    // Update is called once per frame
    void Update()
    {
        // for test
        if(Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("regularPirate3Human3AttackTrigger");
            SpawnOnBetweenHands(spine);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles the attack event based on the enemy type.
    /// </summary>
    /// <param name="enemy">The enemy performing the attack</param>
    private void OnAttack(GameObject enemy)
    {
        if (enemy == Parent)
        {
            animator.SetTrigger("regularPirate3Human3AttackTrigger");
            SpawnOnBetweenHands(spine);
        }
    }

    private void SpawnOnBetweenHands(Transform spine)
    {
        if (spine == null) return;
        Vector3 middlePoint = spine.position;
        middlePoint.z += 0.7f;
        
        // Instantiate and make it a child of lefthand
        currentBarrel = Instantiate(explodingBarrel);
        currentBarrel.transform.position = middlePoint;
        currentBarrel.transform.SetParent(spine, worldPositionStays: true);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Called from animation event to release the barrel
    /// </summary>
    public void ReleaseBarrel()
    {
        currentBarrel.transform.parent = null;
        Rigidbody rb = currentBarrel.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

    }

    #endregion
}
