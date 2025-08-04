using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns a dolphin in front of the player and makes the player invulnerable
/// while the dolphin exists.
/// </summary>
public class DolphinBoost : MonoBehaviour
{
    [SerializeField] private GameObject dolphinPrefab;   // prefab with DolphinBehaviour & SharkBuoyancy
    private float duration = 8f;        // seconds of protection
    private float zOffset = 5f;         // dolphin stays this far ahead

    [Header("References")]
    [SerializeField] private PlayerReference playerReference; // ScriptableObject that stores the player

    // Ayný anda birden fazla dolphin-boost olmasýn diye statik taným
    private static Coroutine runningRoutine;

    public void Activate()
    {
        // Eski boost devam ediyorsa bitir
        if (runningRoutine != null)
            ScoreManager.Instance.StopCoroutine(runningRoutine);

        runningRoutine = ScoreManager.Instance.StartCoroutine(BoostRoutine());
    }

    private IEnumerator BoostRoutine()
    {
        // 1) Spawn dolphin in front of the player
        var player = playerReference.player.transform;
        Vector3 spawnPos = player.position + Vector3.forward * zOffset;
        GameObject dolphin = Instantiate(dolphinPrefab, spawnPos, Quaternion.identity);

        // Inject runtime data so the dolphin can follow
        DolphinBehaviour behaviour = dolphin.GetComponent<DolphinBehaviour>();
        behaviour.Init(player, zOffset, duration);

        // 2) Make player invincible
        Blackboard.Instance.SetValue(BlackboardKey.PlayerInvulnerable, true);

        // 3) Wait until the boost expires
        yield return new WaitForSeconds(duration);

        // 4) Clean-up
        Blackboard.Instance.SetValue(BlackboardKey.PlayerInvulnerable, false);
        if (dolphin != null) Destroy(dolphin);
        runningRoutine = null; // Reset the static reference
    }
}
