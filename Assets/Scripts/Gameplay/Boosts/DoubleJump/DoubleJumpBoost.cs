using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grants the player one extra mid-air jump for <see cref="duration"/> seconds.
/// </summary>
public class DoubleJumpBoost : MonoBehaviour
{
    private int extraJumpCount = 1; // how many extra jumps we grant
    private float duration = 5f;    // seconds boost stays active

    public void Activate()
    {
        // ScoreManager already lives forever, so it is a safe host for coroutines
        ScoreManager.Instance.StartCoroutine(BoostRoutine());
        BoostUIManager.Instance.ShowBoost(BoostType.DoubleJump, duration);
    }

    private IEnumerator BoostRoutine()
    {
        // enable extra jumps
        Blackboard.Instance.SetValue(BlackboardKey.ExtraJumps, extraJumpCount);

        // wait
        yield return new WaitForSeconds(duration);

        // disable again
        Blackboard.Instance.SetValue(BlackboardKey.ExtraJumps, 0);
    }
}
