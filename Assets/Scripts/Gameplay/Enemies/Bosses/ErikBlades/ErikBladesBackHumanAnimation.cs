using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErikBladesBackHumanAnimation : MonoBehaviour
{
    #region Fields

    public GameObject explodingBarrel;
    [SerializeField] private Transform spine;

    Animator animator;

    //barrel variable
    private GameObject currentBarrel;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Called by ErikBladesBehaviour
    /// </summary>
    public void PlayAttack()
    {
        animator.SetTrigger("ErikBladesBackHumanTrigger");
    }


    public void SpawnOnBetweenHands()
    {
        if (spine == null) return;
        Vector3 middlePoint = spine.position;
        middlePoint.z += 0.7f;

        // Instantiate and make it a child of lefthand
        currentBarrel = Instantiate(explodingBarrel);
        currentBarrel.transform.position = middlePoint;
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

    }

    #endregion
}
