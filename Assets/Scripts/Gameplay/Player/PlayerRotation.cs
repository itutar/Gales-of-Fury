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
        // Player'ýn mevcut rotasyonunu Quaternion olarak al
        Quaternion currentRotation = transform.rotation;

        // Rotasyonu Euler açýlarýna çevir (X, Y, Z deðerlerine böl)
        Vector3 eulerRotation = currentRotation.eulerAngles;

        // X eksenindeki rotasyonu düzeltmek için kontrol et
        if (eulerRotation.x > 180)
        {
            eulerRotation.x -= 360;  // 0-360 döngüsünden dolayý açýlarý -180 ile +180 aralýðýna getirir
        }

        // X ekseni için sýnýrlarý uygula (-30 ile 30 derece arasý)
        eulerRotation.x = Mathf.Clamp(eulerRotation.x, -30f, 30f);
        eulerRotation.y = Mathf.Clamp(eulerRotation.x, -30f, 30f);
        eulerRotation.z = Mathf.Clamp(eulerRotation.x, -30f, 30f);

        // Geriye kalan rotasyonu uygulamak için Euler açýlarýný tekrar Quaternion'a çevir
        transform.rotation = Quaternion.Euler(eulerRotation);
    }
}
