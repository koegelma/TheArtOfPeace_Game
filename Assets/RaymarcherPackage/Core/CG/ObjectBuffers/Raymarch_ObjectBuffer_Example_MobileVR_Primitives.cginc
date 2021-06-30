//-----Registered Object Parameters
float4 _obj0Model,params0;
float4 _obj1Model,params1;
float4 _obj2Model,params2;
float4 _obj3Model,params3;

//-----Registered Shape Branches
float2 MAS_ShapeGenerator(float3 center)
{
   float2 result;

   float2 obj0 = OP_Fragment(float2 (GEN_BASE(center - _obj0Model.xyz,params0.xyz),_obj0Model.w),params0.w,center);
   float2 obj1 = OP_Fragment(float2 (GEN_BASE(center - _obj1Model.xyz,params1.xyz),_obj1Model.w),params1.w,center);
   float2 obj2 = OP_Fragment(float2 (GEN_BASE(center - _obj2Model.xyz,params2.xyz),_obj2Model.w),params2.w,center);
   float2 obj3 = OP_Fragment(float2 (GEN_BASE(center - _obj3Model.xyz,params3.xyz),_obj3Model.w),params3.w,center);
//-----Registered Advanced Operators
//-----Connected Advanced Operators - Straight Unions

//-----Connected Advanced Operators - Smooth Unions


//-----Registered Final Groups & Single Objects
   float2 group0 = OP_SmoothUnion(obj0,obj1,_MASrenderSmoothness);
   float2 group1 = OP_SmoothUnion(group0,obj2,_MASrenderSmoothness);
   float2 group2 = OP_SmoothUnion(group1,obj3,_MASrenderSmoothness);
   result = group2;
   return result;
}