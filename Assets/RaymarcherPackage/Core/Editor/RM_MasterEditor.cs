using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace RaymarcherPackage
{
    /// <summary>
    /// Editor script for RM_Master behaviour. Written by Matej Vanco
    /// </summary>
    [CustomEditor(typeof(RM_Master))]
    [CanEditMultipleObjects]
    public class RM_MasterEditor : RM_InternalEditor
    {
        private RM_Master rm;

        public Texture2D rmEIcon_MasterRoot;
        public Texture2D rmEIcon_RenderSection;
        public Texture2D rmEIcon_VisualSection;
        public Texture2D rmEIcon_ShadingSection;
        public Texture2D rmEIcon_Operations;

        public Texture2D rmEIcon_ButDocs;
        public Texture2D rmEIcon_ButDiscord;
        public Texture2D rmEIcon_ButSupport;

        private readonly string docs = "https://docs.google.com/presentation/d/1D4lW-6_2NqYQjXH1uXBGAXtmiLtEDiCYZOAnIszGbh8/edit?usp=sharing";
        private readonly string supp = "https://matejvanco.com/contact";
        private readonly string diss = "https://discord.com/invite/Ztr8ghQKqC";

        [MenuItem("Window/Raymarcher/Set Up Raymarcher PC/PC [PC,Console]")]
        public static void InitRaymarcherPC()
        {
            InitRaymarcher(false, false, false);
        }

        [MenuItem("Window/Raymarcher/Set Up Raymarcher PC/PC VR [Vive,Oculus]")]
        public static void InitRaymarcherVR()
        {
            InitRaymarcher(true, false, false);
        }

        [MenuItem("Window/Raymarcher/Set Up Raymarcher Mobile/Mobile [iOS,Android]")]
        public static void InitRaymarcherMOBILE()
        {
            InitRaymarcher(false, true, false);
        }

        [MenuItem("Window/Raymarcher/Set Up Raymarcher Mobile/Mobile VR [Oculus Go,Oculus Quest]")]
        public static void InitRaymarcherMOBILEVR()
        {
            InitRaymarcher(true, true, false);
        }

        [MenuItem("Window/Raymarcher/Set Up Raymarcher WebGL/WebGL [Low-End]")]
        public static void InitRaymarcherWebGL_Lowend()
        {
            InitRaymarcher(false, true, true);
        }

        [MenuItem("Window/Raymarcher/Set Up Raymarcher WebGL/WebGL [High-End]")]
        public static void InitRaymarcherWebGL_Highend()
        {
            InitRaymarcher(false, false, true);
        }

        /// <summary>
        /// Initialize raymarcher manually from scratch by one click
        /// </summary>
        /// <param name="VR">Is the target platform VR?</param>
        /// <param name="MOBILE">Is the target platform Mobile?</param>
        /// <param name="WEBGL">Is the target platform webGL? [0=Off,1=High-End,2-Low-End]</param>
        public static void InitRaymarcher(bool VR, bool MOBILE, bool WEBGL)
        {
            Camera c = Camera.main;

            if (c == null)
            {
                Debug.LogError("Raymarcher set up - the system couldn't set up raymarcher because the Main Camera is missing! (Select any camera in your scene and tag it 'MainCamera')");
                return;
            }

            if (EditorUtility.DisplayDialog("Warning", "This will prepare your current scene for raymarcher. Please close your programming environment to prevent " +
                "further issues. Also the scene must be saved in the project. If you've already created the raymarcher, " +
                "the new one will replace the actual one, so you will lost your current progress. Are you sure to process this action?", "Yes", "No") == false)
                return;

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

            if (c.GetComponent<RM_MappingMaster>())
            {
                c.GetComponent<RM_MappingMaster>().MAPclearAllObjects();
                DestroyImmediate(c.GetComponent<RM_MappingMaster>());
            }
            if (c.GetComponent<RM_XConvertor>())
                DestroyImmediate(c.GetComponent<RM_XConvertor>());
            if (c.GetComponent<RM_Master>())
                DestroyImmediate(c.GetComponent<RM_Master>());

            RM_Master rmMaster = c.gameObject.AddComponent<RM_Master>();
            rmMaster.rmWebGL = WEBGL;
            rmMaster.rmVRMode = VR;
            rmMaster.rmMobileMode = MOBILE;

            c.gameObject.AddComponent<RM_XConvertor>();
            c.gameObject.AddComponent<RM_MappingMaster>().MAPsetUpFullRaymarcher();
            RMDEBUG.RMDebug(rmMaster, "Raymarcher renderer is now ready to use.");
            AssetDatabase.Refresh();

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        private void OnEnable()
		{
			rm = target as RM_Master;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //Top info
            RMs();

            RMicon(rmEIcon_MasterRoot);
            if (rm.rmWebGL) 
                RMl("WEB-GL " + (!rm.rmMobileMode ? "High-End" : "Low-End"), true, 17);
            else
            {
                if (rm.rmVRMode && !rm.rmMobileMode)
                    RMl("  VR MODE ENABLED", true, 17);
                else if (!rm.rmVRMode && rm.rmMobileMode)
                    RMl("  MOBILE MODE ENABLED\n  <size=12>Deferred rendering path only</size>", true, 17);
                if (rm.rmVRMode && rm.rmMobileMode)
                    RMl("  MOBILE VR MODE ENABLED", true, 17);
            }

            //Devlog
            RMhelpbox("Raymarcher package version 2.1.0\n15.06.2021 (dd/mm/yyyy)", MessageType.Info);
            RMlvlPlus();
            RMhelpbox("- Added mobile support\n- Added mobile-VR support\n- Added WebGL support\n- Major rendering optimizations\n- Added toon shading\n- Added lambert shading\n- Added outline shading\nand more...", MessageType.None);
            RMlvlMinus();

            //Links & Support
            RMbh();
            if (RMb("  Documentation", rmEIcon_ButDocs))
                Application.OpenURL(docs);
            if (RMb("  Discord", rmEIcon_ButDiscord))
                Application.OpenURL(diss);
            if (RMb("  Support", rmEIcon_ButSupport))
                Application.OpenURL(supp);
            RMbhe();

            //Debug info
            if (RM_Master.rmEnableDebugMessages)
            {
                if (RMb("Disable Editor Debug [Enabled]", 0, "Turn off additional notifications in editor/ at runtime such as warnings, errors and logs related to the Raymarcher renderer"))
                    RM_Master.rmEnableDebugMessages = false;
            }
            else
            {
                if (RMb("Enable Editor Debug [Disabled]", 0, "Turn on additional notifications in editor/ at runtime such as warnings, errors and logs related to the Raymarcher renderer"))
                    RM_Master.rmEnableDebugMessages = true;
            }
			GeneratePresets();
            RMs();

            //Mat src
            RMbv();
            RMproperty("rmMaterialSource", "Material Source", "Material source of the Raymarcher renderer. If NON-VR mode is enabled, the material is built internally (invisible in Project window)");
            RMbve();

            RMs(20);

            //Render section
            RMicon(rmEIcon_RenderSection);

            RMbv();

            RMbv();

            RMproperty("SHADER_MASrenderMaxDistance", "Maximum Render Distance", "Maximum render distance of the Raymarcher renderer. The higher the value is, the more performance it takes");
            RMproperty("SHADER_MASrenderQualitySetting", "Simple Render Settings", "Enable advanced-custom render quality settings");
            if (rm.SHADER_MASrenderQualitySetting)
                RMproperty("SHADER_MASrenderQuality_E", "Render Quality");
            else
                RMproperty("SHADER_MASrenderQuality", "Render Quality", "Step of each layer every frame. The lower the step is, the more performance it takes");

            RMbve();

            RMs();

            RMl("Shading Type", true);
            RMbv();
            if (!rm.rmMobileMode)
            {
                RMproperty("inSHADER_Render_Flat", "Flat Shading", "Enable flat shading type. The property requires shader recompilation");
                if (!rm.inSHADER_Render_Flat)
                    RMproperty("inSHADER_Render_Toon", "Toon Shading", "Enable toon shading type. The property requires shader recompilation");
            }
            else
            {
                RMproperty("inSHADER_Render_Lambert", "Lambert Shading", "Enable lambert shading type. The property requires shader recompilation");
                RMproperty("inSHADER_Render_Toon", "Toon Shading", "Enable toon shading type. The property requires shader recompilation");
            }
            if (rm.rmMobileMode)
                RMproperty("inSHADER_Render_Outline", "Soft Outline", "Enable soft outline. The property requires shader recompilation");
            if (RMb("Recompile Shader"))
            {
                if (EditorUtility.DisplayDialog("Warning", "Would you like to process this action? The raymarcher will re-write and re-compile the shader to prepare the renderer for the desired shading type. Nothing will be lost", "Yes", "No"))
                {
                    rm.MasterRecompileMaster(SceneManager.GetActiveScene().name);
                    AssetDatabase.Refresh();
                }
            }
            RMhelpbox("Press 'Recompile Shader' in case of changing the 'Shading Type' properties", MessageType.None);
            RMbve();
            RMbve();

            RMs();

            //Visual section
            RMicon(rmEIcon_VisualSection);

            RMbv();
            RMproperty("SHADER_MASrenderMainColor", "Main Color", "Global color of the renderer");
            if (rm.inSHADER_Render_Flat || rm.rmMobileMode)
            {
                RMproperty("SHADER_MASrenderSecondColor", "Secondary Color", "Secondary-global color of the renderer");
                if (rm.rmMobileMode)
                {
                    RMs(5);
                    RMproperty("SHADER_MAScolorRamp", "Color Ramp", "Color-Ramp of colors available for raymarcher objects");
                }
            }
            else
                RMproperty("SHADER_MASrenderMainEmission", "Main Emission", "Global emission of the renderer");

            RMs(5);

            if (!rm.inSHADER_Render_Flat && !rm.rmMobileMode)
            {
                RMproperty("SHADER_MASrenderPrimaryTexture", "Additional Texture", "Additional global texture in triplanar mapping");
                RMproperty("SHADER_MASrenderPrimaryTextureTile", "Tiling");
                RMs(5);
            }

            RMs(5);
            RMbv();
            RMproperty("SHADER_MASfog", "Distance Fog", "Enable global-linear distance fog for raymarcher");
            if(rm.SHADER_MASfog)
            {
                RMlvlPlus();
                RMproperty("SHADER_MASfogDensity", "Fog Density");
                RMproperty("SHADER_MASfogColor", "Fog Color");
                RMlvlMinus();
            }
            RMbve();
            RMs(5);

            //Essential hi-render params
            RMbv();
            RMproperty("SHADER_MASrenderSmoothness", "Global Object Smoothness", "Global raymarcher objects smoothness");
            RMproperty("SHADER_MASrenderColorSmoothness", "Global Color Smoothness", "Global raymarcher objects color-smoothness");
            RMbve();

            RMbve();

            RMs();

            //Shading section
            if (rm.inSHADER_Render_Flat && !rm.rmMobileMode)
            {
                RMicon(rmEIcon_ShadingSection);
                RMbv();
                RMl("Flat Shading");
                RMbv();
                RMproperty("SHADER_MASrenderFresnel", "Fresnel Density");
                RMproperty("SHADER_MASrenderFresnelMultiplier", "Fresnel Multiplier");
                RMbve();
                RMs(5);
            }

            if (!rm.inSHADER_Render_Flat && !rm.rmMobileMode)
            {
                RMicon(rmEIcon_ShadingSection);
                RMbv();
                if (rm.inSHADER_Render_Toon)
                {
                    RMl("Toon Shading");
                    RMbv();
                    RMproperty("SHADER_MASrenderToonDens", "Toon Density");
                    RMproperty("SHADER_MASrenderToonThresh", "Toon Threshold");
                    RMbve();
                    RMs(5);
                }
                RMl("Lighting");
                RMbv();
                RMproperty("rmDirectionalLightSource", "Directional Light Source");
                RMproperty("SHADER_LIGHTintens", "Light Intensity");
                RMproperty("SHADER_LIGHTjitter", "Normal Jitter");
                RMs(5);
                RMproperty("SHADER_MASrenderSpecularA", "Specular Size");
                RMproperty("SHADER_MASrenderSpecularB", "Specular Blur");
                RMproperty("SHADER_MASrenderSpecularIntens", "Specular Intensity");
                RMbve();

                RMproperty("SHADER_SHADEenabled", "Shadows");
                if (rm.SHADER_SHADEenabled)
                {
                    RMbv();
                    RMproperty("SHADER_SHADEdistance", "Shadow Distance");
                    RMproperty("SHADER_SHADEintens", "Shadow Intensity");
                    RMproperty("SHADER_SHADEsoft", "Soft Shadow");
                    RMproperty("SHADER_SHADEsoftness", "Shadow Softness");
                    RMbve();
                }
                if (!rm.inSHADER_Render_Flat)
                {
                    RMproperty("SHADER_REFLECTenabled", "Reflections");
                    if (rm.SHADER_REFLECTenabled)
                    {
                        RMbv();
                        RMproperty("SHADER_REFLECTintensity", "Reflection Intensity");
                        RMproperty("SHADER_REFLECTcubemapEnabled", "Cubemap Reflection Enabled");
                        RMproperty("SHADER_REFLECTcubemap", "Cubemap Reflection");
                        RMproperty("SHADER_REFLECTphysX", "Physically Based Reflection");
                        if (rm.SHADER_REFLECTphysX)
                        {
                            RMbv();
                            RMproperty("SHADER_REFLECTphysXSampleCount", "Reflections Count");
                            RMbve();
                            RMproperty("SHADER_REFLECTphysXemiss", "PhysX Reflection Emission");
                            RMproperty("SHADER_REFLECTphysXintens", "PhysX Reflection Intensity");
                        }
                        RMbve();
                    }
                }
            }
            else if(rm.rmMobileMode)
            {
                if (rm.inSHADER_Render_Lambert || rm.inSHADER_Render_Toon || rm.inSHADER_Render_Outline)
                {
                    RMicon(rmEIcon_ShadingSection);
                    RMbv();
                    if (rm.inSHADER_Render_Lambert)
                    {
                        RMl("Lambert Shading");
                        RMbv();
                        RMproperty("rmDirectionalLightSource", "Directional Light Source");
                        RMproperty("SHADER_MASrenderShadowUmbraIntens", "Shadow Umbra Intensity");
                        RMs(5);
                        RMproperty("SHADER_MASrenderSpecularIntensMob", "Specular Intensity");
                        RMproperty("SHADER_MASrenderSpecularSize", "Specular Size");
                        RMbve();
                        RMs(5);
                    }
                    if (rm.inSHADER_Render_Toon)
                    {
                        RMl("Toon Shading");
                        RMbv();
                        RMproperty("SHADER_MASrenderToonDens", "Toon Density");
                        RMproperty("SHADER_MASrenderToonThresh", "Toon Threshold");
                        RMbve();
                        RMs(5);
                    }
                    if (rm.inSHADER_Render_Outline)
                    {
                        RMl("Soft Outline");
                        RMbv();
                        RMproperty("SHADER_MASrenderOutlineDens", "Outline Density");
                        RMproperty("SHADER_MASrenderOutlineSoft", "Outline Softness");
                        RMbve();
                    }
                    RMbve();
                }
            }
            if(!rm.rmMobileMode)
                RMbve();

            RMs(10);
            //Operations
            RMicon(rmEIcon_Operations);
            RMbv();
            if(rm.rmMobileMode)
            {
                RMl("Global Fragment Parameters (Mobile Only)", true);
                RMproperty("SHADER_OPfragmentSize", "Fragment Size");
                RMproperty("SHADER_OPfragmentDirect", "Fragment Direction");

                RMs();
            }
            if (!rm.SHADER_OPloopEnabled)
            {
                if (RMb("Enable Loop Operation"))
                {
                    if (EditorUtility.DisplayDialog("Warning", "This action requires recompilation of the raymarcher shader. Nothing will be removed or lost. Are you sure to process this action?", "Yes", "No") == false)
                        return;
                    rm.SHADER_OPloopEnabled = true;
                    rm.MasterRewriteMapping(false);
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                RMproperty("SHADER_OPloopTilling", "Loop Tiling");
                RMhelpbox("Raymarcher objects should be close to the zero world-coordinates for the best results", MessageType.None);
                if (RMb("Disable Loop Operation"))
                {
                    if (EditorUtility.DisplayDialog("Warning", "This action requires recompilation of the raymarcher shader. Nothing will be removed or lost. Are you sure to process this action?", "Yes", "No") == false)
                        return;
                    rm.SHADER_OPloopEnabled = false;
                    rm.MasterRewriteMapping(false);
                    AssetDatabase.Refresh();
                }
            }
            RMbve();

            RMs(20);
			serializedObject.ApplyModifiedProperties();
		}
		
		private void GeneratePresets()
        {
            RMs(5);
            RMbv();
            RMl("Raymarcher Quality Presets");
            RMbh();
            if(RMb("High", 0, "Quality is more prioritized than performance. The render quality & render distance are set to high"))
            {
                if(rm.rmMobileMode)
                {
                    rm.SHADER_MASrenderMaxDistance = 20;
                    rm.SHADER_MASrenderQuality_E = RM_Master.SHADER_MASrenderQuality_E_.High;
                    rm.SHADER_MASrenderQuality = 0.001f;
                }
                else
                {
                    rm.SHADER_MASrenderMaxDistance = 50;
                    rm.SHADER_MASrenderQuality_E = RM_Master.SHADER_MASrenderQuality_E_.Extreme;
                    rm.SHADER_MASrenderQuality = 0.0001f;
                }
                rm.OnValidate();
            }
            if (RMb("Medium", 0, "Quality & performance are slightly balanced. The render quality & render distance are set to medium"))
            {
                if (rm.rmMobileMode)
                {
                    rm.SHADER_MASrenderMaxDistance = 10;
                    rm.SHADER_MASrenderQuality_E = RM_Master.SHADER_MASrenderQuality_E_.Medium;
                    rm.SHADER_MASrenderQuality = 0.015f;
                }
                else
                {
                    rm.SHADER_MASrenderMaxDistance = 25;
                    rm.SHADER_MASrenderQuality_E = RM_Master.SHADER_MASrenderQuality_E_.High;
                    rm.SHADER_MASrenderQuality = 0.001f;
                }
                rm.OnValidate();
            }
            if (RMb("Low", 0, "Performance is more prioritized than quality. The render quality & render distance are set to low"))
            {
                if (rm.rmMobileMode)
                {
                    rm.SHADER_MASrenderMaxDistance = 5;
                    rm.SHADER_MASrenderQuality_E = RM_Master.SHADER_MASrenderQuality_E_.Low;
                    rm.SHADER_MASrenderQuality = 0.12f;
                }
                else
                {
                    rm.SHADER_MASrenderMaxDistance = 10;
                    rm.SHADER_MASrenderQuality_E = RM_Master.SHADER_MASrenderQuality_E_.Medium;
                    rm.SHADER_MASrenderQuality = 0.01f;
                }
                rm.OnValidate();
            }
            RMbhe();
			 if(rm.rmMobileMode)
                if (RMb("Recommended", 0, "Set recommended presets for the currently selected platform"))
                {
                    if (EditorUtility.DisplayDialog("Warning", "You are about to change your current render and quality settings to the recommended level. Also the maximum object count will be changed which may cause some of the existing raymarcher objects to disappear. Are you sure to continue?", "Yes", "No"))
                    {
                        RM_MappingMaster mp = rm.GetComponent<RM_MappingMaster>();
                        if (!mp)
                        {
                            RMDEBUG.RMDebug(rm, "Mapping master component doesn't exist on RM_Master!", true);
                            return;
                        }
                        rm.SHADER_MASrenderMaxDistance = 5;
                        rm.SHADER_MASrenderQuality_E = RM_Master.SHADER_MASrenderQuality_E_.Medium;
                        rm.SHADER_MASrenderQuality = 0.012f;
                        mp.mapMaximumObjects = 3;
                        mp.MAPsetUpMappingMaster(false);
                        rm.OnValidate();
                    }
                }
            RMbve();
            RMs(5);
        }
	}
}