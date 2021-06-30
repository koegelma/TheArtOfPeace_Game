bool MAS_RaymarchHit(float3 position, float3 direction, float depth, inout float3 p, inout float color)
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

		float2 dist = MAS_ShapeGenerator(p);

		if (dist.x < _MASrenderQuality)
		{
			result = true;
			color = dist.y;
			break;
		}
		t += dist.x;
	}

	return result;
}