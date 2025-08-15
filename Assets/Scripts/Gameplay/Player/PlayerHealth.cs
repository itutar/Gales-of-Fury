using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerHealth : MonoBehaviour
{
    #region Fields

    private float damageResetTime = 5f;

    private bool isDamaged = false;
    private Coroutine damageCoroutine;

    [SerializeField] private Animator animator;
    // the name of handlayer in the animator
    string handLayerName = "HandLayer";
    private int handLayerIndex = 1;

    // rigbuilder hand rig reference
    [SerializeField] private Rig handIKRig;

    // Hula female rigidbody 
    [SerializeField] private Rigidbody hulaRigidbody;

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

    private void OnEnable()
    {
        if (GameOverEvent.instance != null)
        {
            GameOverEvent.instance.OnGameOver.AddListener(GameOver);
        }
    }

    private void OnDisable()
    {
        if (GameOverEvent.instance != null)
        {
            GameOverEvent.instance.OnGameOver.RemoveListener(GameOver);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Handles the damage state of the object, triggering effects and game state changes as needed.
    /// </summary>
    /// <remarks>If the object is not already in a damaged state, this method marks it as damaged, starts a
    /// shaking effect,  and begins a timer to reset the damage state. If the object is already damaged, the game
    /// ends.</remarks>
    public void TakeDamage()
    {
        // protected by dolphin!
        if (Blackboard.Instance.GetValue<bool>(BlackboardKey.PlayerInvulnerable))
            return;                     

        if (!isDamaged)
        {
            // First time damage
            isDamaged = true;
            StartShaking();
            // Timer 
            damageCoroutine = StartCoroutine(DamageResetTimer());
        }
        else
        {
            // already damaged, end game
            GameOverEvent.instance.TriggerGameOver();
        }
    }

    #endregion

    #region Private Methods

    private void GameOver()
    {
        // protected by dolphin!
        if (Blackboard.Instance.GetValue<bool>(BlackboardKey.PlayerInvulnerable))
            return;

        // save coin count
        int currentCoins = Blackboard.Instance.GetValue<int>(BlackboardKey.Coin);
        int savedCoins = ES3.Load<int>("coins", 0); // Daha önce kaydedilen coin varsa yükle, yoksa 0 döner
        ES3.Save("coins", savedCoins + currentCoins); // Var olan coinlere yenilerini ekle

        // Save top score if it's higher than previous one
        int currentScore = Blackboard.Instance.GetValue<int>(BlackboardKey.Score);
        int savedTopScore = ES3.Load<int>("topScore", 0); // Daha önce kaydedilen top score yoksa 0 döner

        if (currentScore > savedTopScore)
        {
            ES3.Save("topScore", currentScore);
        }

        // Switch to death animation
        animator.SetTrigger("PlayerDeathTrigger");

        // activate root motion(if revived remember to change it to false)
        animator.applyRootMotion = true;

        // disable kinametic hula rigidbody
        hulaRigidbody.isKinematic = false;

        // deactivate apply root motion after a delay to allow the death animation to play properly
        StartCoroutine(DisableRootMotionDelayed());

        // Reset the damage state and stop any ongoing damage effects
        isDamaged = false;
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
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
        // hanlayer weight 1
        if (handLayerIndex >= 0)
            animator.SetLayerWeight(handLayerIndex, 1f);

        // hand rig weight 1
        if (handIKRig != null)
            handIKRig.weight = 1f;
    }

    

    /// <summary>
    /// disables root motion after a delay to allow the death animation to play properly.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableRootMotionDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        animator.applyRootMotion = false;
    }

    #endregion
}
