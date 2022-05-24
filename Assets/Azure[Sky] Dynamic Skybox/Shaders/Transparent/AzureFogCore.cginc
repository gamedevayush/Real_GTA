#include "Assets/Azure[Sky] Dynamic Skybox/Shaders/Transparent/AzureComputeFogScattering.cginc"

// Apply fog scattering.
float4 ApplyAzureFog (float4 fragOutput, float3 worldPos)
{	
	#ifdef UNITY_PASS_FORWARDADD
		float3 fogScatteringColor = 0;
	#else
		float3 fogScatteringColor = AzureComputeFogScattering(worldPos);
	#endif

	// Calcule Standard Fog.
	float depth = distance(_WorldSpaceCameraPos, worldPos);
	float fog = smoothstep(-_Azure_FogBlend, 1.25, depth / _Azure_FogDistance) * _Azure_FogDensity;
	float heightFogDistance = smoothstep(-_Azure_HeightFogBlend, 1.25, depth / _Azure_HeightFogDistance);

	// Calcule Height Fog.
	float3 worldSpaceDirection = mul((float3x3)_Azure_UpMatrix, worldPos.xyz);
	float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
	heightFog = 1.0 - heightFog;
	heightFog *= heightFog;
	heightFog *= heightFogDistance;
	float fogFactor = saturate(fog + heightFog * _Azure_HeightFogDensity);

	// Apply Fog.
	#if defined(_ALPHAPREMULTIPLY_ON)
	fragOutput.a = lerp(fragOutput.a, 1.0, fogFactor);
	#endif
	fogScatteringColor = lerp(fragOutput.rgb, fogScatteringColor, fogFactor * lerp(fragOutput.a, 1.0, 2.0 - fogFactor));
	return float4(fogScatteringColor, fragOutput.a);
}

// Apply fog scattering to additive/multiply blend mode.
float4 ApplyAzureFog (float4 fragOutput, float3 worldPos, float4 fogColor)
{	
	#ifdef UNITY_PASS_FORWARDADD
		float3 fogScatteringColor = 0;
	#else
		float3 fogScatteringColor = fogColor;
	#endif

	// Calcule Standard Fog.
	float depth = distance(_WorldSpaceCameraPos, worldPos);
	float fog = smoothstep(-_Azure_FogBlend, 1.25, depth / _Azure_FogDistance) * _Azure_FogDensity;
	float heightFogDistance = smoothstep(-_Azure_HeightFogBlend, 1.25, depth / _Azure_HeightFogDistance);

	// Calcule Height Fog.
	float3 worldSpaceDirection = mul((float3x3)_Azure_UpMatrix, worldPos.xyz);
	float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
	heightFog = 1.0 - heightFog;
	heightFog *= heightFog;
	heightFog *= heightFogDistance;
	float fogFactor = saturate(fog + heightFog * _Azure_HeightFogDensity);

	// Apply Fog.
	fogScatteringColor = lerp(fragOutput.rgb, fogScatteringColor, fogFactor * lerp(fragOutput.a, 1.0, 2.0 - fogFactor));
	return float4(fogScatteringColor, fragOutput.a);
}

// DEPRECATED - backward compatibility (Actually, the projPos parameter is no longer needed.)
float4 ApplyAzureFog (float4 fragOutput, float4 projPos, float3 worldPos)
{	
	#ifdef UNITY_PASS_FORWARDADD
		float3 fogScatteringColor = 0;
	#else
		float3 fogScatteringColor = AzureComputeFogScattering(worldPos);
	#endif

	// Calcule Standard Fog.
	float depth = distance(_WorldSpaceCameraPos, worldPos);
	float fog = smoothstep(-_Azure_FogBlend, 1.25, depth / _Azure_FogDistance) * _Azure_FogDensity;
	float heightFogDistance = smoothstep(-_Azure_HeightFogBlend, 1.25, depth / _Azure_HeightFogDistance);

	// Calcule Height Fog.
	float3 worldSpaceDirection = mul((float3x3)_Azure_UpMatrix, worldPos.xyz);
	float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
	heightFog = 1.0 - heightFog;
	heightFog *= heightFog;
	heightFog *= heightFogDistance;
	float fogFactor = saturate(fog + heightFog * _Azure_HeightFogDensity);

	// Apply Fog.
	fogScatteringColor = lerp(fragOutput.rgb, fogScatteringColor, fogFactor * lerp(fragOutput.a, 1.0, 2.0 - fogFactor));
	return float4(fogScatteringColor, fragOutput.a);
}