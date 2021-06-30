//Collection of known fractal formulas [Update 29th April 2021]

float4 GEN_FRAC_Apollonian(float3 p, float3 params, float4x4 _objModel, float3 color)
{
	float4 q = mul(_objModel, float4(p, 1));
	p = q.xyz;

	float s = params.x;

	float scale = 1.0;

	float3 cStorage = color;

	for (int i = 0; i < 8; i++)
	{
		p = -1.0 + 2.0 * frac(0.5 * p + 0.5);

		float r2 = dot(p, p);

		color += lerp(1.0 - color, cos(i * 0.5) * pow(r2, 0.5) + 0.15, clamp(params.z, 0, 1));

		float k = s / r2;
		p *= k;
		p.x *= params.y;
		scale *= k;
	}

	return float4(color * cStorage, 0.25 * abs(p.y) / scale);
}

float4 GEN_FRAC_Kleinian(float3 c, float3 params, float4x4 _objModel, float3 color)
{
	float4 q = mul(_objModel, float4(c, 1));
	float3 p = q.xyz;

	float DEfactor = 1.0;

	float3 cStorage = color;

	for (int i = 0; i < 6; i++)
	{
		p = -1.0 + 2.0 * frac(0.5 * p + 0.5);
		float k = max(0.70968 / dot(p, p) * params.x, 1);

		color += lerp(1.0 - color, sin(i * 2.5) * pow(k, 0.5) + 0.15, clamp(params.z, 0, 1));

		p *= k;
		p.xy *= params.y;
		DEfactor *= k + 0.05;
	}

	float rxy = length(p.xy);

	return float4(color * cStorage, max(rxy - 0.92784, abs(rxy * p.z) / length(p)) / DEfactor);
}

float4 GEN_FRAC_Mandelbulb(float3 p, float3 params, float4x4 _objModel, float3 color)
{

	float4 q = mul(_objModel, float4(p, 1));
	float3 z = q.xyz;

	float m = dot(z, z);

	float dz = 1.0;
	float3 cStorage = color;

	for (int i = 0; i < 5; i++)
	{
		dz = 8.0 * pow(abs(m), 3.5) * dz + 1.0;

		float r = length(z);
		float b = params.x * acos(clamp(z.y / r, -1.0, 1.0));
		float a = params.y * atan2(z.x, z.z);
		z = z + pow(r, 8.0) * float3(sin(b) * sin(a), cos(b), sin(b) * cos(a));

		color += lerp(1.0 - color, atan(log(i * 0.35)) * sin(i * 0.5), clamp(params.z, 0, 1));

		m = dot(z, z);
		if (m > 2.0) break;
	}

	return float4(color * cStorage, 0.25 * log(m) * sqrt(m) / dz);
}

float4 GEN_FRAC_Tetrahedron(float3 pos, float3 params, float4x4 _objModel, float3 color)
{
	float4 q = mul(_objModel, float4(pos, 1));
	pos = q.xyz;

	const float3 a1 = float3(1.0, 1.0, 1.0);
	const float3 a2 = float3(-1.0, -1.0, 1.0);
	const float3 a3 = float3(1.0, -1.0, -1.0);
	const float3 a4 = float3(-1.0, 1.0, -1.0);

	const float scale = params.x;
	float d;
	float3 cStorage = color;

	for (int n = 0; n < 30; ++n)
	{
		float3 c = a1;
		float minDist = length(pos - a1);
		d = length(pos - a2) * params.z;

		if (d < minDist) { c = a2; minDist = d; }

		d = length(pos - a3);

		if (d < minDist) { c = a3; minDist = d; }

		d = length(pos - a4);

		if (d < minDist) { c = a4; minDist = d; }

		pos = scale * pos - c * (scale - 1.0) * params.y;
	}

	return float4(color * cStorage, length(pos) * pow(scale, float(-n)));
}