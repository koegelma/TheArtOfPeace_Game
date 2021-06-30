//-----Registered Object Parameters
float4x4 _obj0Model;
float4x4 _obj1Model;
float4x4 _obj2Model;
float4x4 _obj3Model;
float4x4 _obj4Model;

float obj0sm,enabled0;
float4 size0;
float obj1sm,enabled1;
float4 size1;
float obj2sm,enabled2;
float4 size2;
float obj3sm,enabled3;
float4 size3;
float obj4sm,enabled4;
float4 size4;

float3 color0;
float3 color1;
float3 color2;
float3 color3;
float3 color4;

float4 fragSize0;
float3 DirfragSize0;
float fragEnabled0;
float4 fragSize1;
float3 DirfragSize1;
float fragEnabled1;
float4 fragSize2;
float3 DirfragSize2;
float fragEnabled2;
float4 fragSize3;
float3 DirfragSize3;
float fragEnabled3;
float4 fragSize4;
float3 DirfragSize4;
float fragEnabled4;

//-----Registered Shape Branches
float4 MAS_ShapeGenerator(float3 center)
{
   float4 result;

   OP_Mod(center.x, _OPloopTilling.x);
   OP_Mod(center.y, _OPloopTilling.y);
   OP_Mod(center.z, _OPloopTilling.z);
   float4 obj0 = GENERATOR_SHAPE(center,color0,size0 * enabled0,_obj0Model);
   obj0 = OP_Fragment(obj0,fragSize0,DirfragSize0,center,fragEnabled0);

   float4 obj1 = GENERATOR_SHAPE(center,color1,size1 * enabled1,_obj1Model);
   obj1 = OP_Fragment(obj1,fragSize1,DirfragSize1,center,fragEnabled1);

   float4 obj2 = GENERATOR_SHAPE(center,color2,size2 * enabled2,_obj2Model);
   obj2 = OP_Fragment(obj2,fragSize2,DirfragSize2,center,fragEnabled2);

   float4 obj3 = GENERATOR_SHAPE(center,color3,size3 * enabled3,_obj3Model);
   obj3 = OP_Fragment(obj3,fragSize3,DirfragSize3,center,fragEnabled3);

   float4 obj4 = GENERATOR_SHAPE(center,color4,size4 * enabled4,_obj4Model);
   obj4 = OP_Fragment(obj4,fragSize4,DirfragSize4,center,fragEnabled4);

//-----Registered Advanced Operators
//Groups of obj0
   float4 group0 = OP_SmoothSubtraction(obj1,obj0,obj1sm);

//-----Connected Advanced Operators - Straight Unions

//-----Connected Advanced Operators - Smooth Unions


//-----Registered Final Groups & Single Objects
   float4 group1 = OP_SmoothUnion(group0,obj2,_MASrenderSmoothness);
   float4 group2 = OP_SmoothUnion(group1,obj3,_MASrenderSmoothness);
   float4 group3 = OP_SmoothUnion(group2,obj4,_MASrenderSmoothness);
   result = group3;
   return result;
}