#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace RaymarcherPackage
{
    /// <summary>
    /// Platform extension for XConvertor
    /// </summary>
    public static class PlatformExtension
    {
        /// <summary>
        /// Additional function for returnable platform
        /// </summary>
        /// <param name="str">Entry</param>
        /// <param name="isMobile">Is Mobile?</param>
        /// <returns>Translated text to specific platform</returns>
        public static string ReturnPlatform(this string str, bool isMobile)
        {
            if (!str.Contains(RM_XConvertor.xxPlatformMacro)) return str;
            return str.Replace(RM_XConvertor.xxPlatformMacro, isMobile ? "Mobile" : "PC");
        }
    }

    /// <summary>
    /// XConvertor is one of the essential components that converts user interaction into shader language (Specifically to the ObjectBuffer) in editor.
    /// Once the user hit Refresh or Recompile button, the XConvertor will look for all Raymarcher objects and re-calculate their parameters.
    /// After all the required processes, the code will be written into ObjectBuffer document which the core shader uses as a source of all raymarching shapes.
    /// 
    /// Last Update: 01.06.2021 [dd/mm/yyyy].
    /// Written by Matej Vanco
    /// </summary>
    public class RM_XConvertor : MonoBehaviour
    {
        [TextArea] public string xxLoadedObjectBuffer;
        [TextArea] public string xxLoadedShapesSource;

        public bool xxShowObjectBuffer = false;
        public bool xxShowShapesSource = false;

        public RM_Master xxRMmaster;
        public RM_MappingMaster xxMappingMaster;

        private const string xxMasterPatternName = "Raymarch_Master_SCENENAME";

        public static readonly string xxPlatformMacro = "#%_PLATFORM_%#";
        private static readonly string xs = Path.DirectorySeparatorChar.ToString();

        private readonly string xxMasterPatternMacroObjBuffer = $"#include \"ObjectBuffers{xs}Raymarch_ObjectBuffer.cginc\"";
        private readonly string xxMasterPatternMacroShapeSrc = $"#include \"ShapeSources{xs}Raymarch_ShapesSource.cginc\"";

        private readonly string xxMasterPatternMacroVert = $"#include \"Includes{xs}{xxPlatformMacro}{xs}Raymarch_Vert.cginc\"";
        private readonly string xxMasterPatternMacroCore = $"#include \"Includes{xs}{xxPlatformMacro}{xs}Raymarch_Core.cginc\"";
        private readonly string xxMasterPatternMacroFrag = $"#include \"Includes{xs}{xxPlatformMacro}{xs}Raymarch_Frag.cginc\"";
        private const string xxMasterPatternMacroBlend = "Cull Off ZWrite Off ZTest Always";

        private const string xxMasterPatternMacroTarget3 = "#pragma target 3.0";
        private const string xxMasterPatternMacroTarget5 = "#pragma target 5.0";

        private readonly string xxObjectBufferSource = $"{xs}CG{xs}ObjectBuffers{xs}Raymarch_ObjectBuffer_";
        private readonly string xxObjectShapesSource = $"{xs}CG{xs}ShapeSources{xs}Raymarch_ShapesSource_";
        private readonly string xxMasterPatternDatabaseSource = $"{xs}CG{xs}Includes{xs}{xxPlatformMacro}{xs}RMMasterPatternDatabase.txt";
        private readonly string xxShapesPatternDatabaseSource = $"{xs}CG{xs}Includes{xs}{xxPlatformMacro}{xs}RMShapesPatternDatabase.txt";

        private const string xxMacro_Render_Flat = "#define RENDER_FLAT";
        private const string xxMacro_Render_Toon = "#define RENDER_TOON";
        private const string xxMacro_Render_Lambert = "#define RENDER_LAMBERT";
        private const string xxMacro_Render_Outline = "#define RENDER_OUTLINE";

        private readonly string xxMasterPatternsSource = $"{xs}CG{xs}";
        private string xxInternalFilePathToSave;

        public int xxInternalTextEditorHeight = 350;

        /// <summary>
        /// Refresh all required components for XConvertor (RM_Master & RM_MappingMaster are required)
        /// </summary>
        public void XXRefreshComponents()
        {
            if (xxRMmaster && xxMappingMaster) return;

            if (!xxRMmaster)
                xxRMmaster = GetComponent<RM_Master>();
            if (!xxMappingMaster)
                xxMappingMaster = GetComponent<RM_MappingMaster>();
            if (!xxRMmaster)
                RMDEBUG.RMDebug(this, "XConvertor doesn't contain RM_Master component which is required component to work with Raymarcher", true);
            if (!xxMappingMaster)
                RMDEBUG.RMDebug(this, "XConvertor doesn't contain RM_MappingMaster component which is required component to work with Raymarcher", true);
        }

        public struct XXInternalOperativeModels
        {
            public List<RM_Object> Registered_OperativeModels;
            public List<string> RegisteredGroups;
            public string Registered_OperativeSourceTargetModel;
        }

        /// <summary>
        /// Compilation Algorithm: recompile & refresh all raymarching objects and write it's source to the shader language
        /// </summary>
        public void XXRecompileRaymarchingModelAlgo()
        {
            XXRefreshComponents();

            xxInternalFilePathToSave = XXGetSpecificPath(this);

            #region Internal Variables
            //---Punctuations, Macros & Core Names
            string MAe = ";"; //Macro for Line End ;
            string MAc = ","; //Macro for Coma ,
            string MAb = "("; //Macro for Starting Bracket )
            string MAbe = ")"; //Macro for Ending Bracket )
            string MAq = " = "; //Macro for Equation =
            string MASSPACE = "   "; //Macro for Space
            string MASNEWLINE = "\n"; //Macro for NewLine

            //---MACRO : Variables
            string MASobjName = "obj";
            string MASobjActivity = "enabled";
            string MASgroupName = "group";
            string MASobjParams = "size";
            string MASmodelName = "Model";
            string MAScolorName = "color";
            string MASmobileParams = "params";

            //---MACRO : Core
            string MASmodels = "";
            string MASshapeParamsAndActiv = "";
            string MAScolors = "";
            string MASoperations = "";

            int MASmodelsCount;
            string MASfunctionStart = "\n{\n   float4 result;\n\n";
            string MASfunctionStartMobile = "\n{\n   float2 result;\n\n";
            string MASfunctionCore = "";
            string MASfunctionEnd = "\n   return result;\n}";

            //---MACRO : Operations [Operation macro MASOP, Operation Parameter macro: MASOPpar]
            string MASOPloop = $"OP_Mod(center.x, _OPloopTilling.x);\n{MASSPACE}OP_Mod(center.y, _OPloopTilling.y);\n{MASSPACE}OP_Mod(center.z, _OPloopTilling.z);";


  
            string MASOPunion = "OP_SmoothUnion(";
            string MASOPunionStraight = "OP_StraightUnion(";
            string MASOPfragment = "OP_Fragment(";
            string MASOPsubtraction = "OP_SmoothSubtraction(";
            string MASOPintersection = "OP_SmoothIntersection(";

            string MASOPparFragmentSize = "fragSize";
            string MASOPparFragmentEnabled = "fragEnabled";

            //---MACRO : Registers
            string MASfunction = "float4 MAS_ShapeGenerator(float3 center)";
            string MASfunctionMobile = "float2 MAS_ShapeGenerator(float3 center)";
            string MASregistered0 = "//-----Registered Object Parameters";
            string MASregistered1 = "//-----Registered Shape Branches";

            //---MACRO : Internal Shader Vars [Starts exactly as in Shader Source]
            string _MASinternSmoothness = "_MASrenderSmoothness";
            string _MASinternColorSmoothness = "_MASrenderColorSmoothness";

            //---ADDITIONAL MACRO : Datatypes
            string Data_Matrix4x4 = "float4x4 ";
            string Data_Vector4 = "float4 ";
            string Data_Vector3 = "float3 ";
            string Data_Vector2 = "float2 ";
            string Data_Float = "float ";

            #endregion

            int xxObjectCount = xxMappingMaster.mapMaximumObjects;

            //----------------------------------------------GENERATION PROCESS 1 - creating model variables
            for (int i = 0; i < xxObjectCount; i++)
            {
                //---Model variable
                string fullModel = Data_Matrix4x4 + "_" + MASobjName + i + MASmodelName + MAe;

                if (xxRMmaster.rmMobileMode)
                {
                    fullModel = Data_Vector4 + "_" + MASobjName + i + MASmodelName + MAc + MASmobileParams + i + MAe;
                    if (i + 1 < xxObjectCount) fullModel += MASNEWLINE;
                    MASmodels += fullModel;
                    continue;
                }

                //---Full shape variable [shape type, size abc, local smoothness, activation]
                string fullShapeParams = Data_Float + MASobjName + i.ToString() + "sm" + MAc
                    + MASobjActivity + i.ToString() + MAe + MASNEWLINE +
                    Data_Vector4 + MASobjParams + i + MAe;

                //---Color variable
                string fullColor = Data_Vector3 + MAScolorName + i.ToString() + MAe;
                //---Operation params variable
                string fullAdditionalOperations =
                Data_Vector4 + MASOPparFragmentSize + i.ToString() + MAe + MASNEWLINE + Data_Vector3 + "Dir" + MASOPparFragmentSize + i.ToString() + MAe
                + MASNEWLINE +
                Data_Float + MASOPparFragmentEnabled + i.ToString() + MAe; //---OPERATION Fragment

                if (i + 1 < xxObjectCount) { fullModel += MASNEWLINE; fullShapeParams += MASNEWLINE; fullColor += MASNEWLINE; fullAdditionalOperations += MASNEWLINE; }

                MASmodels += fullModel;
                MASshapeParamsAndActiv += fullShapeParams;
                MAScolors += fullColor;
                MASoperations += fullAdditionalOperations;
            }

            MASshapeParamsAndActiv = MASNEWLINE + MASshapeParamsAndActiv + MASNEWLINE;
            MASoperations += MASNEWLINE;

            MASmodelsCount = MASmodels.Split('\n').Length;
            if (MASmodelsCount == 0)
            {
                RMDEBUG.RMDebug(this, "There are no objects available... The compilation process will be cancelled");
                return;
            }

            int groupCounter = 0;

            //----------------------------------------------GENERATION PROCESS 2 - core pre-builder - shapes & internal operators
            //---Registering essential data of all possible raymarching models
            for (int i = 0; i < MASmodelsCount; i++)
            {
                RM_Object obj = null;
                if (xxMappingMaster.mapObjectsContainer[i].mapVirtualObject)
                    obj = xxMappingMaster.mapObjectsContainer[i].mapVirtualObject;

                //---Creating possible shape variables
                string o_type = MASSPACE + (xxRMmaster.rmMobileMode ? Data_Vector2 : Data_Vector4) + MASobjName + i.ToString() + MAq;

                //---Fractal or generic?
                if (!obj || (obj && !obj.rmIsFractal))
                {
                    if(!xxRMmaster.rmMobileMode)
                        o_type += "GENERATOR_SHAPE(center" + MAc + MAScolorName + i.ToString() + MAc + MASobjParams + i + " * " + MASobjActivity + i + MAc + "_" + MASobjName + i + MASmodelName + MAbe + MAe + MASNEWLINE;
                    else
                        o_type += MASOPfragment + Data_Vector2 + MAb + "GEN_BASE(center - _" + MASobjName + i + MASmodelName + ".xyz," + MASmobileParams + i + ".xyz)," +
                           "_" + MASobjName + i + MASmodelName + ".w)," + MASmobileParams + i + ".w,center" + MAbe + MAe + MASNEWLINE;
                }
                else
                {
                    string fract = "";
                    switch (obj.rmFractalType)
                    {
                        case RM_Object.RMFractalType.Apollonian: fract = "GEN_FRAC_Apollonian(center"; break;
                        case RM_Object.RMFractalType.Kleinian: fract = "GEN_FRAC_Kleinian(center"; break;
                        case RM_Object.RMFractalType.Mandelbulb: fract = "GEN_FRAC_Mandelbulb(center"; break;
                        case RM_Object.RMFractalType.Tetrahedron: fract = "GEN_FRAC_Tetrahedron(center"; break;
                    }
                    if (!xxRMmaster.rmMobileMode)
                        o_type += fract + MAc + MASobjParams + i + ".xyz" + MAc + "_" + MASobjName + i + MASmodelName + MAc + MAScolorName + i.ToString() + MAbe + MAe + MASNEWLINE;
                    else
                        o_type += MASOPfragment +
                            fract + " - " + "_" + MASobjName + i + MASmodelName + ".xyz" + MAc + 
                            MASmobileParams + i + ".xy" + MAc + 
                            "_" + MASobjName + i + MASmodelName + ".w)," + MASmobileParams + i + ".w,center" + MAbe + MAe + MASNEWLINE;
                }
                MASfunctionCore += o_type;

                if (xxRMmaster.rmMobileMode) continue;

                //---Add operation condition
                MASfunctionCore += MASSPACE + MASobjName + i.ToString() + MAq + MASOPfragment + 
                    MASobjName + i.ToString() + MAc + MASOPparFragmentSize + i.ToString() + MAc + "Dir" + 
                    MASOPparFragmentSize + i.ToString() + MAc + "center" + MAc + MASOPparFragmentEnabled + i.ToString() + MAbe + MAe + MASNEWLINE + MASNEWLINE;
            }

            MASfunctionCore += "//-----Registered Advanced Operators";

            List<XXInternalOperativeModels> Registered_Operatives = new List<XXInternalOperativeModels>();
            List<string> Registered_NONOperativeModels = new List<string>();

            //----------------------------------------------GENERATION PROCESS 3 - core pre-builder - registering external operators (with/ without operator)
            //---Registering models with external operation - SUBTRACTION & INTERSECTION
            for (int i = 0; i < MASmodelsCount; i++)
            {
                if (!xxMappingMaster.mapObjectsContainer[i].mapVirtualObject) 
                    continue;
                RM_Object rmObj = xxMappingMaster.mapObjectsContainer[i].mapVirtualObject;

                if (rmObj.rmExternalOperation != RM_Object.RMExternalOperation.None && (rmObj.rmOPOperator_Target != null))
                {
                    bool found = false;
                    foreach (XXInternalOperativeModels s in Registered_Operatives)
                    {
                        if (s.Registered_OperativeSourceTargetModel == rmObj.rmOPOperator_Target.rmMyBornName)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) continue;
                }
                else continue;

                string targetName = rmObj.rmOPOperator_Target.rmMyBornName;
                XXInternalOperativeModels opModel = new XXInternalOperativeModels
                {
                    Registered_OperativeSourceTargetModel = targetName,
                    Registered_OperativeModels = new List<RM_Object>(),
                    RegisteredGroups = new List<string>()
                };

                for (int x = 0; x < MASmodelsCount; x++)
                {
                    if (!xxMappingMaster.mapObjectsContainer[x].mapVirtualObject)
                        continue;

                    RM_Object rmObj2 = xxMappingMaster.mapObjectsContainer[x].mapVirtualObject;
                    if (rmObj2.rmExternalOperation != RM_Object.RMExternalOperation.None && (rmObj2.rmOPOperator_Target != null))
                    {
                        if (rmObj2.rmOPOperator_Target.rmMyBornName == targetName)
                            opModel.Registered_OperativeModels.Add(rmObj2);
                    }
                }
                Registered_Operatives.Add(opModel);
            }

            //---Registering models without external operation
            for (int i = 0; i < MASmodelsCount; i++)
            {
                if (xxMappingMaster.mapObjectsContainer[i].mapVirtualObject == null)
                {
                    Registered_NONOperativeModels.Add(MASobjName + i.ToString());
                    continue;
                }
                RM_Object objrm = xxMappingMaster.mapObjectsContainer[i].mapVirtualObject;
                bool found = false;
                foreach (XXInternalOperativeModels opModel in Registered_Operatives)
                {
                    if (objrm.rmMyBornName == opModel.Registered_OperativeSourceTargetModel)
                    {
                        found = true;
                        break;
                    }
                    foreach (RM_Object s in opModel.Registered_OperativeModels)
                    {
                        if (s.rmMyBornName == objrm.rmMyBornName)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
                if (!found) Registered_NONOperativeModels.Add(objrm.rmMyBornName);
            }

            //----------------------------------------------GENERATION PROCESS 4 - core pre-builder - writing operatives [Subtraction/Intersection]
            //---Writing registered operative models
            foreach (XXInternalOperativeModels opModel in Registered_Operatives)
            {
                MASfunctionCore += MASNEWLINE + "//Groups of " + opModel.Registered_OperativeSourceTargetModel + MASNEWLINE;

                foreach (RM_Object s in opModel.Registered_OperativeModels)
                {
                    string operation = MASOPsubtraction;
                    switch(s.rmExternalOperation)
                    {
                        case RM_Object.RMExternalOperation.Intersect:
                            operation = MASOPintersection;
                            break;
                    }
                    if(!xxRMmaster.rmMobileMode)
                        MASfunctionCore += MASSPACE + Data_Vector4 + MASgroupName + groupCounter.ToString() + MAq + operation + s.rmMyBornName + MAc + opModel.Registered_OperativeSourceTargetModel
                       + MAc + s.rmMyBornName + "sm" + MAbe + MAe + MASNEWLINE;
                    else
                        MASfunctionCore += MASSPACE + Data_Vector2 + MASgroupName + groupCounter.ToString() + MAq + operation + s.rmMyBornName + MAc + opModel.Registered_OperativeSourceTargetModel
                   + MAc + _MASinternSmoothness + MAc + _MASinternColorSmoothness + MAbe + MAe + MASNEWLINE;

                    opModel.RegisteredGroups.Add(MASgroupName + groupCounter.ToString());
                    groupCounter++;
                }
            }

            MASfunctionCore += MASNEWLINE;

            MASfunctionCore += "//-----Connected Advanced Operators - Straight Unions";
            List<string> Registered_StraightUnions = new List<string>();

            //----------------------------------------------GENERATION PROCESS 5 - core pre-builder - writing operatives [Straight Unions]
            //---Writing straight-union groups for Advanced Operations
            foreach (XXInternalOperativeModels opModel in Registered_Operatives)
            {
                if (opModel.RegisteredGroups.Count <= 1)
                    continue;
                MASfunctionCore += MASNEWLINE + "//Connection Group of " + opModel.Registered_OperativeSourceTargetModel;
                for (int i = 0; i < opModel.Registered_OperativeModels.Count - 1; i++)
                {
                    string connectFrom = opModel.RegisteredGroups[i];
                    string connectTo = opModel.RegisteredGroups[i + 1];

                    if (i != 0) connectFrom = MASgroupName + (groupCounter - 1).ToString();

                    MASfunctionCore += MASNEWLINE + MASSPACE + (xxRMmaster.rmMobileMode ? Data_Vector2 : Data_Vector4) + MASgroupName + groupCounter.ToString() + MAq
                      + MASOPunionStraight + connectFrom + MAc + connectTo + MAc + _MASinternColorSmoothness
                      + MAbe + MAe;

                    if (i == opModel.Registered_OperativeModels.Count - 2)
                        Registered_StraightUnions.Add(MASgroupName + groupCounter.ToString());

                    groupCounter++;
                }
            }

            MASfunctionCore += MASNEWLINE + MASNEWLINE + "//-----Connected Advanced Operators - Smooth Unions";

            //----------------------------------------------GENERATION PROCESS 6 - core pre-builder - writing operatives [Smooth Unions]
            //---Writing smooth-union groups for Advanced Operations
            for (int i = 0; i < Registered_StraightUnions.Count - 1; i++)
            {
                string connectFrom = Registered_StraightUnions[i];
                string connectTo = Registered_StraightUnions[i + 1];

                if (i != 0) connectFrom = MASgroupName + (groupCounter - 1).ToString();

                MASfunctionCore += MASNEWLINE + MASSPACE + (xxRMmaster.rmMobileMode ? Data_Vector2 : Data_Vector4) + MASgroupName + groupCounter.ToString() + MAq
                  + MASOPunion + connectFrom + MAc + connectTo + MAc + _MASinternSmoothness
                  + MAbe + MAe;
                groupCounter++;
            }

            //----------------------------------------------GENERATION PROCESS 7 - core pre-builder - writing SINGLE operatives [Smooth Unions]
            //---Writing smooth-union groups of single-non-operative groups for Advanced Operations
            if (Registered_Operatives.Count > 1)
            {
                foreach (XXInternalOperativeModels opModel in Registered_Operatives)
                {
                    if (opModel.RegisteredGroups.Count != 1)
                        continue;
                    MASfunctionCore += MASNEWLINE + "//Connection of single " + opModel.RegisteredGroups[0];

                    string connectFrom = MASgroupName + (groupCounter - 1).ToString();
                    string connectTo = opModel.RegisteredGroups[0];

                    MASfunctionCore += MASNEWLINE + MASSPACE + (xxRMmaster.rmMobileMode ? Data_Vector2 : Data_Vector4) + MASgroupName + groupCounter.ToString() + MAq
                      + MASOPunion + connectFrom + MAc + connectTo + MAc + _MASinternSmoothness
                      + MAbe + MAe;
                    groupCounter++;
                }
            }

            MASfunctionCore += MASNEWLINE;

            MASfunctionCore += MASNEWLINE + MASNEWLINE + "//-----Registered Final Groups & Single Objects";

            //----------------------------------------------GENERATION PROCESS 8 - core builder - finalization [Writing non-operative models]
            //---Writing smooth-union groups of single non-operative models
            if (Registered_NONOperativeModels.Count >= 1)
            {
                int discounter = Registered_Operatives.Count == 0 ? 1 : 0;
                for (int i = 0; i < Registered_NONOperativeModels.Count - discounter; i++)
                {
                    string connectFrom = (Registered_Operatives.Count >= 1) ? MASgroupName + (groupCounter - 1).ToString() : Registered_NONOperativeModels[i];
                    string connectTo = Registered_NONOperativeModels[i + discounter];

                    if (i != 0) connectFrom = MASgroupName + (groupCounter - 1).ToString();
                    else if (Registered_Operatives.Count >= 1) connectTo = Registered_NONOperativeModels[i ];

                    MASfunctionCore += MASNEWLINE + MASSPACE + (xxRMmaster.rmMobileMode ? Data_Vector2 : Data_Vector4) + MASgroupName + groupCounter.ToString() + " = "
                      + MASOPunion + connectFrom + MAc + connectTo + MAc + _MASinternSmoothness
                      + MAbe + MAe;

                    groupCounter++;
                }
            }

            if (MASmodelsCount > 1)
                MASfunctionCore += MASNEWLINE + MASSPACE + "result = " + MASgroupName + (groupCounter - 1).ToString() + MAe;
            else if (MASmodelsCount == 1)
                MASfunctionCore += MASNEWLINE + MASSPACE + "result = " + MASobjName + (MASmodelsCount - 1).ToString() + MAe;

            string copyMASfunctionStart = xxRMmaster.rmMobileMode ? MASfunctionStartMobile : MASfunctionStart;
            if (xxRMmaster.SHADER_OPloopEnabled)
                copyMASfunctionStart += MASSPACE + MASOPloop + MASNEWLINE;

            if(xxRMmaster.rmMobileMode)
            {
                xxLoadedObjectBuffer =
                MASregistered0

                + MASNEWLINE
                + MASmodels         //model params (positions,params)
                + MASNEWLINE

                + MASNEWLINE
                + MASregistered1

                + MASNEWLINE
                + MASfunctionMobile //core
                + copyMASfunctionStart
                + MASfunctionCore
                + MASfunctionEnd;   //end
            }
            else
            {
                xxLoadedObjectBuffer =
                MASregistered0

                + MASNEWLINE
                + MASmodels         //models
                + MASNEWLINE
                + MASshapeParamsAndActiv //shape params [shapetype,sizes,localsmoothness,activation]
                + MASNEWLINE
                + MAScolors         //colors
                + MASNEWLINE
                + MASNEWLINE
                + MASoperations     //operations

                + MASNEWLINE
                + MASregistered1

                + MASNEWLINE
                + MASfunction       //core
                + copyMASfunctionStart
                + MASfunctionCore
                + MASfunctionEnd;   //end
            }
            
            File.WriteAllText(xxInternalFilePathToSave, xxLoadedObjectBuffer);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Get specific asset path in editor
        /// </summary>
        public string XXGetSpecificPath(MonoBehaviour sender, int getTypo = 0)
        {
            XXRefreshComponents();

            MonoScript m = MonoScript.FromMonoBehaviour(sender);
            string iAssetPath = AssetDatabase.GetAssetPath(m);
            string iFilePath = Path.Combine(Directory.GetCurrentDirectory(), iAssetPath);
            iFilePath = iFilePath.Replace("/", xs).Replace("\\", xs);
            if (getTypo == 0)
                iFilePath = Path.GetDirectoryName(iFilePath) + xxObjectBufferSource + xxMappingMaster.mapCurrentSceneName + ".cginc";
            else if (getTypo == 1)
                iFilePath = Path.GetDirectoryName(iFilePath) + xxObjectShapesSource + xxMappingMaster.mapCurrentSceneName + ".cginc";
            else if (getTypo == 2)
                iFilePath = Path.GetDirectoryName(iFilePath);
            return iFilePath;
        }

        /// <summary>
        /// Create essential 'Master Pattern' for specific scene
        /// </summary>
        public void XXCreateNewRaymarchMasterPattern(RM_MappingMaster sender, string sceneName, bool VRMode, bool mobileMode, bool webGL)
        {
            XXRefreshMasterShader(sender, sceneName, VRMode, mobileMode, webGL);

            string fullCorePath = XXGetSpecificPath(sender, 2);

            string shapesPatterDatabase = fullCorePath + xxShapesPatternDatabaseSource.ReturnPlatform(mobileMode);            
            string objectBufferDest = fullCorePath + xxObjectBufferSource + sceneName + ".cginc";
            string shapeSourceDest = fullCorePath + xxObjectShapesSource + sceneName + ".cginc";
           
            if (!File.Exists(objectBufferDest))
                File.Create(objectBufferDest).Dispose();
            if (!File.Exists(shapeSourceDest))
                File.Create(shapeSourceDest).Dispose();

            string[] shapesDatabaseLines = File.ReadAllLines(shapesPatterDatabase);

            File.WriteAllLines(shapeSourceDest, shapesDatabaseLines);

            sender.mapCurrentSceneName = sceneName;
            sender.mapSetForTheCurrentScene = true;
        }

        /// <summary>
        /// Remove current raymarcher files including shaders & source files related to the scene
        /// </summary>
        public void XXRemoveCurrentRaymarchMasterPattern(RM_MappingMaster sender, string sceneName)
        {
            string fullCorePath = XXGetSpecificPath(sender, 2);

            try
            {
                //First remove master shader
                string masterPatternsDir = fullCorePath + xxMasterPatternsSource;

                string masterPatternFileName = xxMasterPatternName.Replace("SCENENAME", sceneName);
                string masterPatternShaderDest = masterPatternsDir + masterPatternFileName + ".shader";

                if (File.Exists(masterPatternShaderDest))
                    File.Delete(masterPatternShaderDest);

                //At last remove source files
                string objectBufferDest = fullCorePath + xxObjectBufferSource + sceneName + ".cginc";
                string shapeSourceDest = fullCorePath + xxObjectShapesSource + sceneName + ".cginc";

                if (File.Exists(objectBufferDest))
                    File.Delete(objectBufferDest);
                if (File.Exists(shapeSourceDest))
                    File.Delete(shapeSourceDest);
            }
            catch(IOException e)
            {
                RMDEBUG.RMDebug(this, "Couldn't remove current raymarcher pattern. Exception: " + e.Message);
                return;
            }
        }

        /// <summary>
        /// Refresh main master shader of the current scene
        /// </summary>
        public void XXRefreshMasterShader(RM_MappingMaster sender, string sceneName, bool VRMode, bool mobileMode, bool webGL)
        {
            XXRefreshComponents();

            string fullCorePath = XXGetSpecificPath(sender, 2);
            string masterPatternsDir = fullCorePath + xxMasterPatternsSource;

            string masterPatternDatabase = fullCorePath + xxMasterPatternDatabaseSource.ReturnPlatform(mobileMode);
            string masterPatternFileName = xxMasterPatternName.Replace("SCENENAME", sceneName);
            string masterPatternShaderDest = masterPatternsDir + masterPatternFileName + ".shader";

            if (!File.Exists(masterPatternShaderDest))
                File.Create(masterPatternShaderDest).Dispose();

            string[] masterDatabaseLines = File.ReadAllLines(masterPatternDatabase);

            FileStream str = new FileStream(masterPatternShaderDest, FileMode.Create);
            StreamWriter wr = new StreamWriter(str);

            for (int i = 0; i < masterDatabaseLines.Length; i++)
            {
                string cl = masterDatabaseLines[i];

                if (cl.Contains("#include")) 
                    cl = cl.Replace("\\", xs).Replace("/", xs);

                if (i == 0)
                {
                    string sName = cl.Replace("SCENENAME", sceneName);
                    wr.WriteLine(sName);
                    continue;
                }

                if (cl.Contains(xxMasterPatternMacroObjBuffer))
                    cl = cl.Replace("Raymarch_ObjectBuffer", "Raymarch_ObjectBuffer_" + sceneName);
                if (cl.Contains(xxMasterPatternMacroShapeSrc))
                    cl = cl.Replace("Raymarch_ShapesSource", "Raymarch_ShapesSource_" + sceneName);

                if (VRMode)
                {
                    if (cl.Contains(xxMasterPatternMacroCore.ReturnPlatform(mobileMode)))
                        cl = cl.Replace("Raymarch_Core", "Raymarch_CoreVR");
                    if (cl.Contains(xxMasterPatternMacroFrag.ReturnPlatform(mobileMode)))
                        cl = cl.Replace("Raymarch_Frag", "Raymarch_FragVR");
                    if (cl.Contains(xxMasterPatternMacroVert.ReturnPlatform(mobileMode)))
                        cl = cl.Replace("Raymarch_Vert", "Raymarch_VertVR");
                    if (cl.Contains(xxMasterPatternMacroBlend))
                        cl = "";
                }

                if(mobileMode)
                {
                    if (cl.Contains(xxMacro_Render_Lambert))
                        cl = !xxRMmaster.inSHADER_Render_Lambert ? cl.Replace(xxMacro_Render_Lambert, "") : cl;
                    if (cl.Contains(xxMacro_Render_Toon))
                        cl = !xxRMmaster.inSHADER_Render_Toon ? cl.Replace(xxMacro_Render_Toon, "") : cl;
                    if (cl.Contains(xxMacro_Render_Outline))
                        cl = !xxRMmaster.inSHADER_Render_Outline ? cl.Replace(xxMacro_Render_Outline, "") : cl;
                }
                else
                {
                    if (cl.Contains(xxMacro_Render_Toon))
                        cl = !xxRMmaster.inSHADER_Render_Toon || xxRMmaster.inSHADER_Render_Flat ? cl.Replace(xxMacro_Render_Toon, "") : cl;
                    if (cl.Contains(xxMacro_Render_Flat))
                        cl = !xxRMmaster.inSHADER_Render_Flat ? cl.Replace(xxMacro_Render_Flat, "") : cl;

                    if (webGL)
                    {
                        if (cl.Contains(xxMasterPatternMacroTarget5))
                            cl = xxMasterPatternMacroTarget3;
                    }
                }

                wr.WriteLine(cl);
            }

            wr.Dispose();
            str.Dispose();
        }

        /// <summary>
        /// Checks for correct raymarcher setup
        /// </summary>
        /// <returns>Returns true if the current raymarcher corresponds to the scene & safe-initial setup</returns>
        public bool XXCheckForSpecificPatterns(RM_MappingMaster sender, string sceneName)
        {
            string fullCorePath = XXGetSpecificPath(sender, 2);
            string masterPatternsDir = fullCorePath + xxMasterPatternsSource;

            string masterPatternFileName = xxMasterPatternName.Replace("SCENENAME", sceneName);
            string masterPatternShaderDest = masterPatternsDir + masterPatternFileName + ".shader";
            string objectBufferDest = fullCorePath + xxObjectBufferSource + sceneName + ".cginc";

            if (!File.Exists(masterPatternShaderDest) || !File.Exists(objectBufferDest)) return false;
            else return true;
        }
    }

    [CustomEditor(typeof(RM_XConvertor))]
    [CanEditMultipleObjects]
    public class Raymarch_XEditorEditor : RM_InternalEditor
    {
        private RM_XConvertor targ;

        private void OnEnable()
        {
            targ = target as RM_XConvertor;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();   
            
            RMs();
            RMhelpbox("XConvertor is an editor-only component that manages essential convertions from user-code to the shader code and provides very simple IDE for shader editing.", MessageType.None);
            RMs(5);
            RMbv();
            RMproperty("xxShowObjectBuffer", "Show Object Buffer", "Show object source editor of the current raymarcher renderer");
            RMproperty("xxShowShapesSource", "Show Shape Source" , "Show shapes source editor of the current raymarcher renderer");
            RMbve();

            RMs();

            if (targ.xxShowObjectBuffer)
            {
                RMbv();
                RMbv();
                RMl("<size=14>Object Buffer</size> Shader Editor", true);
                RMhelpbox("Object Buffer is a shader source file that contains all created raymarching objects in the current scene (each scene has its unique shader instance). It is possible to check & edit the source code anytime. Editing the source code is not recommended as the XConvertor manages it's data automatically.", MessageType.None);
                RMbve();

                RMbh(false);
                GUILayout.FlexibleSpace();
                if (RMb("+1", 30) && targ.xxInternalTextEditorHeight + 50 < 1000)
                    targ.xxInternalTextEditorHeight += 50;
                if (RMb("+3", 30) && targ.xxInternalTextEditorHeight + 100 < 1000)
                    targ.xxInternalTextEditorHeight += 100;
                if (RMb("+8", 30) && targ.xxInternalTextEditorHeight + 200 < 1000)
                    targ.xxInternalTextEditorHeight += 200;
                if (RMb("Reset", 60))
                    targ.xxInternalTextEditorHeight = 200;
                GUILayout.FlexibleSpace();
                RMbhe();

                RMbv();
                InternalProperty("xxLoadedObjectBuffer");
                RMbve();

                RMbh(false);
                GUILayout.FlexibleSpace();
                if (RMb("-1", 30) && targ.xxInternalTextEditorHeight - 50 > 20)
                    targ.xxInternalTextEditorHeight -= 50;
                if (RMb("-3", 30) && targ.xxInternalTextEditorHeight - 100 > 20)
                    targ.xxInternalTextEditorHeight -= 100;
                if (RMb("-8", 30) && targ.xxInternalTextEditorHeight - 200 > 20)
                    targ.xxInternalTextEditorHeight -= 200;
                if (RMb("Reset", 60))
                    targ.xxInternalTextEditorHeight = 200;
                GUILayout.FlexibleSpace();
                RMbhe();
                RMbh();
                if (RMb("Load Object Buffer"))
                {
                    string p = targ.XXGetSpecificPath(targ);
                    try
                    {
                        targ.xxLoadedObjectBuffer = File.ReadAllText(p);
                        AssetDatabase.Refresh();
                    }
                    catch
                    {
                        RMDEBUG.RMDebug(targ, "Could not load the shader! Path:[" + p + "]", true);
                    }
                }
                if (RMb("Save Object Buffer"))
                {
                    if (EditorUtility.DisplayDialog("Warning", "You are about to save the edited Object Buffer file. Are you sure to process this action?", "Yes", "No") == false)
                        return;

                    if (string.IsNullOrWhiteSpace(targ.xxLoadedObjectBuffer))
                    {
                        RMDEBUG.RMDebug(targ, "Could not save the edited shader! The source is empty...", true);
                        return;
                    }

                    string p = targ.XXGetSpecificPath(targ);
                    try
                    {
                        File.WriteAllText(p, targ.xxLoadedObjectBuffer);
                        AssetDatabase.Refresh();
                    }
                    catch
                    {
                        RMDEBUG.RMDebug(targ, "Could not save the shader! Path:[" + p + "]", true);
                    }
                }
                RMbhe();
                RMbv();
                if (RMb("Unload Object Buffer"))
                    targ.xxLoadedObjectBuffer = "";
                RMbve();

                RMbve();

                RMs(20);
            }

            if (targ.xxShowShapesSource)
            {
                RMbv();

                RMbv();
                RMl("<size=14>Shapes Source</size> Shader Editor", true);
                RMhelpbox("Shapes Source is a shader source file that contains general shapes that raymarching objects use. For example the built-in shape formulas (Sphere,Box,Cone) are written here. You can edit the file anytime or even make a custom shape formulas.", MessageType.None);

                RMbve();
                RMs();
                RMl("Global available attributes:",true);
                RMl("<color=cyan>float3</color>   <b>p</b>    [Position]", false,10);
                if(targ.xxMappingMaster.mapRmMaster.rmMobileMode)
                {
                    RMl("<color=cyan>half3</color>   <b>params</b>    [Additional Parameters]", false, 10);
                    RMl("params.<b>x</b>    [Param A]", false, 10);
                    RMl("params.<b>y</b>    [Param B]", false, 10);
                    RMl("params.<b>z</b>    [Param C]", false, 10);
                }
                else
                {
                    RMl("<color=cyan>float4</color>   <b>params</b>    [Additional Parameters]", false, 10);
                    RMl("params.<b>x</b>    [Param A]", false, 10);
                    RMl("params.<b>y</b>    [Param B]", false, 10);
                    RMl("params.<b>z</b>    [Param C]", false, 10);
                    RMl("params.<b>w</b>   [Param D]", false, 10);
                }

                RMbv();
                InternalProperty("xxLoadedShapesSource", 400);
                RMbve();

                RMbh();
                if (RMb("Load Shapes Source"))
                {
                    string p = targ.XXGetSpecificPath(targ,1);
                    try
                    {
                        targ.xxLoadedShapesSource = File.ReadAllText(p);
                        AssetDatabase.Refresh();
                    }
                    catch
                    {
                        RMDEBUG.RMDebug(targ, "Could not load the shader! Path:[" + p + "]", true);
                    }
                }
                if (RMb("Save Shapes Source"))
                {
                    if (EditorUtility.DisplayDialog("Warning", "You are about to save the edited Shapes Source file. Are you sure to process this action?", "Yes", "No") == false)
                        return;

                    if (string.IsNullOrWhiteSpace(targ.xxLoadedShapesSource))
                    {
                        RMDEBUG.RMDebug(targ, "Could not save the edited shader! The source is empty...", true);
                        return;
                    }

                    string p = targ.XXGetSpecificPath(targ, 1);
                    try
                    {
                        File.WriteAllText(p, targ.xxLoadedShapesSource);
                        AssetDatabase.Refresh();
                    }
                    catch
                    {
                        RMDEBUG.RMDebug(targ, "Could not save the shader! Path:[" + p + "]", true);
                    }
                }
                RMbhe();
                RMbv();
                if (RMb("Unload Shapes Source"))
                    targ.xxLoadedShapesSource = "";
                RMbve();

                RMbve();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void InternalProperty(string p, float customHeight = 0)
        {
            if(customHeight == 0)
                EditorGUILayout.PropertyField(serializedObject.FindProperty(p), GUILayout.MinHeight(targ.xxInternalTextEditorHeight));
            else
                EditorGUILayout.PropertyField(serializedObject.FindProperty(p), GUILayout.MinHeight(customHeight));
        }
    }
}
#endif