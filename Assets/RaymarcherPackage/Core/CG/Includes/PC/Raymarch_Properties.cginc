
#define MAX_RENDER_DISTANCE 256

float4 _CamWorldSpace;
float4x4 _CamFrustum, _CamToWorld;

//----------RENDER SECTION-------------------------
//---RENDER : Textures & Colors
half4 _MASrenderMainColor, _MASrenderSecondColor;
half _MASrenderMainEmission;
half _MASrenderSpecularA;
half _MASrenderSpecularB;
half _MASrenderSpecularIntens;

uniform sampler2D _MASrenderMainFilter;
uniform sampler2D _MASrenderPrimaryTexture;
float4 _MASrenderPrimaryTexture_ST;
half _MASrenderPrimaryTextureTile;
uniform sampler2D _CameraDepthTexture;

//---RENDER : Quality & Render Settings
bool _MASrenderQualitySetting;
half _MASrenderMaxDistance;
half _MASrenderQuality;

half _MASfogDensity;
half4 _MASfogColor;

//---RENDER : Rendering Options [Fresnel, Toon, Global smoothness]
half _MASrenderFresnel, _MASrenderFresnelMultiplier;
half _MASrenderToonThresh, _MASrenderToonDens;

half _MASrenderSmoothness;
half _MASrenderColorSmoothness;

//----------SHADING SECTION-------------------------
//---SHADING : Lighting
half _LIGHTintens;
half4 _LIGHTdirect;
half _LIGHTjitter;

//---SHADING : Shadows
int _SHADEenabled;
int _SHADEsoft;
half2 _SHADEdistance;
half _SHADEintens;
half _SHADEsoftness;

//---SHADING : Reflection
int _REFLECTenabled;
samplerCUBE _REFLECTcubemap;
int _REFLECTcubemapEnabled;
half _REFLECTintensity;
int _REFLECTphysX;
half _REFLECTphysXemiss;
half _REFLECTphysXintens;
int _REFLECTphysXSampleCount;

//----------GLOBAL OPERATIONS SECTION-----------------
//---GLOBAL OPERATIONS : Loop
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