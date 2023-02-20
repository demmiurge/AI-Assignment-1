#ifndef POINTLIGHT_INCLUDED
#define POINTLIGHT_INCLUDED

void PointLight_float(in float3 WorldPos/*, out half3 Direction*/, out half3 Color, out half DistanceAtten, out half ShadowAtten) 
{
#ifdef SHADERGRAPH_PREVIEW
	//Direction = half3(0,1,0);
	Color = 1;
	DistanceAtten = 1;
	ShadowAtten = 1;
#else
	#if SHADOWS_SCREEN
		half4 clipPos = TransformWorldToHClip(WorldPos);
		half4 shadowCoord = ComputeScreenPos(clipPos);
	#else
		half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
	#endif

	#if defined(_ADDITIONAL_LIGHTS_VERTEX)
		int pixelLightCount = GetAdditionalLightsCount();
		 for (int i = 0; i < pixelLightCount; ++i)
		{
			Light pointLight = GetAdditionalLight(shadowCoord, WorldPos);
			//Direction = pointLight.direction;
			Color = pointLight.color;
			DistanceAtten = pointLight.distanceAttenuation;
			ShadowAtten = pointLight.shadowAttenuation;
		}
	#endif
#endif
}
#endif