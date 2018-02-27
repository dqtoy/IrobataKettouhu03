// Copyright (C) 2016-2017 David Pol. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The CCG Kit editor accessible from Unity's menu. This editor provides an intuitive way to define
    /// the fundamental properties of a collectible card game.
    /// Unityのメニューからアクセス可能なCCG Kitエディタ。 このエディタは、コレクタブルカードゲームの基本的なプロパティを定義する直感的な方法を提供します。
    /// </summary>
    public class CCGKitEditor : EditorWindow
    {
        private GameConfiguration gameConfig;

        //設定ファイルのパス
        private string gameConfigPath;

        private int selectedTabIndex = -1;
        private int prevSelectedTabIndex = -1;

        private List<EditorTab> tabs = new List<EditorTab>();

        private Vector2 scrollPos;

        //メニューのWindowにCCG Kit Editorという項目を追加。
        [MenuItem("Window/CCG Kit Editor")]
        private static void Init()
        {
            // メニューのWindow/CCGKitEditorを選択するとOpen()またはInit()が呼ばれる。
            // 表示させたいウィンドウは基本的にGetWindow()で表示＆取得する。
            var window = GetWindow(typeof(CCGKitEditor));
            window.titleContent = new GUIContent("CCG Kit Editor");
        }

        private void OnEnable()
        {
            if (EditorPrefs.HasKey("GameConfigurationPath"))
            {
                gameConfigPath = EditorPrefs.GetString("GameConfigurationPath");
                gameConfig = new GameConfiguration();
                gameConfig.LoadGameConfiguration(gameConfigPath);
                ResetEditorTabs();
                selectedTabIndex = 0;
            }
        }

        private void ResetGameConfiguration()
        {
            gameConfig = new GameConfiguration();
            selectedTabIndex = 0;
            ResetEditorTabs();
            ResetIds();
        }
        /// <summary>
        /// タブの初期化処理
        /// </summary>
        private void ResetEditorTabs()
        {
            tabs.Clear();
            tabs.Add(new GameConfigurationEditor(gameConfig));
            tabs.Add(new GameZonesEditor(gameConfig));
            tabs.Add(new PlayerEditor(gameConfig));
            tabs.Add(new CardTypesEditor(gameConfig));
            tabs.Add(new KeywordEditor(gameConfig));
            tabs.Add(new CardCollectionEditor(gameConfig));
            tabs.Add(new AboutEditor(gameConfig));
        }

        private void ResetIds()
        {
            GameZoneType.currentId = 0;
            PlayerStat.currentId = 0;
            CardType.currentId = 0;
            CardStat.currentId = 0;
            Card.currentId = 0;
            Keyword.currentId = 0;
        }

        private void OpenGameConfiguration()
        {
            var path = EditorUtility.OpenFolderPanel("Select game configuration folder", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                gameConfigPath = path;
                gameConfig = new GameConfiguration();
                gameConfig.LoadGameConfiguration(gameConfigPath);
                ResetEditorTabs();
                selectedTabIndex = 0;
                EditorPrefs.SetString("GameConfigurationPath", gameConfigPath);
            }
        }

        //基本ボタンを配置
        private void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("新規に設定ファイルを作成", GUILayout.MaxWidth(200)))
            {
                ResetGameConfiguration();
            }
            if (GUILayout.Button("設定ファイルを開く", GUILayout.MaxWidth(200)))
            {
                OpenGameConfiguration();
            }
            if (GUILayout.Button("保存", GUILayout.MaxWidth(200)))
            {
                gameConfig.SaveGameConfiguration(gameConfigPath);
            }
            if (GUILayout.Button("保存パスを指定", GUILayout.MaxWidth(200)))
            {
                gameConfig.SaveGameConfigurationAs();
                //保存先を取得
                gameConfigPath = EditorPrefs.GetString("GameConfigurationPath");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("現在の保存先: ", GUILayout.MaxWidth(90));
            GUILayout.Label(gameConfigPath);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (gameConfig == null)
            {
                return;
            }

            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, new string[] { "ゲームの基本設定", "場の設定", "プレイヤーの設定", "カードタイプ", "キーワード", "カード設定", "CCG Kitについて" });
            //初期化時に実装した機能の数とタブの数を比較し、対象の機能とタブを結びつける
            if (selectedTabIndex >= 0 && selectedTabIndex < tabs.Count)
            {
                var selectedEditor = tabs[selectedTabIndex];
                if (selectedTabIndex != prevSelectedTabIndex)
                {
                    selectedEditor.OnTabSelected();
                }
                selectedEditor.Draw();

                prevSelectedTabIndex = selectedTabIndex;
            }

            GUILayout.EndScrollView();
        }
    }
}
