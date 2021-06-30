float2x2 rotate(float a)
{
	return float2x2(cos(a), sin(a), -sin(a), cos(a));
}

float GEN_BASE(float3 p, half3 params)
{
	float final = 0;
	p.y -= clamp(p.y, 0.0, params.y);

	//---Sphere*
	float shapeA = length(p) - params.x;

	//---Cube*
	//float shapeB = length(max(abs(p) - float3(params.x, params.y, params.x), 0.0)) - 0.01;

	//---Extended Cube*
	//float3 sas = abs(p) - float3(params.x, params.y, params.x);
	//float shapeC = min(max(sas.x,max(sas.y,sas.z)),0.0) + length(max(sas,0.0));

	//---Pseudo Cone*
	//float shapeD = max(dot(params.xy, float2(length(p.xz), p.y - params.x)), -p.y - params.x);

	//---Donut+custom pseudo rotation formula
	float3 rot = p;
	rot.yx *= normalize(mul(rotate(_Time.x * 65.), rot));
	float3 fin = lerp(p, rot, 0.15);
	float shapeE = length( float2(length(fin.xz)-params.x, fin.y)) - params.y;

	final = lerp(shapeA,shapeE,params.z);

	return final;
}
