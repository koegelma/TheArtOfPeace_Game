//FOG
float MAS_CalculateFog(float3 start, float3 end)
{
	return distance(start, end) * _MASfogDensity;
}

//FRAGMENT INITIALIZATION VR
float4 MAS_Fragment(MAS_Vert2Frag i) : SV_Target
{
	//VR_DIFF
	float3 interRayOrigin = i.ray;
	float3 interRayDir = normalize(i.hpos - i.ray.xyz);
	//-------

	float4 interFinalResult = 0.0;
	float3 interFinalColor;

	float3 interRelativePosition;

	bool interRayResult = MAS_RaymarchHit(interRayOrigin, interRayDir, 0, interRelativePosition, interFinalColor);

	if (interRayResult)
	{
#ifdef RENDER_FLAT //Flat Rendering
		float interTime;
		for (int i = 0; i < MAX_RENDER_DISTANCE; i++)
		{
			interRelativePosition = interRayOrigin + interRayDir * interTime;

			float interDist = MAS_ShapeGenerator(interRelativePosition).w;
			if (interDist < _MASrenderQuality)
			{
				float4 fCol = saturate(float4(((i / _MASrenderFresnel * _MASrenderFresnelMultiplier) / (float)_MASrenderMaxDistance) * _MASrenderSecondColor.rgb + _MASrenderMainColor.rgb * interFinalColor, 1));
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

		MAS_Reflections(interFinalResult, interFinalColor, interRelativePosition, interRayOrigin, interRayDir, n, 0);
#endif
	}
	//VR_DIFF
	else discard;
	//-------
	//VR_DIFF
	return saturate(float4(lerp(interFinalResult.rgb, _MASfogColor.rgb, clamp(MAS_CalculateFog(interRayOrigin, interRelativePosition), 0, 1)), 1.0));
	//-------
}