using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// フォント一括変更クラス
/// Create 2017/01/06 @Akubi
/// Update 2016/01/07 @Akubi
/// </summary>
public class FontSet : EditorWindow
{
    /// <summary>フォントファイル</summary>
    private Font font;

    /// <summary>
    /// UnityEditorメニュー追加
    /// </summary>
    [MenuItem("Tools/フォント一括変更")]
    public static void ReplaceFont()
    {
        GetWindow(typeof(FontSet)).Show();
    }

    /// <summary>
    /// GUI制御
    /// </summary>
    void OnGUI()
    {
        GUILayout.Label("フォント選択後に一括置換ボタン押下で\nScene上のフォントを一括置換します。");
        font = EditorGUILayout.ObjectField("Font", font, typeof(Font), true) as Font;
        if (GUILayout.Button("一括置換"))
            if (font != null)
            {
                ReplaceLabelsFontInScene(font);
            }
    }

    /// <summary>
    /// フォント変更処理
    /// </summary>
    /// <param name="selectFont"></param>
    private static void ReplaceLabelsFontInScene(Font selectFont)
    {
        Text[] labels = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];
        if (labels.Length != 0)
        {
            foreach (Text label in labels)
            {
                label.font = selectFont;
                Debug.Log(label.name + "のフォントを" + selectFont.name + "に変更しました");
            }
            Debug.Log("置換が完了しました。");
        }
    }
}