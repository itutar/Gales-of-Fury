using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Player'�n mevcut rotasyonunu Quaternion olarak al
        Quaternion currentRotation = transform.rotation;

        // Rotasyonu Euler a��lar�na �evir (X, Y, Z de�erlerine b�l)
        Vector3 eulerRotation = currentRotation.eulerAngles;

        // X eksenindeki rotasyonu d�zeltmek i�in kontrol et
        if (eulerRotation.x > 180)
        {
            eulerRotation.x -= 360;  // 0-360 d�ng�s�nden dolay� a��lar� -180 ile +180 aral���na getirir
        }

        // X ekseni i�in s�n�rlar� uygula (-30 ile 30 derece aras�)
        eulerRotation.x = Mathf.Clamp(eulerRotation.x, -30f, 30f);
        eulerRotation.y = Mathf.Clamp(eulerRotation.x, -30f, 30f);
        eulerRotation.z = Mathf.Clamp(eulerRotation.x, -30f, 30f);

        // Geriye kalan rotasyonu uygulamak i�in Euler a��lar�n� tekrar Quaternion'a �evir
        transform.rotation = Quaternion.Euler(eulerRotation);
    }
}
