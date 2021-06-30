//Collection of SDF operators

void OP_Mod(inout float position, float tiling)
#pragma exclude_renderers gles
{
	half halft = tiling * 0.5;
	position = fmod(position + halft, tiling) - halft;
	position = fmod(position - halft, tiling) + halft;
}

float2 OP_SmoothUnion(float2 a, float2 b, half smoothness)
{
	float h = clamp( 0.5 + 0.5 * (b.x - a.x) / smoothness, 0.0, 1.0 );
	float color = lerp(b.y, a.y, h);
	float dist = lerp( b.x, a.x, h ) - smoothness * h * (1.0 - h);
	return float2(dist, color);
}

float2 OP_StraightUnion(float2 a, float2 b, half smoothness)
{
	float h = max(a.x, b.x);
	float h2 = clamp(0.5 + 0.5 * (b.x - a.x) / smoothness, 0.0, 1.0);
	float color = lerp(b.y, a.y, h2 * _MASrenderColorSmoothness);
	return float2(h, color);
}

float2 OP_SmoothSubtraction(float2 a, float2 b, half smoothness, half colsmoothness)
{
	float h = clamp( 0.5 - 0.5 * (b.x + a.x) / smoothness, 0.0, 1.0 );
	float color = lerp(b.y, a.y, h * colsmoothness);
	float final = lerp( b.x, -a.x, h ) + smoothness * h * (1.0 - h);
	return float2(final, color);
}

float2 OP_SmoothIntersection(float2 a, float2 b, half smoothness, half colsmoothness)
{
	float h = clamp( 0.5 - 0.5 * (b.x - a.x) / smoothness, 0.0, 1.0 );
	float color = lerp(b.y, a.y, h * colsmoothness);
	float final = lerp(b.x, a.x, h) + smoothness * h * (1.0 - h);
	return float2(final, color);
}
//-----------------------------------------------------

//--Param - evolution
float2 OP_Fragment(float2 shape, float Param, float3 pos)
{
	float2 a1 = shape;
	float a2 = (sin(_OPfragmentSize.x * (pos.x + _Time.y* _OPfragmentDirect.x))
			  * sin(_OPfragmentSize.y * (pos.y + _Time.y * _OPfragmentDirect.y))
			  * sin(_OPfragmentSize.z * (pos.z + _Time.y * _OPfragmentDirect.z))) * Param;
    return float2(a1.x + a2 , a1.y);
}