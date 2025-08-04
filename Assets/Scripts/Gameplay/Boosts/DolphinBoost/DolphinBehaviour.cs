using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps the dolphin locked to the player's X position
/// while SharkBuoyancy handles Y. Self-destructs after a timeout.
/// </summary>
public class DolphinBehaviour : MonoBehaviour
{
    private Transform player;
    private float zOffset;
    private float lifeTime;

    public void Init(Transform player, float zOffset, float lifeTime)
    {
        this.player = player;
        this.zOffset = zOffset;
        this.lifeTime = lifeTime;
        StartCoroutine(SelfDestruct());
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 pos = transform.position;
        pos.x = player.position.x;        // follow lane changes
        pos.z = player.position.z + zOffset;
        transform.position = pos;                 // y left untouched for buoyancy
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
