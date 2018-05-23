using System.Reflection;

#if UNITY_EDITOR

using UnityEditor;

#endif
using System;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// ユニークなトークンを名前で指定するカスタム属性
    /// </summary>

    //Attributeについては以下を参照
    //http://ufcpp.net/study/csharp/sp_attribute.html
    [AttributeUsage(AttributeTargets.Class)]
    public class UniqueTokenAttribute : FieldAttribute
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prefix">Prefix.</param>
        public UniqueTokenAttribute(string prefix) : base(prefix)
        {
            width = 100;
        }
#if UNITY_EDITOR

        /// <summary>
        /// Draws this attribute.
        /// 属性を描画
        /// </summary>
        /// <param name="gameConfig">The configuration of the game.CaradTypeを作るために使う</param>
        /// <param name="instance">The instance.</param>
        /// <param name="field">The field information.</param>
        public override void Draw(GameConfiguration gameConfig, object instance, ref FieldInfo field)
        {
            /*
            EditorGUILayout.PrefixLabel(prefix);
            var tokenSets = gameConfig.tokenSets;
            var tokens = tokenSets.name;
            var options = new string[tokenSets.Count];
            for (var i = 0; i < tokenSets.Count; i++)
            {
                options[i] = tokenSets[i].name;
            }

            if (options.Length > 0)
            {
                var cardTypeId = (int)field.GetValue(instance);
                if (tokenSets.Find(x => x.id == cardTypeId) == null)
                {
                    field.SetValue(instance, 0);
                }

                var type = cardTypes.Find(x => x.id == cardTypeId);
                var typeIndex = System.Array.FindIndex(options, x => x == type.name);

                var newTypeIndex = EditorGUILayout.Popup(typeIndex, options, GUILayout.MaxWidth(width));
                var newType = options[newTypeIndex];
                field.SetValue(instance, cardTypes.Find(x => x.name == newType).id);
            }
            */
        }

#endif



    }
}