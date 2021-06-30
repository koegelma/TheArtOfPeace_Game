using UnityEngine;
using UnityEngine.UI;

namespace RaymarcherPackage
{
    /// <summary>
    /// Raymarch Master is the general and main component of the package. The master manages rendering, visualization, shading and operations.
    /// The component is highly required for other components to make the Raymarcher work.
    /// 
    /// Last Update: 01.06.2021 [dd/mm/yyyy].
    /// Written by Matej Vanco
    /// </summary>
    [ExecuteInEditMode]
    public class RM_Master : RMInternal_SceneFilter
    {
        public Transform rmDirectionalLightSource;
        public Material rmMaterialSource;

        public static bool rmEnableDebugMessages = true;

        public bool rmVRMode = false;
        public bool rmMobileMode = false;
        public bool rmWebGL = false;

        private Camera rmCamCache;

        private void OnEnable()
        {
            rmCamCache = GetComponent<Camera>();
            if (!rmCamCache)
                RMDEBUG.RMDebug(this, "The RM_Master can't find any camera on its object!", true);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!rmMaterialSource)
                return;
            if (!rmCamCache)
            {
                RMDEBUG.RMDebug(this, "RM_Master is missing a camera property");
                return;
            }
            Graphics.Blit(source, destination);

            rmMaterialSource.SetMatrix("_CamFrustum", Internal_CamFrustum(rmCamCache));
            rmMaterialSource.SetMatrix("_CamToWorld", rmCamCache.cameraToWorldMatrix);
            rmMaterialSource.SetVector(rmMobileMode ? "_MASrenderLightDir" : "_LIGHTdirect", rmDirectionalLightSource ? rmDirectionalLightSource.forward : Vector3.down);
            rmMaterialSource.SetVector("_CamWorldSpace", rmCamCache.transform.position);

            RenderTexture.active = destination;
            rmMaterialSource.SetTexture("_MASrenderMainFilter", source);
            rmMaterialSource.SetPass(0);

            if (!rmVRMode)
             {
                 GL.PushMatrix();
                 GL.LoadOrtho();

                 GL.Begin(GL.QUADS);

                 GL.MultiTexCoord2(0, 0.0f, 0.0f);
                 GL.Vertex3(0.0f, 0.0f, 3.0f);

                 GL.MultiTexCoord2(0, 1.0f, 0.0f);
                 GL.Vertex3(1.0f, 0.0f, 2.0f);

                 GL.MultiTexCoord2(0, 1.0f, 1.0f);
                 GL.Vertex3(1.0f, 1.0f, 1.0f);

                 GL.MultiTexCoord2(0, 0.0f, 1.0f);
                 GL.Vertex3(0.0f, 1.0f, 0.0f);
                 GL.End();
                 GL.PopMatrix();
             }
             
            Internal_RefreshMaterialSource();
        }
        
        private void Update()
        {
            if (!rmVRMode)
                return;
            if (!rmMaterialSource)
                return;
            if (!rmCamCache)
            {
                RMDEBUG.RMDebug(this, "RM_Master is missing a camera property");
                return;
            }

            rmMaterialSource.SetMatrix("_CamToWorld", rmCamCache.cameraToWorldMatrix);
            rmMaterialSource.SetVector(rmMobileMode ? "_MASrenderLightDir" : "_LIGHTdirect", rmDirectionalLightSource ? rmDirectionalLightSource.forward : Vector3.down);
            rmMaterialSource.SetVector("_CamWorldSpace", rmCamCache.transform.position);

            Internal_RefreshMaterialSource();
        }

        #region SHADER VARIABLES

        //----------RENDER SECTION-------------------------
        //---RENDER : Textures & Colors
        public Color SHADER_MASrenderMainColor = new Color(1, 1, 1, 1);
        public Color SHADER_MASrenderSecondColor = new Color(1, 0.5f, 0, 1);
        [Range(1,3)]        public float SHADER_MASrenderMainEmission = 1.0f;
        [Range(0,1.0f)]     public float SHADER_MASrenderSpecularA = 0.01f;
        [Range(0.5f, 45.0f)]public float SHADER_MASrenderSpecularB = 42.0f;
        [Range(0, 2)]       public float SHADER_MASrenderSpecularIntens = 0.5f;
        public Texture2D SHADER_MASrenderPrimaryTexture;
        public float SHADER_MASrenderPrimaryTextureTile = 1;

        public bool SHADER_MASfog = false;
        [Range(0.0f,2.0f)]  public float SHADER_MASfogDensity = 0.25f;
        public Color SHADER_MASfogColor = Color.gray;

        //---RENDER : Quality & Render Settings
        public float SHADER_MASrenderMaxDistance = 25;
        public bool SHADER_MASrenderQualitySetting = true;
        [Range(0.5f, 0.000001f)] public float SHADER_MASrenderQuality = 0.002f;
        public enum SHADER_MASrenderQuality_E_ { VeryLow, Low, Medium, High, VeryHigh, Extreme, Maximum, MaximumDouble };
        public SHADER_MASrenderQuality_E_ SHADER_MASrenderQuality_E = SHADER_MASrenderQuality_E_.VeryHigh;

        //---RENDER : Rendering Options [Toon Shading, Fresnel, Global smoothness]
        public bool inSHADER_Render_Lambert = true;
        public bool inSHADER_Render_Flat = false;
        public bool inSHADER_Render_Toon = false;
        public bool inSHADER_Render_Outline = false;

        [Range(0.0f, 1.0f)] public float SHADER_MASrenderToonThresh = 0.05f;
        [Range(0.0f, 1.0f)] public float SHADER_MASrenderToonDens = 0.125f;

        [Range(0.001f, 1.0f)] public float SHADER_MASrenderFresnel = 1f;
        public float SHADER_MASrenderFresnelMultiplier = 1;

        public float SHADER_MASrenderSmoothness = 0.2f;
        public float SHADER_MASrenderColorSmoothness = 0.8f;

        //----------SHADING SECTION-------------------------
        //---SHADING : Lighting
        public float SHADER_LIGHTintens = 0.5f;
        [Range(0.001f, 0.5f)] public float SHADER_LIGHTjitter = 0.01f;

        //---SHADING : Shadows
        public bool SHADER_SHADEenabled = true;
        public Vector2 SHADER_SHADEdistance = new Vector2(0.01f, 15);
        public float SHADER_SHADEintens = 1;
        public bool SHADER_SHADEsoft = true;
        [Range(1.0f, 25.0f)] public float SHADER_SHADEsoftness = 7;

        //---SHADING : Reflection
        public bool SHADER_REFLECTenabled = true;
        public float SHADER_REFLECTintensity = 0.6f;
        public bool SHADER_REFLECTcubemapEnabled = true;
        public Cubemap SHADER_REFLECTcubemap;
        public bool SHADER_REFLECTphysX = false;
        [Range(0.0f, 1.0f)] public float SHADER_REFLECTphysXemiss = 0;
        [Range(0.0f, 10.0f)] public float SHADER_REFLECTphysXintens = 1;
        [Range(1, 10)] public int SHADER_REFLECTphysXSampleCount = 1;

        //----------GLOBAL OPERATIONS SECTION-------------------------
        //---GLOBAL OPERATIONS : Loop
        public bool SHADER_OPloopEnabled = false;
        public Vector3 SHADER_OPloopTilling = new Vector3(10, 10, 10);


        //ADD_MOBILE
        public Texture2D SHADER_MAScolorRamp;

        [Range(0, 2.0f)] public float SHADER_MASrenderSpecularIntensMob = 0.5f;
        [Range(0.0f, 9.99f)] public float SHADER_MASrenderSpecularSize = 1.0f;

        [Range(-45.0f, 45.0f)] public float SHADER_MASrenderOutlineSoft = 2.0f;
        [Range(0.1f, 5.0f)] public float SHADER_MASrenderOutlineDens = 3.0f;

        [Range(0.0f,1.0f)] public float SHADER_MASrenderShadowUmbraIntens = 0.1f;

        public Vector3 SHADER_OPfragmentSize = new Vector3(0, 0, 0);
        public Vector3 SHADER_OPfragmentDirect = Vector3.one;

        #endregion

#if UNITY_EDITOR

        /// <summary>
        /// Recompile & rewrite current mapping master of the raymacher
        /// </summary>
        /// <param name="clearAllObjects">Remove all raymarcher objects in the scene?</param>
        public void MasterRewriteMapping(bool clearAllObjects = true)
        {
            RM_MappingMaster mp = GetComponent<RM_MappingMaster>();
            if (!mp)
            {
                RMDEBUG.RMDebug(this, "Mapping master component doesn't exist on RM_Master!", true);
                return;
            }
            mp.MAPsetUpMappingMaster(clearAllObjects);
        }

        /// <summary>
        /// Recompile & rewrite current master shader of the raymarcher
        /// </summary>
        /// <param name="currSceneName">Actual scene name, must exists in the project [saved]</param>
        public void MasterRecompileMaster(string currSceneName)
        {
            RM_MappingMaster mp = GetComponent<RM_MappingMaster>();
            RM_XConvertor xc = GetComponent<RM_XConvertor>();
            if (!mp)
            {
                RMDEBUG.RMDebug(this, "Mapping master component doesn't exist on RM_Master!", true);
                return;
            }
            if (!xc)
            {
                RMDEBUG.RMDebug(this, "XConvertor component doesn't exist on RM_Master!", true);
                return;
            }
            xc.XXRefreshMasterShader(mp, currSceneName, rmVRMode, rmMobileMode, rmWebGL);
        }

#endif

        #region Public Functions

        public void MASrenderDistance(Slider v)
        {
            SHADER_MASrenderMaxDistance = v.value;
        }
        public void MASrenderDistance(float v)
        {
            SHADER_MASrenderMaxDistance = v;
        }

        public void MASrenderQuality(Slider v)
        {
            SHADER_MASrenderQuality = v.value;
        }
        public void MASrenderQuality(int v)
        {
            SHADER_MASrenderQuality_E = (SHADER_MASrenderQuality_E_)v;
        }
        public void MASrenderQuality(float v)
        {
            SHADER_MASrenderQuality = v;
        }

        public void MASchangeGlobalSmoothness(Slider v)
        {
            SHADER_MASrenderSmoothness = v.value;
        }
        public void MASchangeGlobalSmoothness(float v)
        {
            SHADER_MASrenderSmoothness = v;
        }

        #endregion

        public void Internal_RefreshMaterialSource()
        {
            if (!rmMaterialSource)
                return;

            rmMaterialSource.SetColor("_MASrenderMainColor", SHADER_MASrenderMainColor);
            rmMaterialSource.SetColor("_MASrenderSecondColor", SHADER_MASrenderSecondColor);

            if (!rmMobileMode)
                Internal_RefreshMaterialSrcPC();
            else
                Internal_RefreshMaterialSrcMobile();

            //---RENDER : Quality & Render Settings
            rmMaterialSource.SetFloat("_MASrenderMaxDistance", SHADER_MASrenderMaxDistance);
            int intSHADER_MASrenderQualitySetting = SHADER_MASrenderQualitySetting ? 1 : 0;
            rmMaterialSource.SetInt("_MASrenderQualitySetting", intSHADER_MASrenderQualitySetting);
            rmMaterialSource.SetFloat("_MASrenderQuality", SHADER_MASrenderQuality);

            rmMaterialSource.SetFloat("_MASfogDensity", !SHADER_MASfog ? 0f : SHADER_MASfogDensity);
            rmMaterialSource.SetColor("_MASfogColor", SHADER_MASfogColor);

            if (SHADER_MASrenderQualitySetting)
            {
                switch (SHADER_MASrenderQuality_E)
                {
                    case SHADER_MASrenderQuality_E_.VeryLow:
                        SHADER_MASrenderQuality = 0.4f;
                        break;
                    case SHADER_MASrenderQuality_E_.Low:
                        SHADER_MASrenderQuality = 0.1f;
                        break;
                    case SHADER_MASrenderQuality_E_.Medium:
                        SHADER_MASrenderQuality = 0.01f;
                        break;
                    case SHADER_MASrenderQuality_E_.High:
                        SHADER_MASrenderQuality = 0.001f;
                        break;
                    case SHADER_MASrenderQuality_E_.VeryHigh:
                        SHADER_MASrenderQuality = 0.0005f;
                        break;
                    case SHADER_MASrenderQuality_E_.Extreme:
                        SHADER_MASrenderQuality = 0.0001f;
                        break;
                    case SHADER_MASrenderQuality_E_.Maximum:
                        SHADER_MASrenderQuality = 0.00001f;
                        break;
                    case SHADER_MASrenderQuality_E_.MaximumDouble:
                        SHADER_MASrenderQuality = 0.000001f;
                        break;
                }
            }

            //---RENDER : Rendering Options [Shading type, Fresnel, Global smoothness]
            rmMaterialSource.SetFloat("_MASrenderToonThresh", SHADER_MASrenderToonThresh);
            rmMaterialSource.SetFloat("_MASrenderToonDens", SHADER_MASrenderToonDens);

            rmMaterialSource.SetFloat("_MASrenderSmoothness", Mathf.Abs(SHADER_MASrenderSmoothness));
            rmMaterialSource.SetFloat("_MASrenderColorSmoothness", Mathf.Abs(SHADER_MASrenderColorSmoothness));

            //----------GLOBAL OPERATIONS SECTION-------------------------
            //---GLOBAL OPERATIONS : Loop
            int intSHADER_OPloopEnabled = SHADER_OPloopEnabled ? 1 : 0;
            rmMaterialSource.SetInt("_OPloopEnabled", intSHADER_OPloopEnabled);
            rmMaterialSource.SetVector("_OPloopTilling", SHADER_OPloopTilling);
        }

        private void Internal_RefreshMaterialSrcPC()
        {
            rmMaterialSource.SetTexture("_MASrenderPrimaryTexture", SHADER_MASrenderPrimaryTexture);
            rmMaterialSource.SetFloat("_MASrenderPrimaryTextureTile", SHADER_MASrenderPrimaryTextureTile);

            rmMaterialSource.SetFloat("_MASrenderMainEmission", SHADER_MASrenderMainEmission);

            rmMaterialSource.SetFloat("_MASrenderSpecularA", SHADER_MASrenderSpecularA);
            rmMaterialSource.SetFloat("_MASrenderSpecularB", SHADER_MASrenderSpecularB);
            rmMaterialSource.SetFloat("_MASrenderSpecularIntens", SHADER_MASrenderSpecularIntens);


            rmMaterialSource.SetFloat("_MASrenderFresnel", SHADER_MASrenderFresnel);
            rmMaterialSource.SetFloat("_MASrenderFresnelMultiplier", SHADER_MASrenderFresnelMultiplier);

            //----------SHADING SECTION-------------------------
            //---SHADING : Lighting
            rmMaterialSource.SetFloat("_LIGHTintens", SHADER_LIGHTintens);
            rmMaterialSource.SetFloat("_LIGHTjitter", SHADER_LIGHTjitter);

            //---SHADING : Shadows
            int intSHADER_SHADEenabled = SHADER_SHADEenabled ? 1 : 0;
            rmMaterialSource.SetInt("_SHADEenabled", intSHADER_SHADEenabled);
            rmMaterialSource.SetVector("_SHADEdistance", SHADER_SHADEdistance);
            rmMaterialSource.SetFloat("_SHADEintens", SHADER_SHADEintens);
            int intSHADER_SHADEsoft = SHADER_SHADEsoft ? 1 : 0;
            rmMaterialSource.SetInt("_SHADEsoft", intSHADER_SHADEsoft);
            rmMaterialSource.SetFloat("_SHADEsoftness", SHADER_SHADEsoftness);

            //---SHADING : Reflection
            int intSHADER_REFLECTenabled = SHADER_REFLECTenabled ? 1 : 0;
            rmMaterialSource.SetInt("_REFLECTenabled", intSHADER_REFLECTenabled);
            rmMaterialSource.SetFloat("_REFLECTintensity", SHADER_REFLECTintensity);
            int intSHADER_REFLECTcubemapEnabled = SHADER_REFLECTcubemapEnabled ? 1 : 0;
            rmMaterialSource.SetInt("_REFLECTcubemapEnabled", intSHADER_REFLECTcubemapEnabled);
            rmMaterialSource.SetTexture("_REFLECTcubemap", SHADER_REFLECTcubemap);
            int intSHADER_REFLECTphysX = SHADER_REFLECTphysX ? 1 : 0;
            rmMaterialSource.SetInt("_REFLECTphysX", intSHADER_REFLECTphysX);
            rmMaterialSource.SetFloat("_REFLECTphysXemiss", SHADER_REFLECTphysXemiss);
            rmMaterialSource.SetFloat("_REFLECTphysXintens", SHADER_REFLECTphysXintens);
            rmMaterialSource.SetFloat("_REFLECTphysXSampleCount", SHADER_REFLECTphysXSampleCount);
        }

        private void Internal_RefreshMaterialSrcMobile()
        {
            rmMaterialSource.SetTexture("_MAScolorRamp", SHADER_MAScolorRamp);

            rmMaterialSource.SetFloat("_MASrenderSpecularIntens", SHADER_MASrenderSpecularIntensMob);
            rmMaterialSource.SetFloat("_MASrenderSpecularSize", SHADER_MASrenderSpecularSize);

            rmMaterialSource.SetFloat("_MASrenderOutlineSoft", SHADER_MASrenderOutlineSoft);
            rmMaterialSource.SetFloat("_MASrenderOutlineDens", SHADER_MASrenderOutlineDens);

            rmMaterialSource.SetFloat("_MASrenderShadowUmbraIntens", SHADER_MASrenderShadowUmbraIntens);

            rmMaterialSource.SetVector("_OPfragmentSize", SHADER_OPfragmentSize);
            rmMaterialSource.SetVector("_OPfragmentDirect", SHADER_OPfragmentDirect);
        }

        private Matrix4x4 Internal_CamFrustum(Camera cam)
        {
            Matrix4x4 frustum = Matrix4x4.identity;
            float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

            Vector3 ud = Vector3.up * fov;
            Vector3 rd = Vector3.right * fov * cam.aspect;

            Vector3 ldf = (-Vector3.forward - rd + ud);
            Vector3 rdf = (-Vector3.forward + rd + ud);
            Vector3 rd2 = (-Vector3.forward + rd - ud);
            Vector3 ld2 = (-Vector3.forward - rd - ud);

            frustum.SetRow(0, ldf);
            frustum.SetRow(1, rdf);
            frustum.SetRow(2, rd2);
            frustum.SetRow(3, ld2);

            return frustum;
        }
    }
}