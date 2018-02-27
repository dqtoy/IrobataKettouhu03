using System.Reflection;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Custom attribute for game zones.
    /// ゲームゾーンのカスタム属性。
    /// </summary>
    public class GameZoneFieldAttribute : FieldAttribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prefix">Prefix.</param>
        public GameZoneFieldAttribute(string prefix) : base(prefix)
        {
            width = 100;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Draws this attribute.
        /// </summary>
        /// <param name="gameConfig">The configuration of the game.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="field">The field information.</param>
        public override void Draw(GameConfiguration gameConfig, object instance, ref FieldInfo field)
        {
            EditorGUILayout.PrefixLabel(prefix);
            var gameZones = gameConfig.gameZones;
            var options = new string[gameZones.Count];
            for (var i = 0; i < gameZones.Count; i++)
            {
                options[i] = gameZones[i].name;
            }

            if (options.Length > 0)
            {
                var zoneId = (int)field.GetValue(instance);
                if (gameZones.Find(x => x.id == zoneId) == null)
                {
                    field.SetValue(instance, 0);
                }

                var zone = gameZones.Find(x => x.id == zoneId);
                var zoneIndex = System.Array.FindIndex(options, x => x == zone.name);

                var newZoneIndex = EditorGUILayout.Popup(zoneIndex, options, GUILayout.MaxWidth(width));
                var newZone = options[newZoneIndex];
                field.SetValue(instance, gameZones.Find(x => x.name == newZone).id);
            }
        }

#endif
    }
}
