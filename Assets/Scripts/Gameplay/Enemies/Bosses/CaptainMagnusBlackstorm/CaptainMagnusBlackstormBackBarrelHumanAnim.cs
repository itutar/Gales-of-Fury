using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainMagnusBlackstormBackBarrelHumanAnim : MonoBehaviour
{
    #region Fields

    public GameObject explodingBarrel;
    [SerializeField] private Transform spine;
    private Transform player;

    // player reference
    public PlayerReference playerReference;

    Animator animator;
    [SerializeField] GameObject Parent;

    //barrel variable
    private GameObject currentBarrel;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        player = playerReference.player.transform;
        animator = this.GetComponent<Animator>();
        EnemyEventManager.Instance.OnEnemyAttack.AddListener(OnAttack);
    }

    // Update is called once per frame
    void Update()
    {
        // for test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("CaptainMagnusBlackstormBackHumanTrigger");

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
            animator.SetTrigger("CaptainMagnusBlackstormBackHumanTrigger");

        }
    }

    #endregion

    #region Public Methods

    public void SpawnOnBetweenHands()
    {
        if (spine == null) return;
        Vector3 middlePoint = spine.position;

        // Instantiate and make it a child of lefthand
        currentBarrel = Instantiate(explodingBarrel);
        currentBarrel.transform.position = middlePoint;

        Vector3 playerDirection = (player.position - currentBarrel.transform.position).normalized;
        float moveAmount = 0.4f; // Ne kadar yaklaþsýn istiyorsan
        currentBarrel.transform.position += playerDirection * moveAmount;
        currentBarrel.transform.SetParent(spine, worldPositionStays: true);
    }

    /// <summary>
    /// Called from animation event to release the barrel
    /// </summary>
    public void ReleaseBarrel()
    {
        currentBarrel.transform.parent = null;
        
        Rigidbody rb = currentBarrel.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce((player.transform.position - currentBarrel.transform.position).normalized * 10f, ForceMode.Impulse);
    }

    #endregion
}
