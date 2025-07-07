using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainMagnusBlackstormBackCannonAnimMovement : MonoBehaviour
{
    #region Fields

    public float moveSpeed = 2f;
    private bool isMovingRight = false;
    private bool isMovingLeft = false;
    private bool isPickingUp = false;
    // first target transform
    [SerializeField] private Transform rightTargetTransform;
    // returning target transform
    [SerializeField] private Transform leftTargetTransform;
    // looking direction
    Vector3 lookDirection;
    // player transform
    private Transform playerTransform;
    // player reference
    public PlayerReference playerReference;


    #endregion

    #region Unity Methods

    private void Start()
    {
        playerTransform = playerReference.player.transform;

    }

    void Update()
    {
        if (isMovingRight)
        {
            //if (Vector3.Distance(transform.position, rightTargetTransform.position) < 0.05f)
            //    isMovingRight = false;
            transform.position = Vector3.Lerp(transform.position, rightTargetTransform.position, moveSpeed * Time.deltaTime);
        }
        if (isMovingLeft)
        {
            //if (Vector3.Distance(transform.position, leftTargetTransform.position) < 0.05f)
            //    isMovingLeft = false;
            transform.position = Vector3.Lerp(transform.position, leftTargetTransform.position, moveSpeed * Time.deltaTime);
        }
        if (isPickingUp)
        {
            lookDirection = (playerTransform.position - transform.position).normalized;
            Quaternion hedefRotasyon = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, hedefRotasyon, 4f * Time.deltaTime);
        }
    }

    #endregion

    #region Public Methods

    // Animasyon Event fonksiyonu (ilk frame)
    public void StartWalkRightMovement()
    {
        lookDirection = (rightTargetTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(lookDirection);
        isMovingRight = true;
        
    }

    // Animasyon Event fonksiyonu (son frame)
    public void StopWalkRightMovement()
    {
        isMovingRight = false;
        
        
        
    }
    /// <summary>
    /// for picking up the barrel
    /// </summary>
    public void PickingUpRotation()
    {
        isPickingUp = true;
        

    }
    /// <summary>
    /// for picking up the barrel
    /// </summary>
    public void StopPickingUp()
    {
        isPickingUp = false;
        
    }
    public void StartWalkLeftMovement()
    {
        lookDirection = (leftTargetTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        isMovingLeft = true;

        
    }

    // Animasyon Event fonksiyonu (son frame)
    public void StopWalkLeftMovement()
    {
        lookDirection = (rightTargetTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(lookDirection);
        isMovingLeft = false;
        
        
    }

    /// <summary>
    /// work out sitting rotation
    /// </summary>
    public void SittinRotation()
    {
        lookDirection = (leftTargetTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    /// <summary>
    /// work out inital pick up animation rotation
    /// </summary>
    public void InitalPickingRotation()
    {
        lookDirection = (leftTargetTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(lookDirection);
        
    }

    #endregion
}
