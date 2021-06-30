using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RaymarcherPackage
{
#if UNITY_EDITOR

    /// <summary>
    /// Internal utility & editor for Developer
    /// </summary>
    public class RM_InternalEditor : Editor
	{
        protected void RMproperty(string name, string text = "DEFAULT", string tooltip = "")
		{
            string txt = text;
			if(txt == "DEFAULT")
                EditorGUILayout.PropertyField(serializedObject.FindProperty(name));
            else
                EditorGUILayout.PropertyField(serializedObject.FindProperty(name), new GUIContent(txt, tooltip));
        }

        protected void RMpropertyList(string name, string text = "DEFAULT", string tooltip = "")
        {
            string txt = text;
            if (txt == "DEFAULT")
                EditorGUILayout.PropertyField(serializedObject.FindProperty(name),true);
            else
                EditorGUILayout.PropertyField(serializedObject.FindProperty(name), new GUIContent(txt, tooltip), true);
        }

        protected string RMtxtfield(string text, float multiLine)
        {
            return GUILayout.TextArea(text, GUILayout.MinHeight(multiLine));
        }

        protected void RMhelpbox(string text, MessageType m = MessageType.Warning)
        {
            EditorGUILayout.HelpBox(text, m);
        }

        protected void RMlvlPlus(int lvl = 2)
        {
            EditorGUI.indentLevel += lvl;
        }

        protected void RMlvlMinus(int lvl = 2)
        {
            EditorGUI.indentLevel -= lvl;
        }

        protected void RMbv(bool box = true)
		{
			if(box)
				GUILayout.BeginVertical("Box");
			else
				GUILayout.BeginVertical();
		}

        protected void RMbve()
		{
			GUILayout.EndVertical();
		}

        protected void RMbh(bool box = true)
        {
            if (box)
                GUILayout.BeginHorizontal("Box");
            else
                GUILayout.BeginHorizontal();
        }

        protected void RMbhe()
        {
            GUILayout.EndHorizontal();
        }

        protected void RMicon(Texture2D entry)
        {
            GUILayout.Label(entry);
        }

        protected void RMs(float index = 10)
		{
			GUILayout.Space(index);
		}

        protected bool RMb(string innerText, float Width = 0, string tooltip = "")
		{
			if(Width == 0)
				return GUILayout.Button(new GUIContent(innerText, tooltip));
			else
				return GUILayout.Button(new GUIContent(innerText, tooltip), GUILayout.Width(Width));
		}

        protected bool RMb(string innerText, Texture2D img)
        {
            return GUILayout.Button(new GUIContent(innerText, img));
        }

        protected void RMl(string text, bool bold = false, int size = 0)
		{
            GUIStyle style = new GUIStyle(GUI.skin.label) { richText = true };
            if (size != 0) style.fontSize = size;
            if(bold) style.fontStyle = FontStyle.Bold;
            GUILayout.Label(text, style);
		}

        protected void RMt(Texture2D texture)
        {
            GUIStyle stl = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label(texture, stl);
        }
    }

#endif

    /// <summary>
    /// Raymarcher-specified debug
    /// </summary>
    public static class RMDEBUG
    {
        /// <summary>
        /// Internal debug output
        /// </summary>
        /// <param name="sender">Mono-sender</param>
        /// <param name="message">Msg</param>
        /// <param name="error">Is it error or default debug?</param>
        public static void RMDebug(MonoBehaviour sender, string message, bool error = false)
        {
            if (RM_Master.rmEnableDebugMessages == false) return;
            if (error)
                Debug.LogError("Raymarcher ERROR Message - " + sender.GetType().Name + ": " + message);
            else
                Debug.Log("Raymarcher Message - " + sender.GetType().Name + ": " + message);
        }
    }
}