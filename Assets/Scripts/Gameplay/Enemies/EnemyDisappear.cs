using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDisappear : MonoBehaviour
{
    #region Fields
    
    [SerializeField] DisappearType disappearType;

    // Currently running disappear coroutine (if any)
    private Coroutine activeDisappearCoroutine;

    [SerializeField]
    private float moveToCornerForce = 100f;

    #endregion

    #region Unity Methods


    private void OnEnable()
    {
        EnemyEventManager.Instance.OnEnemyDisappear.AddListener(HandleEnemyDisappear);
    }

    private void OnDisable()
    {
        EnemyEventManager.Instance?.OnEnemyDisappear.RemoveListener(HandleEnemyDisappear);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Starts disappearing
    /// </summary>
    /// <param name="enemy">Disappearing object</param>
    private void HandleEnemyDisappear(GameObject enemy)
    {
        // If this is not my GameObject, ignore (defensive check)
        if (enemy != this.gameObject)
            return;

        // If a coroutine is already running → stop it first
        if (activeDisappearCoroutine != null)
        {
            StopCoroutine(activeDisappearCoroutine);
            activeDisappearCoroutine = null;
        }

        // Now start the desired coroutine
        switch (disappearType)
        {
            case DisappearType.FadeOut:
                activeDisappearCoroutine = StartCoroutine(SinkAndDestroy(enemy));
                break;
            case DisappearType.StayBehind:
                activeDisappearCoroutine = StartCoroutine(MoveBackAndDestroy(enemy));
                break;
            case DisappearType.MoveToTopCornerAndDisappear:
                activeDisappearCoroutine = StartCoroutine(MoveToTopCornerAndDestroy(enemy));
                break;
        }
    }

    /// <summary>
    /// Gradually fades out the specified enemy GameObject by moving the enemy downward.
    /// Once fully disappeared, the GameObject is destroyed to remove it from the scene.
    /// Also gradually disables buoyancy by reducing sinkFactor.
    /// </summary>
    /// <param name="enemy">The enemy GameObject to fade out and destroy</param>
    /// <returns>Coroutine IEnumerator for timed fading and destroying process</returns>
    private IEnumerator SinkAndDestroy(GameObject enemy)
    {
        float duration = 3f;
        float elapsed = 0f;
        float multiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
        duration /= multiplier; // Adjust duration based on speed multiplier

        IBuoyancy buoyancy = enemy.GetComponent<IBuoyancy>();
        if (buoyancy != null)
        {
            buoyancy.SinkFactor = 1f; // başta kaldırma kuvveti tam güçte
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Lower sinkFactor over time (from 1 → 0)
            if (buoyancy != null)
            {
                buoyancy.SinkFactor = Mathf.Lerp(1f, 0f, t);
            }

            // No manual position change! Let buoyancy handle sinking naturally.

            yield return null;
        }

        Destroy(enemy);
    }

    /// <summary>
    /// Gradually moves the enemy GameObject backward along the z-axis over time.
    /// Once sufficiently far, the GameObject is destroyed to remove it from the scene.
    /// </summary>
    /// <param name="enemy">The enemy GameObject to move backward and destroy.</param>
    /// <returns>Coroutine IEnumerator for the backward movement process.</returns>
    private IEnumerator MoveBackAndDestroy(GameObject enemy)
    {
        float duration = 4f;
        float elapsed = 0f;
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Enemy has no Rigidbody for MoveBackAndDestroy!");
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Check if Rigidbody is still valid
            if (rb != null)
            {
                float multiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);

                Vector3 force = Vector3.back * 50f * multiplier; // adjust force strength here
                rb.AddForce(force, ForceMode.Force);
            }
            else
            {
                Debug.LogWarning("Rigidbody destroyed during MoveBackAndDestroy!");
                yield break;
            }

            yield return null;
        }

        Destroy(enemy);
    }

    private IEnumerator MoveToTopCornerAndDestroy(GameObject enemy)
    {
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Enemy has no Rigidbody for MoveToTopCornerAndDisappear!");
            yield break;
        }

        // Randomly pick left or right
        Vector3 direction = (Random.value < 0.5f) ? new Vector3(-1f, 0f, 1f) : new Vector3(1f, 0f, 1f);
        direction.Normalize();

        Camera cam = Camera.main;

        bool isOutOfScreen = false;

        while (!isOutOfScreen)
        {
            // Apply a force toward top corner
            float multiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
            Vector3 force = direction * moveToCornerForce * multiplier;
            rb.AddForce(force, ForceMode.Force);

            // Dynamically calculate left and right edges in world space based on current Z position
            float currentZ = enemy.transform.position.z;
            float leftEdgeX = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, currentZ)).x - 6f;
            float rightEdgeX = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, currentZ)).x + 6f;

            // Check if X position passed the threshold
            float currentX = enemy.transform.position.x;

            if (currentX < leftEdgeX || currentX > rightEdgeX)
            {
                isOutOfScreen = true;
            }

            yield return null;
        }

        Destroy(enemy);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the type of disappearance behavior for the object.
    /// </summary>
    /// <param name="newType">The new disappearance type to apply.</param>
    public void SetDisappearType(DisappearType newType)
    {
        disappearType = newType;
    }

    #endregion
}
