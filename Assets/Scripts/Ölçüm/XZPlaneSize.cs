using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XZPlaneSize : MonoBehaviour
{
    void Start()
    {
        // Boyutlar� d�zenli olarak yazd�rmak i�in Coroutine ba�lat
        StartCoroutine(PrintSizeEveryFiveSeconds());
    }

    IEnumerator PrintSizeEveryFiveSeconds()
    {
        while (true)
        {
            // MeshRenderer bile�enini al
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                // MeshRenderer'�n bounds boyutlar�n� al
                Bounds bounds = meshRenderer.bounds;

                // XZ d�zlemindeki ger�ek boyutlar� hesapla
                float width = bounds.size.x; // X ekseni geni�li�i
                float length = bounds.size.z; // Z ekseni uzunlu�u

                // Sonu�lar� konsola yazd�r
                //Debug.Log($"Objenin XZ plane'indeki geni�li�i: {width}, uzunlu�u: {length}");
            }
            else
            {
                Debug.LogError("Bu obje bir MeshRenderer bile�enine sahip de�il!");
                yield break; // MeshRenderer yoksa d�ng�y� durdur
            }

            // 5 saniye bekle
            yield return new WaitForSeconds(5f);
        }
    }
}
