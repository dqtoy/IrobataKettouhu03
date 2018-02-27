#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

namespace CCGKit
{
    public class StringProperty : Property
    {
        public string value;

#if UNITY_EDITOR

        public override void Draw()
        {
            base.Draw();
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Default value");
            value = EditorGUILayout.TextField(value, GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
        }

#endif
    }
}