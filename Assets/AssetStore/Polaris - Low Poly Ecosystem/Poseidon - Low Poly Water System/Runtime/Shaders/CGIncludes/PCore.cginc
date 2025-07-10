#ifndef PCORE_INCLUDED
#define PCORE_INCLUDED

#include "PUniforms.cginc"
#include "PWave.cginc"

// Curved-World helper – object space ➜ object space
inline float3 ApplyCurvedWorld(float3 posOS, float bendRadius, float bendSign)
{
    float3 wp = mul(unity_ObjectToWorld, float4(posOS, 1.0)).xyz;
    wp.y += bendSign * (wp.z * wp.z) / bendRadius;
    return mul(unity_WorldToObject, float4(wp, 1.0)).xyz;
}

struct Input  
{
	float4 vertexPos;
	float3 worldPos;
	float4 screenPos;
	float3 normal;
	float fogCoord;
	float crestMask;
};

//// Curved World helper — object-space -> object-space
//// helper – returns float3
//float3 ApplyCurvedWorldXZ(float3 posOS, float radius)
//{
//    // Z boyunca ileri gittikçe Y yukarı bükülsün
//    posOS.y += (posOS.z * posOS.z) / radius;
//    return posOS;
//}
 
void vertexFunction(inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	o.vertexPos = v.vertex;
	
	#if MESH_NOISE
		ApplyMeshNoise(v.vertex, v.texcoord, v.color);
	#endif
	#if WAVE
		ApplyWaveHQ(v.vertex, v.texcoord, v.color, o.crestMask);
	#endif
	ApplyRipple(v.vertex, v.texcoord, v.color);

	// ---- Curved World *burada* uygulanıyor -----------------------
	//v.vertex.y += 100.0;
	//v.vertex.xyz = ApplyCurvedWorldXZ(v.vertex.xyz, 80.0); // 80 = agresif, test için küçük tut

	 // ===== Curved World uygulaması burada ========================
	float bendRadius = 550.0; // veya shader property’si varsa _BendRadius
	float bendSign = -1.0;    // aşağı kıvırmak için -1, yukarı için +1

	v.vertex.xyz   = ApplyCurvedWorld(v.vertex.xyz, bendRadius, bendSign);
	v.texcoord.xyz = ApplyCurvedWorld(v.texcoord.xyz, bendRadius, bendSign);
	v.color.xyz    = ApplyCurvedWorld(v.color.xyz, bendRadius, bendSign);
	// =============================================================

	CalculateNormal(v.vertex, v.texcoord, v.color, v.normal);
	o.normal = v.normal;

	UNITY_TRANSFER_FOG(o, UnityObjectToClipPos(v.vertex));
}

void finalColorFunction(Input i, SurfaceOutputStandardSpecular o, inout fixed4 color)
{
	UNITY_APPLY_FOG(i.fogCoord, color);
}
#endif
