#include "Raymarch_Shading.cginc"

//FOG
float MAS_CalculateFog(float3 start, float3 end)
{
	return distance(start, end) * _MASfogDensity;
}

//FRAGMENT INITIALIZATION
float4 MAS_Fragment(MAS_Vert2Frag i) : SV_Target
{
	float3 interRayOrigin = i.ray;
	float3 interRayDir = normalize(i.hpos - i.ray.xyz);

	float interFinalColorProcess;

	float3 interRelativePosition;

	bool interRayResult = MAS_RaymarchHit(interRayOrigin, interRayDir, interRelativePosition, interFinalColorProcess);
	float3 interFinalColor = tex2D(_MAScolorRamp, float2(interFinalColorProcess, 0)).rgb;

	if (interRayResult)
	{
		float l = 1.0;
#ifdef RENDER_LAMBERT
		l = MAS_Lighting(interRelativePosition);
#endif
		float l2 = 1.0;
#ifdef RENDER_OUTLINE
		l2 = MAS_PseudoOutline(_CamWorldSpace.xyz, interRelativePosition);
#endif
#ifdef RENDER_TOON
		l = ceil(l / _MASrenderToonThresh) * _MASrenderToonDens;
		l2 = ceil(l2 / _MASrenderToonThresh) * _MASrenderToonDens;
		interFinalColor = ceil(interFinalColor / _MASrenderToonThresh) * _MASrenderToonDens;
#endif
		return lerp(saturate(float4(interFinalColor * (_MASrenderMainColor.rgb * l) * l2 + _MASrenderSecondColor, 1)), _MASfogColor, clamp(MAS_CalculateFog(interRayOrigin, interRelativePosition), 0, 1));
	}
	else discard;

	return 0;
}