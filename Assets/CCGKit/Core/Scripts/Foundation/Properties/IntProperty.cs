#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

namespace CCGKit
{
    public class IntProperty : Property
    {
        public int value;

#if UNITY_EDITOR

        public override void Draw()
        {
            base.Draw();
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Default value");
            value = EditorGUILayout.IntField(value, GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
        }

#endif
    }
}