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
	float3 interFinalColor;

	float3 interRelativePosition;

	bool interRayResult = MAS_RaymarchHit(interRayOrigin, interRayDir, interDepth, interRelativePosition, interFinalColor);

	if (interRayResult)
	{
#ifdef RENDER_FLAT //Flat Rendering
			float interTime;
			for (int xy = 0; xy < MAX_RENDER_DISTANCE; xy++)
			{
				interRelativePosition = interRayOrigin + interRayDir * interTime;

				float interDist = MAS_ShapeGenerator(interRelativePosition).w;
				if (interDist < _MASrenderQuality)
				{
					float4 fCol = saturate(float4(((xy / _MASrenderFresnel * _MASrenderFresnelMultiplier) / (float)_MASrenderMaxDistance) * _MASrenderSecondColor.rgb + _MASrenderMainColor.rgb * interFinalColor, 1));
					fCol = lerp(fCol, _MASfogColor, clamp(MAS_CalculateFog(interRayOrigin, interRelativePosition), 0, 1));
					return fCol;
				}
				interTime += interDist;
			}
#else //High-Lambertian Rendering
			float3 n = MAS_Normals(interRelativePosition);
			float3 s = MAS_Lighting(interRelativePosition, n, interRayDir) * _MASrenderMainColor;
			s *= float4(interFinalColor, 1);
			interFinalResult = float4(s, 1);
			float t = 1.0;
#ifdef RENDER_TOON
			t = floor(interFinalResult.rgb / _MASrenderToonThresh) * _MASrenderToonDens;
#endif
			interFinalResult.rgb *= t;
			float3x3 textureSample = MAS_TriplanarMap(interRelativePosition, _MASrenderPrimaryTextureTile, _MASrenderPrimaryTexture) * _MASrenderMainEmission;
			interFinalResult *= float4(textureSample[0].xyz * textureSample[1].xyz * textureSample[2].xyz, 1);

			MAS_Reflections(interFinalResult, interFinalColor, interRelativePosition, interRayOrigin, interRayDir, n, interDepth);
#endif
	}
	else interFinalResult = float4(0,0,0,0);

    return saturate(float4(tex2D(_MASrenderMainFilter, i.uv) * (1.0 - interFinalResult.w) + (lerp(interFinalResult.xyz, _MASfogColor, clamp(MAS_CalculateFog(interRayOrigin, interRelativePosition), 0, 1))) * interFinalResult.w, 1.0));
}