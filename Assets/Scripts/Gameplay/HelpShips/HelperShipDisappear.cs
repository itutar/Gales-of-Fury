using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperShipDisappear : MonoBehaviour
{
    #region Fields

    private float moveToCornerForce = 12f;    // K��eye g�nderme kuvveti
    private float screenOffsetX = 6f;          // Ekrar�n kenar�ndan ne kadar uzakta yok sayaca��z

    private Coroutine activeCoroutine;

    #endregion

    #region Public Methods

    /// <summary>
    /// D��ar�dan �a�r�ld���nda gemiyi �st k��eye do�ru hareket ettirip yok eden coroutine'i ba�lat�r.
    /// </summary>
    public void StartDisappearance()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(MoveToTopCornerAndDestroy());
    }

    #endregion

    #region Private Methods
    private IEnumerator MoveToTopCornerAndDestroy()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("HelperShipDisappear requires a Rigidbody component!");
            yield break;
        }

        // 1) Rastgele sola-�st veya sa�a-�st y�n� se�
        Vector3 direction = (Random.value < 0.5f)
            ? new Vector3(-1f, 0f, 1f)
            : new Vector3(1f, 0f, 1f);
        direction.Normalize();

        Camera cam = Camera.main;
        bool outOfScreen = false;

        // 2) Ekrar kenar�n� ge�ene kadar kuvvet uygula
        while (!outOfScreen)
        {
            float multiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
            rb.AddForce(direction * moveToCornerForce * multiplier, ForceMode.Force);

            // D�nya uzay�nda anl�k Z pozisyonuna g�re sol-sa� ekran kenarlar�n� hesapla
            float z = transform.position.z;
            float leftX = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, z)).x - screenOffsetX;
            float rightX = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, z)).x + screenOffsetX;

            float x = transform.position.x;
            if (x < leftX || x > rightX)
                outOfScreen = true;

            yield return null;
        }

        // 3) Ekran d���na ��kt�ktan sonra yok et
        Destroy(gameObject);
    }

    #endregion
}
