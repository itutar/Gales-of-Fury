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

    //barrel variable
    private GameObject currentBarrel;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        player = playerReference.player.transform;
        animator = this.GetComponent<Animator>();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// called from CaptainMagnusBlackstormBehaviour script.
    /// </summary>
    public void PlayAttack()
    {
        animator.SetTrigger("CaptainMagnusBlackstormBackHumanTrigger");
    }

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
