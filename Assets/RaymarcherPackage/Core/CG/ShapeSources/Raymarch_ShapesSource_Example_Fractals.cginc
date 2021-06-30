float GEN_BASE(float3 p, float4 params)
{
	float final = 0;
	p.y -= clamp(p.y, 0.0, params.y);
	
	float shapeA = length(p) - params.x;
	float shapeB = length(max(abs(p) - float3(params.x, params.y, params.z), 0.0)) - 0.01;


	float3 shapeCadd = abs(p) - params.xyz;
	float shapeC = min(max(shapeCadd.x,max(shapeCadd.y,shapeCadd.z)),0.0) + length(max(shapeCadd,0.0));

	float shapeD = length( float2(length(p.xz)-params.x, p.y)) - params.y;

	final = lerp(shapeA, shapeC, params.w);

	return final;
}

//---m = transform matrix (pos,rot,scale)
float4 GENERATOR_SHAPE(float3 center, float3 interFinalColor, float4 sizes, float4x4 m)
{
	float4 q = mul(m, float4(center, 1));
	center = q.xyz;

	return float4(interFinalColor, GEN_BASE(center, sizes));
}