Shader "Hidden/ZBlocker"
{
    SubShader
    {
        Tags { "Queue"="Geometry" }
        Cull Off
        Pass
        {
            ZWrite On
            ColorMask 0 // Ekrana hi�bir renk yazma
        }
    }
}