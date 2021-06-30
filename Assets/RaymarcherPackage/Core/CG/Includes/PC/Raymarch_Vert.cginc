//VERTEX INITIALIZATION
MAS_Vert2Frag MAS_Vertex (appdata v)
{
	MAS_Vert2Frag o;
	half dept = v.vertex.z;
	v.vertex.z = 0;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv;
						
	o.ray = _CamFrustum[(int)dept].xyz;
	o.ray /= abs(o.ray.z);
	o.ray = mul(_CamToWorld, o.ray);
	o.hpos = 0.;
	return o;
}
