using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RaymarcherPackage
{
    /// <summary>
    /// RM Object is an essential component for each raymarching object. Contains 'morpher' functionality, custom shape generator, fractals and additional parameters.
    /// Each raymarching object has a unique ID and 'Born Name' which are defined right after their creation.
    /// 
    /// Last Update: 01.06.2021 [dd/mm/yyyy].
    /// Written by Matej Vanco
    /// </summary>
	[ExecuteInEditMode]
    public class RM_Object : MonoBehaviour
	{
        public bool rmIsFractal = false;

        public enum RMFractalType : int { Apollonian, Kleinian, Mandelbulb, Tetrahedron };
        public RMFractalType rmFractalType;

        public float rmShapeSmoothness = 0.2f;

        public float rmParamA = 1.0f;
		public float rmParamB = 1.0f;
		public float rmParamC = 1.0f;
        public float rmParamD = 1.0f;
        
		public Color rmColor = Color.white;
        [Range(0.0f,1.0f)] public float rmColorVal = 0.0f;

        //Internal Operation
        public bool rmOPFragment = false;
		public Vector3 rmOPFragmentPar_Size = new Vector3(3,3,3);
		public float rmOPFragmentPar_Evolution = 0.35f;
        public bool rmOPFragmentPar_EnableAnim = false;
        public Vector3 rmOPFragmentPar_AnimDirection = Vector3.one;
        //---------------------

        //External Operation
        public enum RMExternalOperation : int { None, Subtract, Intersect};
        public RMExternalOperation rmExternalOperation = RMExternalOperation.None;

		public RM_Object rmOPOperator_Target;
        public bool rmOPBeingOperated;
        //---------------------

		public string rmMyBornName;

        public RM_MappingMaster rmMappingMaster;
        [SerializeField] private int bornID = 0;

        //Editor related
        public bool eHist_WasFractal;
        public int eHist_FractalType;
        public int eHist_WasOperation;
        public RM_Object eHist_OPTarget;
        //----------------------

        public void Awake()
        {
            if (!rmMappingMaster)
                rmMappingMaster = (RM_MappingMaster)FindObjectOfType(typeof(RM_MappingMaster));
            if (!rmMappingMaster)
            {
                this.gameObject.name = "RMObject";
                RMDEBUG.RMDebug(this, "Mapping Master doesn't exist in the current scene! Press 'Set Up Raymarcher' in Window/Raymarcher", true);
                DestroyImmediate(this);
                return;
            }

            if (bornID != GetInstanceID())
            {
                if (bornID == 0)
                {
                    bornID = GetInstanceID();
                    rmMappingMaster.MAPaddObject(this);
                    SetupMyself();
                }
                else
                {
                    bornID = GetInstanceID();
                    if (bornID < 0)
                    {
                        rmMappingMaster.MAPaddObject(this);
                        SetupMyself();
                    }
                }
            }
            else
            { 
                rmMappingMaster.MAPaddObject(this);
                SetupMyself();
            }
        }

        /// <summary>
        /// Setup this object - editor setting helper
        /// </summary>
        internal void SetupMyself()
        {
            eHist_WasFractal = rmIsFractal;
            eHist_FractalType = (int)rmFractalType;
            eHist_WasOperation = (int)rmExternalOperation;
            eHist_OPTarget = rmOPOperator_Target;
        }

        private void OnDrawGizmosSelected()
        {
            if (rmIsFractal)
                return;
            if (rmExternalOperation != RMExternalOperation.Subtract)
                return;
            Gizmos.DrawWireSphere(transform.position, rmParamA);
        }

        private void OnDestroy()
        {
            if (rmMappingMaster)
                rmMappingMaster.MAPrefreshRaymarcherScene();
        }

        /// <summary>
        /// Reset parameters to preset values
        /// </summary>
        public void ResetToDefaultParams(int preset = 0, bool ignoreIfNotFractal = false)
        {
            if (!rmIsFractal && !ignoreIfNotFractal) return;

            switch (preset)
            {
                case 0:
                    rmParamA = 1.5f;
                    rmParamB = 1.0f;
                    rmParamC = 1.0f;
                    break;
                case 1:
                    rmParamA = 1.0f;
                    rmParamB = 1.0f;
                    rmParamC = 1.0f;
                    break;
                case 2:
                    rmParamA = 15f;
                    rmParamB = 15f;
                    rmParamC = 0.9f;
                    break;
                case 3:
                    rmParamA = 1.7f;
                    rmParamB = 1.5f;
                    rmParamC = 1.0f;
                    break;

                default:
                    rmParamA = 1.5f;
                    rmParamB = 1.0f;
                    rmParamC = 1.0f;
                    break;
            }
        }

        #region Public Available Events

        public void ObjchangeParamA(Slider v)
        {
            rmParamA = v.value;
        }
        public void ObjchangeParamA(float v)
        {
            rmParamA = v;
        }
        public void ObjchangeParamB(Slider v)
        {
            rmParamB = v.value;
        }
        public void ObjchangeParamB(float v)
        {
            rmParamB = v;
        }
        public void ObjchangeParamC(Slider v)
        {
            rmParamC = v.value;
        }
        public void ObjchangeParamC(float v)
        {
            rmParamC = v;
        }
        public void ObjchangeParamD(Slider v)
        {
            rmParamD = v.value;
        }
        public void ObjchangeParamD(float v)
        {
            rmParamD = v;
        }

        public void ObjchangeSmoothness(Slider v)
        {
            rmShapeSmoothness = v.value;
        }
        public void ObjchangeSmoothness(float v)
        {
            rmShapeSmoothness = v;
        }

        public void ObjOP_Fragment(bool v)
        {
            rmOPFragment = v;
        }
        public void ObjOP_Fragment(Toggle v)
        {
            rmOPFragment = v.isOn;
        }

        public void ObjOP_changeFragmentSize(Slider v)
        {
            rmOPFragmentPar_Size = Vector3.one * v.value;
        }
        public void ObjOP_changeFragmentSize(float v)
        {
            rmOPFragmentPar_Size = Vector3.one * v;
        }
        public void ObjOP_changeFragmentEvol(Slider v)
        {
            rmOPFragmentPar_Evolution = v.value;
        }
        public void ObjOP_changeFragmentEvol(float v)
        {
            rmOPFragmentPar_Evolution = v;
        }

        public void ObjOP_FragmentAnim(bool v)
        {
            rmOPFragmentPar_EnableAnim =v;
        }
        public void ObjOP_FragmentAnim(Toggle v)
        {
            rmOPFragmentPar_EnableAnim = v.isOn;
        }

        public void ObjOP_changeFragmentDir(Slider v)
        {
            rmOPFragmentPar_AnimDirection = Vector3.one * v.value;
        }
        public void ObjOP_changeFragmentDir(float v)
        {
            rmOPFragmentPar_AnimDirection = Vector3.one * v;
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(RM_Object))]
	[CanEditMultipleObjects]
	public class RM_ObjectEditor : RM_InternalEditor
	{
		private RM_Object rm;

        private void OnEnable()
		{
			rm = target as RM_Object;
			rm.Awake();
		}

        private RM_Object.RMFractalType ErmFractalType;

        public override void OnInspectorGUI()
		{
			serializedObject.Update();
            RMs(20);
            RMl("Born Name: " + rm.rmMyBornName);
            RMs(5);
            RMproperty("rmIsFractal", "Is Fractal");
            RMbv();
            if (rm.rmIsFractal)
                ErmFractalType = (RM_Object.RMFractalType)EditorGUILayout.EnumPopup("Fractal Type", rm.rmFractalType);
            if (!rm.rmMappingMaster.mapRmMaster.rmMobileMode)
                RMproperty("rmColor", "Color");
            else
            {
                RMbh();
                if (rm.rmMappingMaster.mapRmMaster.SHADER_MAScolorRamp)
                    RMt(rm.rmMappingMaster.mapRmMaster.SHADER_MAScolorRamp);
                else
                    RMl("No color-ramp texture available");
                RMbhe();
                RMproperty("rmColorVal", "Color Coords", "Color coordinates of the texture on X axis");
            }

            RMbve();

            RMs(5);

            RMbv();
            if (!rm.rmIsFractal)
            {
                RMproperty("rmParamA", "Param A");
                RMproperty("rmParamB", "Param B");
                if (!rm.rmMappingMaster.mapRmMaster.rmMobileMode)
                {
                    RMproperty("rmParamC", "Param C");
                    RMs(3);
                    RMproperty("rmParamD", "Shape Morph [Param D]");
                    rm.rmParamD = Mathf.Clamp01(rm.rmParamD);
                }
                else
                {
                    RMproperty("rmParamC", "Shape Morph [Param C]");
                    rm.rmParamC = Mathf.Clamp01(rm.rmParamC);
                }
                RMs(5);
            }
            else
            {
                switch(ErmFractalType)
                {
                    case RM_Object.RMFractalType.Apollonian:
                        RMproperty("rmParamA", "Fractal Approach");
                        rm.rmParamA = Mathf.Clamp(rm.rmParamA,0.1f, 3.0f);
                        RMproperty("rmParamB", "Fractal Bend");
                        rm.rmParamB = Mathf.Clamp(rm.rmParamB, 0.1f, 4.0f);
                        if (!rm.rmMappingMaster.mapRmMaster.rmMobileMode)
                        {
                            RMproperty("rmParamC", "Fractal Color Dirt");
                            rm.rmParamC = Mathf.Clamp01(rm.rmParamC);
                        }
                        break;

                    case RM_Object.RMFractalType.Kleinian:
                        RMproperty("rmParamA", "Fractal Approach");
                        rm.rmParamA = Mathf.Clamp(rm.rmParamA, 0.0f, 3.5f);
                        RMproperty("rmParamB", "Fractal Noise");
                        rm.rmParamB = Mathf.Clamp(rm.rmParamB, 0.8f, 2.5f);
                        if (!rm.rmMappingMaster.mapRmMaster.rmMobileMode)
                        {
                            RMproperty("rmParamC", "Fractal Color Dirt");
                            rm.rmParamC = Mathf.Clamp01(rm.rmParamC);
                        }
                        break;

                    case RM_Object.RMFractalType.Mandelbulb:
                        RMproperty("rmParamA", "Fractal Tiling X");
                        rm.rmParamA = Mathf.Clamp(rm.rmParamA, 1.0f, 30.0f);
                        RMproperty("rmParamB", "Fractal Tiling Y");
                        rm.rmParamB = Mathf.Clamp(rm.rmParamB, 1.0f, 30.0f);
                        if (!rm.rmMappingMaster.mapRmMaster.rmMobileMode)
                        {
                            RMproperty("rmParamC", "Fractal Color Dirt");
                            rm.rmParamC = Mathf.Clamp01(rm.rmParamC);
                        }
                        break;

                    case RM_Object.RMFractalType.Tetrahedron:
                        RMproperty("rmParamA", "Fractal Approach");
                        rm.rmParamA = Mathf.Clamp(rm.rmParamA, 1.4f, 2.0f);
                        RMproperty("rmParamB", "Fractal Size");
                        rm.rmParamB = Mathf.Clamp(rm.rmParamB, 0.01f, 10.0f);
                        if (!rm.rmMappingMaster.mapRmMaster.rmMobileMode)
                        {
                            RMproperty("rmParamC", "Fractal Burst");
                            rm.rmParamC = Mathf.Clamp(rm.rmParamC, 0.5f, 1.0f);
                        }
                        break;

                    default:
                        RMproperty("rmShapeSizeA", "Fractal Params A");
                        RMproperty("rmShapeSizeB", "Fractal Params B");
                        RMproperty("rmShapeSizeC", "Fractal Params C");
                        break;
                }
            }
            if (RMb("Reset Params To Default", 220, "Reset parameters above to default values. Usually to 1;1;1.5"))
            {
                if(rm.rmIsFractal)
                    rm.ResetToDefaultParams((int)ErmFractalType);
                else
                    rm.ResetToDefaultParams(0, true);
            }
            RMbve();

            RMs();

            if (!rm.rmMappingMaster.mapRmMaster.rmMobileMode)
            {
                bool ErmOPFragment = EditorGUILayout.ToggleLeft(" Internal Operation - Fragment", rm.rmOPFragment);
                if (ErmOPFragment != rm.rmOPFragment)
                    rm.rmOPFragment = ErmOPFragment;

                if (rm.rmOPFragment)
                {
                    RMbv();
                    RMproperty("rmOPFragmentPar_Size", "Fragment Size");
                    RMproperty("rmOPFragmentPar_Evolution", "Fragment Evolution");
                    RMproperty("rmOPFragmentPar_EnableAnim", "Enable Animation");
                    if (rm.rmOPFragmentPar_EnableAnim)
                        RMproperty("rmOPFragmentPar_AnimDirection", "Direction");
                    else
                        rm.rmOPFragmentPar_AnimDirection = Vector3.zero;
                    RMbve();
                }
            }
            else
            {
                RMbv();
                RMproperty("rmOPFragmentPar_Evolution", "Fragment Operation - Evolution");
                RMbve();
            }

            RMs(5);
            if (!rm.rmOPBeingOperated)
            {
                RMproperty("rmExternalOperation", "External Operation");

                if (rm.rmExternalOperation != RM_Object.RMExternalOperation.None)
                {
                    RMbv();
                    RMproperty("rmOPOperator_Target", "Operation Target");
                    if (!rm.rmMappingMaster.mapRmMaster.rmMobileMode)
                        RMproperty("rmShapeSmoothness", "Smoothness");
                    RMbve();
                }
            }
            else RMl("This object is already a part of external operation!");

            RMs(5);
            RMbv();
            if (RMb("Apply & Compile Object"))
            {
                if (EditorUtility.DisplayDialog("Warning", "Would you like to process this action? The raymarcher will re-write and compile the whole shader which may take a few seconds or even minutes (depending on the raymarcher objects count)", "Yes", "No"))
                {
                    AdvOP_Refresh();
                    rm.ResetToDefaultParams((int)ErmFractalType);
                    rm.rmMappingMaster.MAPsetUpMappingMaster(false);
                    rm.SetupMyself();
                    AssetDatabase.Refresh();
                }
            }
            if (rm.eHist_FractalType != (int)rm.rmFractalType ||
                rm.eHist_WasFractal != rm.rmIsFractal ||
                rm.eHist_WasOperation != (int)rm.rmExternalOperation ||
                rm.eHist_OPTarget != rm.rmOPOperator_Target)
                RMhelpbox("Press 'Apply & Compile Object' to refresh current changes");
            RMbve();

            if (ErmFractalType != rm.rmFractalType)
                rm.rmFractalType = ErmFractalType;

            RMs();

            serializedObject.ApplyModifiedProperties();
		}

        private void AdvOP_Refresh()
        {
            if (!rm.rmMappingMaster)
                rm.rmMappingMaster = (RM_MappingMaster)FindObjectOfType(typeof(RM_MappingMaster));

            if (!rm.rmMappingMaster)
            {
                RMDEBUG.RMDebug(rm, "Mapping Master doesn't exist in the current scene", true);
                return;
            }

            List<string> beingOperatedModels = new List<string>();
            foreach (RM_MappingMaster.MapObjectsContainer obj in rm.rmMappingMaster.mapObjectsContainer)
            {
                if (obj.mapVirtualObject == null)
                    continue;
                RM_Object currTarget = obj.mapVirtualObject.GetComponent<RM_Object>();
                if (currTarget.rmExternalOperation != RM_Object.RMExternalOperation.None && currTarget.rmOPOperator_Target)
                    beingOperatedModels.Add(currTarget.rmOPOperator_Target.rmMyBornName);
            }

            foreach (RM_MappingMaster.MapObjectsContainer obj in rm.rmMappingMaster.mapObjectsContainer)
            {
                if (obj.mapVirtualObject == null)
                    continue;
                RM_Object currTarget = obj.mapVirtualObject.GetComponent<RM_Object>();
                if (beingOperatedModels.Contains(currTarget.rmMyBornName))
                    currTarget.rmOPBeingOperated = true;
                else
                    currTarget.rmOPBeingOperated = false;
            }
        }
    }

#endif
}
