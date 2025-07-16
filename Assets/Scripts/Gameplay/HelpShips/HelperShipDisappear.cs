using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperShipDisappear : MonoBehaviour
{
    #region Fields

    private float moveToCornerForce = 12f;    // Köþeye gönderme kuvveti
    private float screenOffsetX = 6f;          // Ekrarýn kenarýndan ne kadar uzakta yok sayacaðýz

    private Coroutine activeCoroutine;

    #endregion

    #region Public Methods

    /// <summary>
    /// Dýþarýdan çaðrýldýðýnda gemiyi üst köþeye doðru hareket ettirip yok eden coroutine'i baþlatýr.
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

        // 1) Rastgele sola-üst veya saða-üst yönü seç
        Vector3 direction = (Random.value < 0.5f)
            ? new Vector3(-1f, 0f, 1f)
            : new Vector3(1f, 0f, 1f);
        direction.Normalize();

        Camera cam = Camera.main;
        bool outOfScreen = false;

        // 2) Ekrar kenarýný geçene kadar kuvvet uygula
        while (!outOfScreen)
        {
            float multiplier = Blackboard.Instance.GetValue<float>(BlackboardKey.SpeedMultiplier);
            rb.AddForce(direction * moveToCornerForce * multiplier, ForceMode.Force);

            // Dünya uzayýnda anlýk Z pozisyonuna göre sol-sað ekran kenarlarýný hesapla
            float z = transform.position.z;
            float leftX = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, z)).x - screenOffsetX;
            float rightX = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, z)).x + screenOffsetX;

            float x = transform.position.x;
            if (x < leftX || x > rightX)
                outOfScreen = true;

            yield return null;
        }

        // 3) Ekran dýþýna çýktýktan sonra yok et
        Destroy(gameObject);
    }

    #endregion
}
