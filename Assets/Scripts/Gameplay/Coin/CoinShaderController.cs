using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinShaderController : MonoBehaviour
{
    [SerializeField] private Renderer coinRenderer;

    void Update()
    {
        float rotationY = transform.localEulerAngles.y; // Y rotasyonunu al
        coinRenderer.material.SetFloat("_RotationY", rotationY); // Shader'a gönder
    }
}
