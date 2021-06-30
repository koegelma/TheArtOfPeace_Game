//VERTEX INITIALIZATION
MAS_Vert2Frag MAS_Vertex (appdata v)
{
	MAS_Vert2Frag o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MASrenderPrimaryTexture);
	o.ray = _WorldSpaceCameraPos;
	o.hpos = mul(unity_ObjectToWorld, v.vertex);
	return o;
}
