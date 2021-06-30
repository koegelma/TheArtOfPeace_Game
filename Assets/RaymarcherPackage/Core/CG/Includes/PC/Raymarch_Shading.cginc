//Normals
float3 MAS_Normals(float3 pos)
{
	float2 offset = float2(_LIGHTjitter, 0.0);
	float c = MAS_ShapeGenerator(pos).w;
	float3 n = c - float3(MAS_ShapeGenerator(pos - offset.xyy).w, MAS_ShapeGenerator(pos - offset.yxy).w, MAS_ShapeGenerator(pos - offset.yyx).w);
	return normalize(n);
}

//Traditional hard/soft Shadows
float MAS_Shadows(float3 rorigin, float3 rdirection, float minint, float maxint, float s)
{
	float result = 1.0;
	for(float i = minint; i < maxint;)
	{
		float d = MAS_ShapeGenerator(rorigin + rdirection * i).w;
		if(d < 0.001) return 0.0;
		result = min(result, s * (1.0 - _SHADEsoft + (d / i)));
		i += d;
	}
	return clamp(result, 0., 1.);
}

//Lambertian shading model
float MAS_Lighting(float3 p, float3 n, float3 rdir)
{
	float result = (dot(-_LIGHTdirect, n) * 0.5 + 0.5) * _LIGHTintens;
	float specular = pow(max(dot(rdir, reflect(-_LIGHTdirect, n)) / (1.0 - _MASrenderSpecularA), 0.0), pow(45.0 - _MASrenderSpecularB, 2)) * _LIGHTintens;
	specular = clamp(specular, 0, 5) * _MASrenderSpecularIntens;

	float shadow = MAS_Shadows(p, -_LIGHTdirect, _SHADEdistance.x, _SHADEdistance.y, _SHADEsoftness) * 0.5 + 0.5;
	shadow = max(0.0, pow(shadow, _SHADEintens));

	result *= max(1.0 - _SHADEenabled, shadow);
	result += specular;
	return result;
}

//Simple triplanar mapping
float3x3 MAS_TriplanarMap(float3 surfp, float s, sampler2D sender)
{
	float3x3 trmp = 
		float3x3(
		tex2D(sender, surfp.yz * s).rgb,
		tex2D(sender, surfp.xz * s).rgb,
		tex2D(sender, surfp.xy * s).rgb
		);
	return trmp;
}