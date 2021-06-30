float2 GEN_FRAC_Apollonian(float3 p, float2 params, float color)
{
	float s = params.x;
	float scale = 1.0;
	float cStorage = color;

	for (int i = 0; i < 4; i++)
	{
		p = -1.0 + 2.0 * frac(0.5 * p + 0.5);

		float r2 = dot(p, p);

		color = pow(r2, 0.05);

		float k = s / r2;
		p *= k;
		p.x *= params.y;
		scale *= k;
	}

	return float2(0.5 * abs(p.y) / scale, color + cStorage);
}

float2 GEN_FRAC_Kleinian(float3 p, float2 params, float color)
{
	float DEfactor = 1.0;
	float cStorage = color;

	for (int i = 0; i < 4; i++)
	{
		p = -1.0 + 2.0 * frac(0.5 * p + 0.5);
		float k = max(0.70968 / dot(p, p) * params.x, 1);

		color = pow(k, 0.05);

		p *= k;
		p.xy *= params.y;
		DEfactor *= k + 0.05;
	}

	float rxy = length(p.xy);
	return float2((abs(rxy * p.z) / length(p)) / DEfactor, color + cStorage);
}

float2 GEN_FRAC_Mandelbulb(float3 p, float2 params, float color)
{
	float m = dot(p, p);

	float dz = 1.0;
	float cStorage = color;

	for (int i = 0; i < 4; i++)
	{
		dz = 8.0 * pow(abs(m), 3.5) * dz + 1.0;

		float r = length(p);
		float b = params.x * acos(clamp(p.y / r, -1.0, 1.0));
		float a = params.y * atan2(p.x, p.z);
		p = p + pow(r, 8.0) * float3(sin(b) * sin(a), cos(b), sin(b) * cos(a));

		color = pow(r, 1.25);

		m = dot(p, p);
		if (m > 2.0) break;
	}

	return float2(0.25 * log(m) * sqrt(m) / dz, color * cStorage);
}

float2 GEN_FRAC_Tetrahedron(float3 p, float2 params, float color)
{
	const float3 a1 = float3(1.0, 1.0, 1.0);
	const float3 a2 = float3(-1.0, -1.0, 1.0);
	const float3 a3 = float3(1.0, -1.0, -1.0);
	const float3 a4 = float3(-1.0, 1.0, -1.0);

	const float scale = params.x;
	float d;
	float cStorage = color;

	for (int n = 0; n < 20; ++n)
	{
		float3 c = a1;
		float minDist = length(p - a1);
		d = length(p - a2);

		if (d < minDist) { c = a2; minDist = d; }

		d = length(p - a3);

		if (d < minDist) { c = a3; minDist = d; }

		d = length(p - a4);

		if (d < minDist) { c = a4; minDist = d; }

		color = pow(minDist, 0.15);

		p = scale * p - c * (scale - 1.0) * params.y;
	}

	return float2(length(p) * pow(scale, float(-n)), color * cStorage);
}
