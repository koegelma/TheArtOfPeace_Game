//Source of shape formulas
float GEN_BASE(float3 p, float4 params)
{
	float final = 0;
	p.y -= clamp(p.y, 0.0, params.y);
	
	//---Sphere*
	float shapeA = length(p) - params.x;

	//---Cube*
	//float shapeB = length(max(abs(p) - float3(params.x, params.y, params.z), 0.0)) - 0.01;

	//---Extended Cube*
	//float3 sas = abs(p) - params.xyz;
	//float shapeC = min(max(sas.x,max(sas.y,sas.z)),0.0) + length(max(sas,0.0));

	//---Pseudo Cone*
	//float shapeD = max(dot(params.xy, float2(length(p.xz), p.y - params.z)), -p.y - params.z);

	//---Donut*
	float shapeE = length( float2(length(p.xz)-params.x, p.y)) - params.y;

	//Finalization = morph between shape A and shape B [replace these fields]
	final = lerp(shapeA, shapeE, params.w);

	return final;
}

//Main shape generator; m = transform matrix (pos,rot,scale)
float4 GENERATOR_SHAPE(float3 center, float3 interFinalColor, float4 sizes, float4x4 m)
{
	float4 q = mul(m, float4(center, 1));
	center = q.xyz;

	return float4(interFinalColor, GEN_BASE(center, sizes));
}
