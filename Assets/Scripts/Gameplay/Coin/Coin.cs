using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    #region Fields

    // Movement support
    [SerializeField] float moveSpeed = 5f;
    // Player collect flag
    private bool isCollected = false;

    // sucked into the player support
    float moveDuration = 0.5f;
    [SerializeField] PlayerReference playerReference;
    private GameObject player;
    bool isSuckedIn = false;
    public int laneIndex;



    #endregion

    #region Unity Methods

    void Start()
    {
        // Get the player position from the PlayerReference
        player = playerReference.player;
    }

    private void OnTriggerEnter(Collider other)
    {
        int playerLane = Blackboard.Instance.GetValue<int>(BlackboardKey.PlayerLane);
        if (other.CompareTag("Player"))
        {
            isCollected = true;
            CoinObjectPool.ReturnCoin(gameObject);
            CoinManager.Instance.Add(1);
            ScoreManager.Instance.Add(1);
        }
    }

    private void Update()
    {
        int playerLane = Blackboard.Instance.GetValue<int>(BlackboardKey.PlayerLane);
        if (!isSuckedIn && laneIndex == playerLane
            && Vector3.Distance(transform.position, player.transform.position) < 3f)
        {
            StartCoroutine(MoveToPlayer());
        }
        transform.position += Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier) * 
            moveSpeed * Time.deltaTime * Vector3.back;
    }

    private void OnBecameInvisible()
    {
        // When Coin is out of camera view, send it back to the pool
        if (!isCollected)
        {
            CoinObjectPool.ReturnCoin(gameObject);
        }
    }

    /// <summary>
    /// Called each time the coin object is enabled, resetting its state for reuse.
    /// </summary>
    private void OnEnable()
    {
        isCollected = false;
        isSuckedIn = false;
    }

    #endregion

    #region Private Methods

    IEnumerator MoveToPlayer()
    {
        isSuckedIn = true;
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < moveDuration)
        {
            Vector3 end = player.transform.position;
            transform.position = Vector3.Lerp(start, end, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    #endregion

}
