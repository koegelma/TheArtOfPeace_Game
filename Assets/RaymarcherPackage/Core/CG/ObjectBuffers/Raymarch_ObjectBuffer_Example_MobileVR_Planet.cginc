//-----Registered Object Parameters
float4 _obj0Model,params0;
float4 _obj1Model,params1;
float4 _obj2Model,params2;

//-----Registered Shape Branches
float2 MAS_ShapeGenerator(float3 center)
{
   float2 result;

   float2 obj0 = OP_Fragment(float2 (GEN_BASE(center - _obj0Model.xyz,params0.xyz),_obj0Model.w),params0.w,center);
   float2 obj1 = OP_Fragment(float2 (GEN_BASE(center - _obj1Model.xyz,params1.xyz),_obj1Model.w),params1.w,center);
   float2 obj2 = OP_Fragment(float2 (GEN_BASE(center - _obj2Model.xyz,params2.xyz),_obj2Model.w),params2.w,center);
//-----Registered Advanced Operators
//Groups of obj0
   float2 group0 = OP_SmoothIntersection(obj1,obj0,_MASrenderSmoothness,_MASrenderColorSmoothness);
   float2 group1 = OP_SmoothSubtraction(obj2,obj0,_MASrenderSmoothness,_MASrenderColorSmoothness);

//-----Connected Advanced Operators - Straight Unions
//Connection Group of obj0
   float2 group2 = OP_StraightUnion(group0,group1,_MASrenderColorSmoothness);

//-----Connected Advanced Operators - Smooth Unions


//-----Registered Final Groups & Single Objects
   result = group2;
   return result;
}