//-----Registered Object Parameters
float4x4 _obj0Model;

float obj0sm,enabled0;
float4 size0;

float3 color0;

float4 fragSize0;
float3 DirfragSize0;
float fragEnabled0;

//-----Registered Shape Branches
float4 MAS_ShapeGenerator(float3 center)
{
   float4 result;

   float4 obj0 = GEN_FRAC_Apollonian(center,size0.xyz,_obj0Model,color0);
   obj0 = OP_Fragment(obj0,fragSize0,DirfragSize0,center,fragEnabled0);

//-----Registered Advanced Operators
//-----Connected Advanced Operators - Straight Unions

//-----Connected Advanced Operators - Smooth Unions


//-----Registered Final Groups & Single Objects
   result = obj0;
   return result;
}