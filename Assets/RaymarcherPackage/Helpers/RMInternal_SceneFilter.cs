using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RaymarcherPackage
{
    /// <summary>
    /// Raymarcher internal - Scene View Filter allows user to see raymarch render in the Unity Editor realtime
    /// </summary>
    public class RMInternal_SceneFilter : MonoBehaviour
    {
#if UNITY_EDITOR
        private bool hasValidated = false;

        public virtual void OnValidate()
        {
            hasValidated = true;
        }

        static RMInternal_SceneFilter()
        {
            SceneView.duringSceneGui += CheckCamFilter;
        }

        private static void CheckCamFilter(SceneView sv)
        {
            if (Event.current.type != EventType.Layout)
                return;
            if (!Camera.main)
                return;
            
            RMInternal_SceneFilter[] cameraFilters = Camera.main.GetComponents<RMInternal_SceneFilter>();
            RMInternal_SceneFilter[] sceneFilters = sv.camera.GetComponents<RMInternal_SceneFilter>();

            if (cameraFilters.Length != sceneFilters.Length)
            {
                RecreateCamFilter(sv);
                return;
            }

            for (int i = 0; i < cameraFilters.Length; i++)
            {
                if (cameraFilters[i].GetType() != sceneFilters[i].GetType())
                {
                    RecreateCamFilter(sv);
                    return;
                }
            }

            for (int i = 0; i < cameraFilters.Length; i++)
            {
                if (cameraFilters[i].hasValidated || sceneFilters[i].enabled != cameraFilters[i].enabled)
                {
                    EditorUtility.CopySerialized(cameraFilters[i], sceneFilters[i]);
                    cameraFilters[i].hasValidated = false;
                }
            }
        }

        private static void RecreateCamFilter(SceneView sv)
        {
            RMInternal_SceneFilter filter;
            while (filter = sv.camera.GetComponent<RMInternal_SceneFilter>())
                DestroyImmediate(filter);

            foreach (RMInternal_SceneFilter f in Camera.main.GetComponents<RMInternal_SceneFilter>())
            {
                RMInternal_SceneFilter newFilter = sv.camera.gameObject.AddComponent(f.GetType()) as RMInternal_SceneFilter;
                EditorUtility.CopySerialized(f, newFilter);
            }
        }
#endif
    }
}
