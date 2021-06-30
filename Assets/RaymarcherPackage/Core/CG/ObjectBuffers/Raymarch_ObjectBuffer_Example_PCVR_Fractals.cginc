//-----Registered Object Parameters
float4x4 _obj0Model;
float4x4 _obj1Model;

float obj0sm,enabled0;
float4 size0;
float obj1sm,enabled1;
float4 size1;

float3 color0;
float3 color1;

float4 fragSize0;
float3 DirfragSize0;
float fragEnabled0;
float4 fragSize1;
float3 DirfragSize1;
float fragEnabled1;

//-----Registered Shape Branches
float4 MAS_ShapeGenerator(float3 center)
{
   float4 result;

   float4 obj0 = GEN_FRAC_Apollonian(center,size0.xyz,_obj0Model,color0);
   obj0 = OP_Fragment(obj0,fragSize0,DirfragSize0,center,fragEnabled0);

   float4 obj1 = GENERATOR_SHAPE(center,color1,size1 * enabled1,_obj1Model);
   obj1 = OP_Fragment(obj1,fragSize1,DirfragSize1,center,fragEnabled1);

//-----Registered Advanced Operators
//-----Connected Advanced Operators - Straight Unions

//-----Connected Advanced Operators - Smooth Unions


//-----Registered Final Groups & Single Objects
   float4 group0 = OP_SmoothUnion(obj0,obj1,_MASrenderSmoothness);
   result = group0;
   return result;
}