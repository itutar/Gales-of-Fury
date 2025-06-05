using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaterContactVFXTrigger : MonoBehaviour
{
    public Transform playerTransform;
    public FindWaterSurfaceLevel findWaterSurface;
    public float contactThreshold = 0.1f;
    public ParticleSystem vfx;

    private bool vfxPlaying = false;

    void Update()
    {
        if (playerTransform == null || findWaterSurface == null || vfx == null)
            return;

        float playerY = playerTransform.position.y;
        float waterY = findWaterSurface.GetWaterSurfaceY();
        float distance = playerY - waterY;

        // Suya temas: playerY <= waterY + contactThreshold
        if (distance <= contactThreshold)
        {
            if (!vfxPlaying)
            {
                vfx.Play();
                vfxPlaying = true;
            }
        }
        else
        {
            if (vfxPlaying)
            {
                vfx.Stop();
                vfxPlaying = false;
            }
        }
    }
}
