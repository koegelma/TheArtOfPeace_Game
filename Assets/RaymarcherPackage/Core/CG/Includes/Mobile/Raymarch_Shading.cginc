//Shape normals
float3 MAS_Normals(float3 p)
{
	float2 o = float2(0.001, 0.0);
	float copy = MAS_ShapeGenerator(p).x;
	return normalize(copy - float3(MAS_ShapeGenerator(p - o.xyy).x, MAS_ShapeGenerator(p - o.yxy).x, MAS_ShapeGenerator(p - o.yyx).x));
}

//Global lighting
float MAS_Lighting(float3 p)
{
	float3 LP = normalize(-_MASrenderLightDir.xyz);
	float3 n = MAS_Normals(p);
	float spec = pow(max(dot(LP, n), 0.), 1000. - (_MASrenderSpecularSize * 100.)) * _MASrenderSpecularIntens;
	return clamp(dot(LP, n), _MASrenderShadowUmbraIntens, 1.) + spec;
}

//Soft outline
float MAS_PseudoOutline(float3 a, float3 b)
{
	float3 pc = normalize(a - b);
	float3 n = MAS_Normals(b);
	return pow(dot(pc, n) * _MASrenderOutlineDens, _MASrenderOutlineSoft);
}