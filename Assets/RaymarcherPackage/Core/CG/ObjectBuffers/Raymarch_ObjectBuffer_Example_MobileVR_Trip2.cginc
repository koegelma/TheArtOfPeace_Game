//-----Registered Object Parameters
float4 _obj0Model,params0;

//-----Registered Shape Branches
float2 MAS_ShapeGenerator(float3 center)
{
   float2 result;

   float2 obj0 = OP_Fragment(GEN_FRAC_Kleinian(center - _obj0Model.xyz,params0.xy,_obj0Model.w),params0.w,center);
//-----Registered Advanced Operators
//-----Connected Advanced Operators - Straight Unions

//-----Connected Advanced Operators - Smooth Unions


//-----Registered Final Groups & Single Objects
   result = obj0;
   return result;
}