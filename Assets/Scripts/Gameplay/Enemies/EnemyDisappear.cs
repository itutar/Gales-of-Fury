using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDisappear : MonoBehaviour
{
    #region Fields

    [SerializeField] DisappearType disappearType;

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
        switch (disappearType)
        {
            case DisappearType.FadeOut:
                StartCoroutine(FadeOutAndDestroy(enemy));
                break;
            case DisappearType.StayBehind:
                StartCoroutine(MoveBackAndDestroy(enemy));
                break;
        }
    }

    /// <summary>
    /// Gradually fades out the specified enemy GameObject by moving the enemy downward.
    /// Once fully disappeared, the GameObject is destroyed to remove it from the scene.
    /// </summary>
    /// <param name="enemy">The enemy GameObject to fade out and destroy</param>
    /// <returns>Coroutine IEnumerator for timed fading and destroying process</returns>
    private IEnumerator FadeOutAndDestroy(GameObject enemy)
    {
        float fadeDuration = 2f;

        // moves the enemy downward on the y-axis
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            
            enemy.transform.position += Vector3.down * Time.deltaTime * 0.5f;
            yield return null;
        }

        // Destroy the enemy object after it fades out completely
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
        float moveDuration = 4f;

        // Move the enemy backward along the z-axis
        for (float t = 0; t < moveDuration; t += Time.deltaTime)
        {
            enemy.transform.position += Vector3.back * Time.deltaTime * 4f; 
            yield return null;
        }

        // Destroy the enemy object after it has moved back sufficiently
        Destroy(enemy);
    }


    #endregion
}
