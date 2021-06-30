using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RaymarcherPackage
{
    /// <summary>
    /// Mapping Master is one of the essential components that manages all existing raymarching objects. The system refreshes their parameters and transforms.
    /// 
    /// Last Update: 01.06.2021 [dd/mm/yyyy].
    /// Written by Matej Vanco
    /// </summary>
    [ExecuteInEditMode]
    public class RM_MappingMaster : MonoBehaviour
    {
        public RM_Master mapRmMaster;

        [Range(1, 30)]
        public int mapMaximumObjects = 10;
        public int mapCurrentMaximumObjects = 10;
        public int mapCurrentObjectsCount = 0;

        public bool mapSetForTheCurrentScene = false;
        public string mapCurrentSceneName = "";

        [System.Serializable]
        public class MapObjectsContainer
        {
            public RM_Object mapVirtualObject;
        }

        public List<MapObjectsContainer> mapObjectsContainer = new List<MapObjectsContainer>();


        /// <summary>
        /// Create raymarcher object
        /// </summary>
        /// <returns>Returns gameObject with RM_Object component</returns>
#if UNITY_EDITOR
        [MenuItem("GameObject/3D Object/Raymarcher Object")]
#endif
        public static GameObject MAPcreateRaymarcherObject()
        {
            RM_Object newObject = new GameObject().AddComponent<RM_Object>();
            if (!newObject) return null;
            newObject.name = "RMObject";
#if UNITY_EDITOR
            Selection.activeGameObject = newObject.gameObject;
            Camera sceneCam = SceneView.lastActiveSceneView.camera;
            if (sceneCam) newObject.transform.position = sceneCam.transform.position + sceneCam.transform.forward * 8f;
#endif
            return newObject.gameObject;
        }

#if UNITY_EDITOR       

        /// <summary>
        /// Set up scene for raymarcher from scratch
        /// </summary>
        public void MAPsetUpMappingMaster(bool clearAllObjects = true)
        {
            if (Application.isPlaying)
            {
                RMDEBUG.RMDebug(this, "It's not possible to setup the raymarcher mapping environment while playing!");
                return;
            }

            this.Awake();

            if(clearAllObjects) mapCurrentObjectsCount = 0;

            if (mapMaximumObjects > 20)
            {
                if (EditorUtility.DisplayDialog("Warning", "You are about to create more than 20 raymarching entities. This may take few minutes (2-5) or more. Are you sure to process this action?", "Yes", "No") == false)
                    return;
            }
            //Just in case!
            if(mapMaximumObjects > 50)
            {
                mapMaximumObjects = 50;
                RMDEBUG.RMDebug(this, "The maximum objects count has been set to 50, because previous value was too high & could cause a serious performance drop.",true);
            }

            mapCurrentMaximumObjects = mapMaximumObjects;

            //---Create new possible array in container
            if (clearAllObjects)
            {            
                //---Clear the container first [if contains some elements]
                MAPclearAllObjects();
                for (int i = 0; i < mapMaximumObjects; i++)
                    mapObjectsContainer.Add(new MapObjectsContainer());
            }
            else
            {
                List<RM_Object> objs = new List<RM_Object>();
                foreach(var v in mapObjectsContainer)
                {
                    if (v.mapVirtualObject) objs.Add(v.mapVirtualObject);
                }
                mapObjectsContainer = new List<MapObjectsContainer>();
                for (int i = 0; i < mapMaximumObjects; i++)
                    mapObjectsContainer.Add(new MapObjectsContainer() {  mapVirtualObject = i <= objs.Count - 1 ? objs[i] : null });
            }

            RM_XConvertor conv = GetComponent<RM_XConvertor>();
            if(!conv)
            {
                RMDEBUG.RMDebug(this, "XConvertor doesn't exist! Please set up the raymarcher again or add XConvertor to the RM_Master component.",true);
                return;
            }
            conv.XXRecompileRaymarchingModelAlgo();
        }

        /// <summary>
        /// Create a new Master Pattern for the specific scene
        /// </summary>
        public void MAPcreateNewMasterPattern()
        {
            RM_XConvertor conv = GetComponent<RM_XConvertor>();
            if (!conv)
            {
                RMDEBUG.RMDebug(this, "XConvertor doesn't exist! Please set up the raymarcher again or add XConvertor to the RM_Master component.", true);
                return;
            }
            conv.XXCreateNewRaymarchMasterPattern(this, SceneManager.GetActiveScene().name, mapRmMaster.rmVRMode, mapRmMaster.rmMobileMode, mapRmMaster.rmWebGL);
        }

        /// <summary>
        /// Check the essential files like 'Master Pattern' and 'Object Buffer'
        /// </summary>
        /// <returns>Returns true if all required essential files exist</returns>
        public bool MAPcheckForEssentialInputOutput()
        {
            RM_XConvertor conv = GetComponent<RM_XConvertor>();
            if (!conv)
            {
                RMDEBUG.RMDebug(this, "XConvertor doesn't exist! Please set up the raymarcher again or add XConvertor to the RM_Master component.", true);
                return false;
            }
            return conv.XXCheckForSpecificPatterns(this, SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Full raymarcher setup process from scratch, clears all existing raymarcher objects and creates brand new raymarcher renderer
        ///(function specially made for Raymarcher initialization from editor)
        /// </summary>
        public void MAPsetUpFullRaymarcher()
        {
            MAPcreateNewMasterPattern();
            MAPsetUpMappingMaster();
            string sceneName = SceneManager.GetActiveScene().name;
            Material newMat = new Material(Shader.Find("Matej Vanco/RayMarcher/" + (mapRmMaster.rmMobileMode ? "Mobile/" : "PC/") + "RM Master_" + sceneName));
            mapRmMaster.rmMaterialSource = newMat;
            if (mapRmMaster.rmVRMode)
            {
                AssetDatabase.CreateAsset(newMat, "Assets/RM Master_" + sceneName + ".mat");
                GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
                screen.name = "Raymarch_Filter";
                DestroyImmediate(screen.GetComponent<Collider>());
                screen.GetComponent<Renderer>().sharedMaterial = newMat;
            }
            Light[] lights = Light.GetLights(LightType.Directional, 0);
            if (lights.Length == 0) return;
            mapRmMaster.rmDirectionalLightSource = lights[0].transform;
        }

        /// <summary>
        /// Opposite to setup - clear current raymarcher renderer with all files
        /// </summary>
        public void MAPclearFullRaymarcher()
        {
            RM_XConvertor conv = GetComponent<RM_XConvertor>();
            if (!conv)
            {
                RMDEBUG.RMDebug(this, "XConvertor doesn't exist! Please set up the raymarcher again or add XConvertor to the RM_Master component.", true);
                return;
            }
            MAPclearAllObjects();
            conv.XXRemoveCurrentRaymarchMasterPattern(this, SceneManager.GetActiveScene().name);
            DestroyImmediate(mapRmMaster);
            DestroyImmediate(conv);
            RMDEBUG.RMDebug(this, "Raymarcher has been successfully removed & scene is clear.");
            DestroyImmediate(this);
            AssetDatabase.Refresh();
        }

#endif

        /// <summary>
        /// Refresh raymarcher objects to master shader
        /// </summary>
        public void MAPrefreshContainer()
        {
            for (int i = 0; i < mapCurrentMaximumObjects; i++)
            {
                MapObjectsContainer obj = mapObjectsContainer[i];
                if (obj.mapVirtualObject == null)
                {
                    if (mapRmMaster.rmMobileMode)
                    {
                        mapRmMaster.rmMaterialSource.SetVector("params" + i.ToString(), new Vector4(-1, -1, -1, 0));
                        continue;
                    }
                    mapRmMaster.rmMaterialSource.SetFloat("enabled" + i.ToString(), 0);
                    Matrix4x4 Mattt = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, Vector3.one * 0.01f);
                    mapRmMaster.rmMaterialSource.SetMatrix("_obj" + i.ToString() + "Model", Mattt.inverse);
                    mapRmMaster.rmMaterialSource.SetFloat("fragEnabled" + i.ToString(), 0);
                    continue;
                }

                if(mapRmMaster.rmMobileMode)
                {
                    RM_Object rmobjmob = obj.mapVirtualObject.GetComponent<RM_Object>();
                    rmobjmob.rmMyBornName = "obj" + i.ToString();

                    int a = rmobjmob.gameObject.activeInHierarchy ? 1 : 0;
                    mapRmMaster.rmMaterialSource.SetVector("_obj" + i.ToString() + "Model", new Vector4(rmobjmob.transform.position.x, rmobjmob.transform.position.y, rmobjmob.transform.position.z, rmobjmob.rmColorVal));
                    mapRmMaster.rmMaterialSource.SetVector("params" + i.ToString(), new Vector4(rmobjmob.rmParamA, rmobjmob.rmParamB, rmobjmob.rmParamC, rmobjmob.rmOPFragmentPar_Evolution) * a);
                    continue;
                }

                int aaa = obj.mapVirtualObject.gameObject.activeInHierarchy ? 1 : 0;
                mapRmMaster.rmMaterialSource.SetFloat("enabled" + i.ToString(), aaa);

                float add = aaa == 0 ? 0.01f : 1;
                RM_Object rmobj = obj.mapVirtualObject.GetComponent<RM_Object>();
                Matrix4x4 Mat = Matrix4x4.TRS(obj.mapVirtualObject.transform.position, obj.mapVirtualObject.transform.rotation, obj.mapVirtualObject.transform.localScale * add);
                mapRmMaster.rmMaterialSource.SetMatrix("_obj" + i.ToString() + "Model", Mat.inverse);

                rmobj.rmMyBornName = "obj" + i.ToString();

                mapRmMaster.rmMaterialSource.SetVector("size" + i.ToString(), new Vector4(Mathf.Abs(rmobj.rmParamA), Mathf.Abs(rmobj.rmParamB), Mathf.Abs(rmobj.rmParamC), rmobj.rmParamD));

                mapRmMaster.rmMaterialSource.SetFloat("obj" + i.ToString() + "sm", Mathf.Abs(rmobj.rmShapeSmoothness));

                mapRmMaster.rmMaterialSource.SetColor("color" + i.ToString(), rmobj.rmColor);

                mapRmMaster.rmMaterialSource.SetFloat("fragEnabled" + i.ToString(), rmobj.rmOPFragment ? 1 : 0);

                if (rmobj.rmOPFragment)
                {
                    mapRmMaster.rmMaterialSource.SetVector("fragSize" + i.ToString(),
                    new Vector4(
                    rmobj.rmOPFragmentPar_Size.x,
                    rmobj.rmOPFragmentPar_Size.y,
                    rmobj.rmOPFragmentPar_Size.z,
                    rmobj.rmOPFragmentPar_Evolution));

                    mapRmMaster.rmMaterialSource.SetVector("DirfragSize" + i.ToString(),
                    new Vector4(
                    rmobj.rmOPFragmentPar_AnimDirection.x,
                    rmobj.rmOPFragmentPar_AnimDirection.y,
                    rmobj.rmOPFragmentPar_AnimDirection.z,
                    0));
                }
            }
        }

        /// <summary>
        /// Add new raymarching object manually to the Mapping Master database to handle it's transform & params
        /// </summary>
        public void MAPaddObject(RM_Object sender)
        {
            MAPrefreshRaymarcherScene();
            if (mapCurrentObjectsCount > mapCurrentMaximumObjects) return;
            int selIndex = -1;
            bool exists = false;
            for (int i = 0; i < mapCurrentMaximumObjects; i++)
            {
                if (mapObjectsContainer[i].mapVirtualObject == null && selIndex == -1)
                    selIndex = i;
                if(!exists && mapObjectsContainer[i].mapVirtualObject != null)
                    exists = mapObjectsContainer[i].mapVirtualObject == sender;
            }
            if (exists) return;
            if (selIndex < 0) return;
            mapObjectsContainer[selIndex].mapVirtualObject = sender;
        }

        /// <summary>
        /// Clear all raymarcher objects manually. If included parameter is true, the whole list will be cleared (otherwise the list's values will be cleared and list count will remain)
        /// By clearing the list [param's true] = the shader recompilation is required.
        /// </summary>
        public void MAPclearAllObjects(bool includeContainerElement = true)
        {
            if (mapObjectsContainer.Count == 0)
                return;

            for (int i = mapObjectsContainer.Count - 1; i >= 0; i--)
            {
                if (mapObjectsContainer[i].mapVirtualObject)
                    DestroyImmediate(mapObjectsContainer[i].mapVirtualObject.gameObject);
                if(includeContainerElement)
                    mapObjectsContainer.RemoveAt(i);
            }
            if(includeContainerElement)
                mapObjectsContainer.Clear();
        }

        /// <summary>
        /// Refresh current raymarching scene and sum-up all available RM Objects
        /// </summary>
        public void MAPrefreshRaymarcherScene()
        {
            mapCurrentObjectsCount = FindObjectsOfType(typeof(RM_Object)).Length;
            if (mapCurrentObjectsCount > mapCurrentMaximumObjects)
                RMDEBUG.RMDebug(this, "Maximum raymarcher objects reached! [MAX SET:" + mapCurrentMaximumObjects + "]");
            mapCurrentObjectsCount = (mapCurrentObjectsCount > mapCurrentMaximumObjects) ? mapCurrentMaximumObjects : mapCurrentObjectsCount;
        }

        private void Awake()
        {
            if (!mapRmMaster)
                mapRmMaster = GetComponent<RM_Master>();
            if(!mapRmMaster)
                RMDEBUG.RMDebug(this, "RM_Master doesn't exist! Please set up the raymarcher again.", true);

            if (string.IsNullOrEmpty(mapCurrentSceneName))
                mapSetForTheCurrentScene = false;
#if UNITY_EDITOR
            else if (SceneManager.GetActiveScene().name != mapCurrentSceneName)
                mapSetForTheCurrentScene = false;

            if (MAPcheckForEssentialInputOutput() == false)
                mapSetForTheCurrentScene = false;
#endif
        }

        private void Update()
        {
            if (mapObjectsContainer.Count > 0) MAPrefreshContainer();
        }
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(RM_MappingMaster))]
    [CanEditMultipleObjects]
    public class RM_MappingMasterEditor : RM_InternalEditor
    {
        private RM_MappingMaster rm;

        private void OnEnable()
        {
            rm = target as RM_MappingMaster;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!rm.mapSetForTheCurrentScene)
            {
                RMs(15);
                RMhelpbox("The current raymarcher renderer is not yet set for the current scene. Please create a new 'Master Pattern' to set the Raymarcher renderer for the current scene.");
                if (RMb("Create a new Master Pattern"))
                    rm.MAPsetUpFullRaymarcher();

                serializedObject.ApplyModifiedProperties();
                return;
            }
            else
            {
                RMs(15);
                RMhelpbox("The current raymarcher renderer is set for '" + rm.mapCurrentSceneName + "' scene.", MessageType.None);
            }

            RMs(5);

            if (RMb("Clear & Set Up Scene"))
            {
                if (EditorUtility.DisplayDialog("Warning", "This process will clear your current raymarcher mapping and it's environment. Are you sure to process this action?", "Yes", "No") == false)
                    return;
                rm.MAPsetUpMappingMaster();
                return;
            }
            if (RMb("Change Maximum Objects Only"))
            {
                if (EditorUtility.DisplayDialog("Warning", "This process will just change the maximum raymarching objects only. Nothing will be removed. Are you sure to process this action?", "Yes", "No") == false)
                    return;
                rm.MAPsetUpMappingMaster(false);
                return;
            }
            RMbh();
            if (RMb("Recompile Shader", 150))
            {
                if (EditorUtility.DisplayDialog("Warning", "Would you like to process this action? The raymarcher will re-write and re-compile the whole shader which may take a few seconds or even minutes (depending on the raymarcher objects count)", "Yes", "No"))
                {
                    rm.MAPsetUpMappingMaster(false);
                    AssetDatabase.Refresh();
                    return;
                }
            }
            if (RMb("Get Objects From Scene", 300))
            {
                if (EditorUtility.DisplayDialog("Warning", "This process will clear all your current raymarching objects and will find any existing raymarching objects in the scene. Would you like to continue?", "Yes", "No"))
                {
                    rm.MAPsetUpMappingMaster(true);
                    foreach(RM_Object rmo in FindObjectsOfType(typeof(RM_Object)))
                    {
                        if (rm.mapCurrentObjectsCount >= rm.mapCurrentMaximumObjects)
                            break;
                        rm.MAPaddObject(rmo);
                    }
                    return;
                }
            }
            RMbhe();

            RMs(5);

            RMbv();
            RMproperty("mapMaximumObjects", "Maximum Possible Objects");
            if (rm.mapMaximumObjects > 10 && (rm.mapRmMaster.rmMobileMode || (rm.mapRmMaster.rmMobileMode && rm.mapRmMaster.rmVRMode)))
                RMhelpbox("It's recommended to use max 10 objects for mobiles", MessageType.None);
            if(rm.mapMaximumObjects != rm.mapCurrentMaximumObjects)
                RMhelpbox("Maximum object count has been changed. Press 'Change Maximum Objects Only' to apply the changes");
            RMbv();
            RMl("Max Objects Count Set To <b>" + rm.mapCurrentMaximumObjects.ToString()+"</b>");

            RMl("Current Objects Count: " + rm.mapCurrentObjectsCount.ToString());
            RMbve();
            RMbve();
            Color cache = GUI.color;
            GUI.color = (Color.red * 1.2f);
            if (RMb("Clear & Remove Raymarcher"))
            {
                if (EditorUtility.DisplayDialog("Warning", "This process will clear your current raymarcher renderer for the current scene and all its source files. Are you sure to process this action? There's no undo.", "Yes", "No") == false)
                    return;
                rm.MAPclearFullRaymarcher();
                return;
            }
            GUI.color = cache;
            RMs();
            RMlvlPlus(1);
            RMpropertyList("mapObjectsContainer", "Object Container [DEBUG]","Currently-possible object container for debug purposes (please do not change anything)");
            RMlvlMinus(1);

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}