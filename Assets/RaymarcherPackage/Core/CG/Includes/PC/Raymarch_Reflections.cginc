//Traditional reflections
void MAS_Reflections(inout float4 interFinalResult, inout float3 interFinalColor, float3 interRelativePosition, float3 interRayOrigin, float3 interRayDir, float3 n, float interDepth)
{
	interFinalResult += float4(texCUBE(_REFLECTcubemap, n).rgb * _REFLECTintensity, 0) * _REFLECTcubemapEnabled;
	interFinalResult *= max(_REFLECTenabled, interFinalResult);

	if (_REFLECTphysX != 1 || _REFLECTenabled == 0) return;
	
	bool interRayResult;
	for (int samp = 0; samp < _REFLECTphysXSampleCount; samp++)
	{
		interRayDir = normalize(reflect(interRayDir, n));
		interRayOrigin = interRelativePosition + (interRayDir * 0.001);
		interRayResult = MAS_RaymarchHit(interRayOrigin, interRayDir, interDepth, interRelativePosition, interFinalColor);
		if (interRayResult)
		{
			float3 nn = MAS_Normals(interRelativePosition);
			float3 ss = MAS_Lighting(interRelativePosition, nn, interRayDir) * _MASrenderMainColor * interFinalColor;
			interFinalResult += float4(ss * _REFLECTintensity, _REFLECTphysXemiss) * _REFLECTphysXintens;
		}
	}
}
