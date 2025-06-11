using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerHealth : MonoBehaviour
{
    #region Fields

    public float damageResetTime = 2f;

    private bool isDamaged = false;
    private Coroutine damageCoroutine;

    [SerializeField] private Animator animator;
    // the name of handlayer in the animator
    string handLayerName = "HandLayer";
    private int handLayerIndex = 1;

    // rigbuilder hand rig reference
    [SerializeField] private Rig handIKRig;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (animator == null)
            Debug.LogError("Animator is not assigned in PlayerHealth script. Please assign it in the inspector.");

        handLayerIndex = animator.GetLayerIndex(handLayerName);
        if (handLayerIndex < 0)
        {
            Debug.LogError($"Hand layer '{handLayerName}' not found in animator.");
        }
    }

    #endregion
    /// <summary>
    /// Handles the damage state of the object, triggering effects and game state changes as needed.
    /// </summary>
    /// <remarks>If the object is not already in a damaged state, this method marks it as damaged, starts a
    /// shaking effect,  and begins a timer to reset the damage state. If the object is already damaged, the game
    /// ends.</remarks>
    public void TakeDamage()
    {
        if (!isDamaged)
        {
            // First time damage
            Debug.Log("Player takes damage!");
            isDamaged = true;
            StartShaking();
            // Timer 
            damageCoroutine = StartCoroutine(DamageResetTimer());
        }
        else
        {
            // already damaged, end game
            GameOver();
        }
    }

    private IEnumerator DamageResetTimer()
    {
        yield return new WaitForSeconds(damageResetTime);
        // 2 saniye geçti ve tekrar damage yemediyse
        isDamaged = false;
        StopShaking();
    }

    private void StartShaking()
    {
        Debug.Log("Player shakes!");
        animator.SetTrigger("PlayerShakenTrigger");

        // handlayer weight 0
        if (handLayerIndex >= 0)
            animator.SetLayerWeight(handLayerIndex, 0f);

        // hand rig weight 0
        if (handIKRig != null)
            handIKRig.weight = 0f;

    }

    private void StopShaking()
    {
        Debug.Log("Player normal!");
        // hanlayer weight 1
        if (handLayerIndex >= 0)
            animator.SetLayerWeight(handLayerIndex, 1f);

        // hand rig weight 1
        if (handIKRig != null)
            handIKRig.weight = 1f;
    }

    private void GameOver()
    {
        // Switch to death animation
        animator.SetTrigger("PlayerDeathTrigger");

        // Trigger game over event
        Debug.Log("Game Over!");
        GameOverEvent.instance.TriggerGameOver();

        // Reset the damage state and stop any ongoing damage effects
        isDamaged = false;
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
    }
}
