bool MAS_RaymarchHit(float3 position, float3 direction, float depth, inout float3 p, inout float3 color)
{
	float t = 0;
	bool result;

	for (int i = 0; i < MAX_RENDER_DISTANCE; i++)
	{
		if (t > _MASrenderMaxDistance || t >= depth)
		{
			result = false;
			break;
		}

		p = position + direction * t;

		float4 dist = MAS_ShapeGenerator(p);

		if (dist.w < _MASrenderQuality)
		{
			result = true;
			color = dist.rgb;
			break;
		}
		t += dist.w;
	}

	return result;
}