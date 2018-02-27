#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The base class for properties.
    /// プロパティの基本クラス。
    /// </summary>
    public class Property
    {
        /// <summary>
        /// The name of this property.
        /// このプロパティの名前。
        /// </summary>
        public string name;

#if UNITY_EDITOR

        /// <summary>
        /// Draws this property in the editor.
        /// このプロパティをエディタに描画します。
        /// </summary>
        public virtual void Draw()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Name");
            name = EditorGUILayout.TextField(name, GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
        }

#endif
    }
}
