//-----Registered Object Parameters
float4 _obj0Model,params0;
float4 _obj1Model,params1;
float4 _obj2Model,params2;
float4 _obj3Model,params3;
float4 _obj4Model,params4;
float4 _obj5Model,params5;
float4 _obj6Model,params6;
float4 _obj7Model,params7;
float4 _obj8Model,params8;

//-----Registered Shape Branches
float2 MAS_ShapeGenerator(float3 center)
{
   float2 result;

   float2 obj0 = OP_Fragment(float2 (GEN_BASE(center - _obj0Model.xyz,params0.xyz),_obj0Model.w),params0.w,center);
   float2 obj1 = OP_Fragment(float2 (GEN_BASE(center - _obj1Model.xyz,params1.xyz),_obj1Model.w),params1.w,center);
   float2 obj2 = OP_Fragment(float2 (GEN_BASE(center - _obj2Model.xyz,params2.xyz),_obj2Model.w),params2.w,center);
   float2 obj3 = OP_Fragment(float2 (GEN_BASE(center - _obj3Model.xyz,params3.xyz),_obj3Model.w),params3.w,center);
   float2 obj4 = OP_Fragment(float2 (GEN_BASE(center - _obj4Model.xyz,params4.xyz),_obj4Model.w),params4.w,center);
   float2 obj5 = OP_Fragment(float2 (GEN_BASE(center - _obj5Model.xyz,params5.xyz),_obj5Model.w),params5.w,center);
   float2 obj6 = OP_Fragment(float2 (GEN_BASE(center - _obj6Model.xyz,params6.xyz),_obj6Model.w),params6.w,center);
   float2 obj7 = OP_Fragment(float2 (GEN_BASE(center - _obj7Model.xyz,params7.xyz),_obj7Model.w),params7.w,center);
   float2 obj8 = OP_Fragment(float2 (GEN_BASE(center - _obj8Model.xyz,params8.xyz),_obj8Model.w),params8.w,center);
//-----Registered Advanced Operators
//-----Connected Advanced Operators - Straight Unions

//-----Connected Advanced Operators - Smooth Unions


//-----Registered Final Groups & Single Objects
   float2 group0 = OP_SmoothUnion(obj0,obj1,_MASrenderSmoothness);
   float2 group1 = OP_SmoothUnion(group0,obj2,_MASrenderSmoothness);
   float2 group2 = OP_SmoothUnion(group1,obj3,_MASrenderSmoothness);
   float2 group3 = OP_SmoothUnion(group2,obj4,_MASrenderSmoothness);
   float2 group4 = OP_SmoothUnion(group3,obj5,_MASrenderSmoothness);
   float2 group5 = OP_SmoothUnion(group4,obj6,_MASrenderSmoothness);
   float2 group6 = OP_SmoothUnion(group5,obj7,_MASrenderSmoothness);
   float2 group7 = OP_SmoothUnion(group6,obj8,_MASrenderSmoothness);
   result = group7;
   return result;
}