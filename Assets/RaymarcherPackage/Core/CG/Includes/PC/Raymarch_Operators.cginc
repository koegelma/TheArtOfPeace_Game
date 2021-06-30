//Collection of SDF operators

void OP_Mod(inout float position, float tiling)
#pragma exclude_renderers gles
{
	float halft = tiling * 0.5;
	position = fmod(position + halft, tiling) - halft;
	position = fmod(position - halft, tiling) + halft;
}

float4 OP_SmoothUnion(float4 a, float4 b, float smoothness )
{
	float h = clamp( 0.5 + 0.5 * (b.w - a.w) / smoothness, 0.0, 1.0 );
	float3 color = lerp(b.rgb, a.rgb, h);
	float dist = lerp( b.w, a.w, h ) - smoothness * h * (1.0 - h);
	return float4(color, dist);
}

float4 OP_StraightUnion(float4 a, float4 b, float smoothness)
{
	float h = max(a.w, b.w);
	float h2 = clamp(0.5 + 0.5 * (b.w - a.w) / smoothness, 0.0, 1.0);
	float3 color = lerp(b.rgb, a.rgb, h2);
	return float4(color, h);
}

float4 OP_SmoothSubtraction(float4 a, float4 b, float smoothness )
{
	float h = clamp( 0.5 - 0.5 * (b.w + a.w) / smoothness, 0.0, 1.0 );
	float3 color = lerp(b.rgb, a.rgb, h);
	float final = lerp( b.w, -a.w, h ) + smoothness * h * (1.0 - h);
	return float4(color, final);
}

float4 OP_SmoothIntersection(float4 a, float4 b, float smoothness )
{
	float h = clamp( 0.5 - 0.5 * (b.w - a.w) / smoothness, 0.0, 1.0 );
	float3 color = lerp(b.rgb, a.rgb, h);
	float final = lerp(b.w, a.w, h) + smoothness * h * (1.0 - h);
	return float4(color, final);
}
//-----------------------------------------------------

//--Params: x- size.x; y- size.y; z- size.z; w- evolution
float4 OP_Fragment(float4 shape, float4 Params, float3 Directions, float3 pos, float actv = 0)
{
	float4 a1 = shape;
	float a2 = (sin(Params.x * (pos.x + _Time.y*  Directions.x))
		* sin(Params.y * (pos.y + _Time.y*  Directions.y))
		* sin(Params.z * (pos.z + _Time.y*  Directions.z))) * Params.w;
    return float4(a1.rgb, a1.w + (a2 * actv));
}