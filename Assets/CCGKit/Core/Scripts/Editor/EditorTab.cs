using UnityEditor;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// CCG Kit editor's base tab class.
    /// CCG Kitエディタの基本タブクラス。
    /// </summary>
    public class EditorTab
    {
        public string name { get; protected set; }

        protected GameConfiguration gameConfig;

        public EditorTab(GameConfiguration config)
        {
            gameConfig = config;
        }
        //virtualはサブクラス独自の変数やメソッドを利用するため、親クラスの変数につける必要がある
        //(子クラスで同じ名前のメソッドが使われている)
        public virtual void OnTabSelected()
        {
        }
        public virtual void Draw()
        {
        }

        protected void DrawDefinitionStat(DefinitionStat stat)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = EditorConfig.LargeLabelWidth;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("名前");
            stat.name = EditorGUILayout.TextField(stat.name, GUILayout.MaxWidth(EditorConfig.RegularTextFieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel("Base value");
            EditorGUILayout.PrefixLabel("基本値");
            stat.baseValue = EditorGUILayout.IntField(stat.baseValue, GUILayout.MaxWidth(EditorConfig.RegularIntFieldWidth));
            stat.originalValue = stat.baseValue;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel("Minimum value");
            EditorGUILayout.PrefixLabel("下限値");
            stat.minValue = EditorGUILayout.IntField(stat.minValue, GUILayout.MaxWidth(EditorConfig.RegularIntFieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel("Maximum value");
            EditorGUILayout.PrefixLabel("上限値");
            stat.maxValue = EditorGUILayout.IntField(stat.maxValue, GUILayout.MaxWidth(EditorConfig.RegularIntFieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        protected void DrawStat(Stat stat)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = EditorConfig.LargeLabelWidth;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel("Name");
            EditorGUILayout.PrefixLabel("名前");
            stat.name = EditorGUILayout.TextField(stat.name, GUILayout.MaxWidth(EditorConfig.RegularTextFieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel("Base value");
            EditorGUILayout.PrefixLabel("基本値");
            stat.baseValue = EditorGUILayout.IntField(stat.baseValue, GUILayout.MaxWidth(EditorConfig.RegularIntFieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel("Minimum value");
            EditorGUILayout.PrefixLabel("下限値");
            stat.minValue = EditorGUILayout.IntField(stat.minValue, GUILayout.MaxWidth(EditorConfig.RegularIntFieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel("Maximum value");
            EditorGUILayout.PrefixLabel("上限値");
            stat.maxValue = EditorGUILayout.IntField(stat.maxValue, GUILayout.MaxWidth(EditorConfig.RegularIntFieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
}
