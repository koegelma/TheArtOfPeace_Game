#define MAX_RENDER_DISTANCE 256

float4 _CamWorldSpace;
float4x4 _CamFrustum, _CamToWorld;

//----------RENDER SECTION-------------------------
//---RENDER : Textures & Colors
half4 _MASrenderMainColor, _MASrenderSecondColor;

uniform sampler2D _MASrenderMainFilter;
uniform sampler2D _MAScolorRamp;
uniform sampler2D _CameraDepthTexture;

//---RENDER : Quality & Render Settings
bool _MASrenderQualitySetting;
float _MASrenderMaxDistance;
float _MASrenderQuality;

half _MASfogDensity;
half4 _MASfogColor;

//---RENDER : Rendering Options [Soft Outline, Cel Shading, Global Smoothness]
half _MASrenderOutlineSoft, _MASrenderOutlineDens;
half _MASrenderToonThresh, _MASrenderToonDens;

half _MASrenderSpecularIntens, _MASrenderSpecularSize;

half4 _MASrenderLightDir;
half _MASrenderShadowUmbraIntens;

half _MASrenderSmoothness;
half _MASrenderColorSmoothness;

//----------GLOBAL OPERATIONS SECTION-----------------
//---GLOBAL OPERATIONS : Fragment[MobileOnly] + Loop
half3 _OPfragmentSize;
half3 _OPfragmentDirect;

int _OPloopEnabled;
half3 _OPloopTilling;

struct appdata  //--------------------------------VERTEX2FRAG & DATA INITIALIZATION
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
};

struct MAS_Vert2Frag
{
	float4 vertex : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 ray : TEXCOORD1;
	float3 hpos : TEXCOORD2;
};