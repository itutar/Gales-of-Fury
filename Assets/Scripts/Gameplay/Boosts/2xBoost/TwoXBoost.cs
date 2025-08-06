using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoXBoost : MonoBehaviour
{
    float multiplier = 2f;
    float duration = 5f;
    public void Activate()
    {
        // score manager gameobject won't be destroyed 
        // so we can safely start a coroutine from it
        ScoreManager.Instance.StartCoroutine(BoostRoutine());
        BoostUIManager.Instance.ShowBoost(BoostType.TwoX, duration);
    }

    /// <summary>
    /// Temporarily increases the speed multiplier for a specified duration.
    /// </summary>
    /// <remarks>This method adjusts the speed multiplier stored in the blackboard to a boosted value  for the
    /// duration specified. After the duration elapses, the speed multiplier is  restored to its original
    /// value.</remarks>
    /// <returns></returns>
    private IEnumerator BoostRoutine()
    {
        
        float original = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
        Blackboard.Instance.SetValue(BlackboardKey.SpeedMultiplier, original * multiplier);

        yield return new WaitForSeconds(duration);
        Blackboard.Instance.SetValue(BlackboardKey.SpeedMultiplier, original);
    }

}
