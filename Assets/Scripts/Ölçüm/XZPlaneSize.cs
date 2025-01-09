using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XZPlaneSize : MonoBehaviour
{
    void Start()
    {
        // Boyutlarý düzenli olarak yazdýrmak için Coroutine baþlat
        StartCoroutine(PrintSizeEveryFiveSeconds());
    }

    IEnumerator PrintSizeEveryFiveSeconds()
    {
        while (true)
        {
            // MeshRenderer bileþenini al
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                // MeshRenderer'ýn bounds boyutlarýný al
                Bounds bounds = meshRenderer.bounds;

                // XZ düzlemindeki gerçek boyutlarý hesapla
                float width = bounds.size.x; // X ekseni geniþliði
                float length = bounds.size.z; // Z ekseni uzunluðu

                // Sonuçlarý konsola yazdýr
                //Debug.Log($"Objenin XZ plane'indeki geniþliði: {width}, uzunluðu: {length}");
            }
            else
            {
                Debug.LogError("Bu obje bir MeshRenderer bileþenine sahip deðil!");
                yield break; // MeshRenderer yoksa döngüyü durdur
            }

            // 5 saniye bekle
            yield return new WaitForSeconds(5f);
        }
    }
}
