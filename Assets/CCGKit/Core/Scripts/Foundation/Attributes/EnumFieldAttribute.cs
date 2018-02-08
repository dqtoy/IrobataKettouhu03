using System;
using System.Reflection;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Custom attribute for enums.
    /// enum‚ÌƒJƒXƒ^ƒ€‘®«
    /// </summary>
    public class EnumFieldAttribute : FieldAttribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prefix">Prefix.</param>
        public EnumFieldAttribute(string prefix) : base(prefix)
        {
            width = 100;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Draws this attribute.
        /// ‚±‚Ì‘®«‚Ì•`‰æ
        /// </summary>
        /// <param name="gameConfig">The configuration of the game.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="field">The field information.</param>
        public override void Draw(GameConfiguration gameConfig, object instance, ref FieldInfo field)
        {
            EditorGUILayout.PrefixLabel(prefix);
            field.SetValue(instance, EditorGUILayout.EnumPopup((Enum)field.GetValue(instance), GUILayout.MaxWidth(width)));
        }

#endif
    }
}