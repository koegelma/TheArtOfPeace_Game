#include "Raymarch_Shading.cginc"

//FOG
float MAS_CalculateFog(float3 start, float3 end)
{
	return distance(start, end) * _MASfogDensity;
}

//FRAGMENT INITIALIZATION
float4 MAS_Fragment(MAS_Vert2Frag i) : SV_Target
{
	float interDepth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r);
	interDepth *= length(i.ray);

	float3 interRayOrigin = _CamWorldSpace;
	float3 interRayDir = normalize(i.ray.xyz);

	float4 interFinalResult = 0.0;
	float interFinalColorProcess;

	float3 interRelativePosition;

	bool interRayResult = MAS_RaymarchHit(interRayOrigin, interRayDir, interDepth, interRelativePosition, interFinalColorProcess);
	fixed3 interFinalColor = tex2D(_MAScolorRamp, float2(interFinalColorProcess, 0)).rgb;

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
		float4 fCol = saturate(float4(interFinalColor * (_MASrenderMainColor.rgb * l) * l2 + _MASrenderSecondColor, 1));
		return lerp(fCol, _MASfogColor, clamp(MAS_CalculateFog(interRayOrigin, interRelativePosition), 0, 1));
	}
	else interFinalResult = float4(0, 0, 0, 0);

	return saturate(float4(tex2D(_MASrenderMainFilter, i.uv) * (1.0 - interFinalResult.w) + (lerp(interFinalResult.xyz, _MASfogColor, clamp(MAS_CalculateFog(interRayOrigin, interRelativePosition), 0, 1))) * interFinalResult.w, 1.0));
}