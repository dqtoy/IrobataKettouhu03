﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;
using UnityEngine.Assertions;

using FullSerializer;

namespace CCGKit
{
    /// <summary>
    /// Contains the entire game configuration details, which are comprised of general game settings,
    /// player/card/effect definitions and the card database.
    /// 一般的なゲーム設定、プレーヤー/カード/エフェクト定義、カードデータベースで構成されているゲーム設定の詳細がすべて含まれています。
    /// </summary>
    public class GameConfiguration
    {

        string checkTeam;
        /// <summary>
        /// The properties of the game.
        /// ゲームのプロパティ。
        /// </summary>
        public GameProperties properties = new GameProperties();

        /// <summary>
        /// The game zones of the game.
        /// ゲームのゲームゾーン。
        /// </summary>
        public List<GameZoneType> gameZones = new List<GameZoneType>();

        /// <summary>
        /// The player stats.
        /// プレーヤーのスタッツ。
        /// </summary>
        public List<DefinitionStat> playerStats = new List<DefinitionStat>();

        /// <summary>
        /// The card types.
        /// カードの種類。
        /// </summary>
        public List<CardType> cardTypes = new List<CardType>();

        /// <summary>
        /// The card types.
        /// カードの種類。
        /// </summary>
        public List<HeroPowerType> HeroPowerTypes = new List<HeroPowerType>();

        /// <summary>
        /// The card types.
        /// トークンの種類。
        /// </summary>
        public List<CardType> tokenTypes = new List<CardType>();

        /// <summary>
        /// The keywords of the game.
        /// ゲームのキーワード。
        /// </summary>
        public List<Keyword> keywords = new List<Keyword>();

        /// <summary>
        /// The card sets of the game.
        /// ゲームのカードセット。(陣営が入ったリスト)
        /// </summary>
        public List<CardSet> cardSets = new List<CardSet>();

        /// <summary>
        /// The card sets of the game.
        /// ゲームのヒロパセット。
        /// </summary>
        public List<HeroPowerSet> HeroPowerSets = new List<HeroPowerSet>();

        /// <summary>
        /// The card sets of the game.
        /// ゲームのカードセット。
        /// </summary>
        public List<CardSet> tokenSets = new List<CardSet>();

        /// <summary>
        /// The cards of the game.
        /// ゲームのカード。
        /// </summary>
        public List<Card> cards = new List<Card>();

        /// <summary>
        /// ヒロパの一覧。
        /// </summary>
        public List<HeroPower> heroPowers = new List<HeroPower>();

        /// <summary>
        /// トークンの一覧。
        /// </summary>
        public List<Card> tokens = new List<Card>();

        /// <summary>
        /// The JSON serializer.
        /// JSONシリアライザ
        /// </summary>
        private fsSerializer serializer = new fsSerializer();

        private TokenPool currentTokenPool;

        /// <summary>
        /// Loads the game configuration from the specified path.
        /// 指定されたパスからゲーム設定をロードします。
        /// </summary>
        /// <param name="path">The path to the game configuration.</param>
        public void LoadGameConfiguration(string path)
        {
            var gamePropertiesPath = path + "/game_properties.json";
            var gameProperties = LoadJSONFile<GameProperties>(gamePropertiesPath);
            if (gameProperties != null)
            {
                properties = gameProperties;
            }

            var gameZonesPath = path + "/game_zones.json";
            var zones = LoadJSONFile<List<GameZoneType>>(gameZonesPath);
            if (zones != null)
            {
                gameZones = zones;
            }
            if (gameZones.Count > 0)
            {
                GameZoneType.currentId = gameZones.Max(x => x.id) + 1;
            }

            var playerStatsPath = path + "/player_stats.json";
            var stats = LoadJSONFile<List<DefinitionStat>>(playerStatsPath);
            if (stats != null)
            {
                playerStats = stats;
            }
            if (playerStats.Count > 0)
            {
                PlayerStat.currentId = playerStats.Max(x => x.id) + 1;
            }

            var cardTypesPath = path + "/card_types.json";
            var types = LoadJSONFile<List<CardType>>(cardTypesPath);
            if (types != null)
            {
                cardTypes = types;
            }
            if (cardTypes.Count > 0)
            {
                CardType.currentId = cardTypes.Max(x => x.id) + 1;
            }
            var ids = new List<int>();
            foreach (var type in cardTypes)
            {
                if (type.stats.Count > 0)
                {
                    ids.Add(type.stats.Max(x => x.id));
                }
            }
            if (ids.Count > 0)
            {
                CardStat.currentId = ids.Max() + 1;
            }

            var keywordsPath = path + "/keywords.json";
            var keywords = LoadJSONFile<List<Keyword>>(keywordsPath);
            if (keywords != null)
            {
                this.keywords = keywords;
            }
            if (this.keywords.Count > 0)
            {
                Keyword.currentId = this.keywords.Max(x => x.id) + 1;
            }

            ///各陣営分けはここで設定する？
            var cardLibraryPath = path + "/card_library.json";
            var KoumaCardLibraryPath = path + "/Kcard_library.json";
            var HakugyokuCardLibraryPath = path + "/Hcard_library.json";
            var EienCardLibraryPath = path + "/Ecard_library.json";
            var tokenLibraryPath = path + "/token_library.json";
            //カードの一覧をIDで取得、Listにして返す
            var cardLibrary = LoadJSONFile<List<CardSet>>(cardLibraryPath);
            //           var KoumaCardLibrary = LoadJSONFile<List<CardSet>>(KoumaCardLibraryPath);
            //            var HakugyokuCardLibrary = LoadJSONFile<List<CardSet>>(HakugyokuCardLibraryPath);
            //          var EienCardLibrary = LoadJSONFile<List<CardSet>>(EienCardLibraryPath);
            var tokenLibrary = LoadJSONFile<List<CardSet>>(tokenLibraryPath);
            if (cardLibrary != null)
            {
                cardSets = cardLibrary;
            }
            if (tokenLibrary != null)
            {
                tokenSets = tokenLibrary;
            }
            var max = -1;
            //inは以降にはコンテナを指定する
            //コンテナ(コレクション)は、配列やリスト、辞書などの複数の要素をひとつにまとめるクラスのこと
            //setはCardSetクラス
            //foreachはここ参照 http://ufcpp.net/study/csharp/sp_foreach.html
            var i = 1;
            foreach (var set in cardSets)
            {

                //カード一覧からの各陣営のIDを取得(cardsはCardSetクラス)
                var currentMax = set.cards.Max(x => x.id);

                //Debug.Log(currentMax);


                if (currentMax > max)
                {
                    max = currentMax;
                    /*
                    if (max == 305)
                    {
                        break;
                    }
                    */
                }
                i++;
            }
            Card.currentId = max + 1;

            /*
            checkTeam=SelectTeamScene.GetTeamFlag();
            if(checkTeam!=null){
                if(checkTeam=="kouma"){
                   cardLibraryPath = path + "/kouma_card_library.json";
                }else if(checkTeam=="hakugyoku"){
                    cardLibraryPath = path + "/hakugyoku_card_library.json";
                }else if(checkTeam=="eien"){
                    cardLibraryPath = path + "/eien_card_library.json";
                }else{

                }
                
            }
             */

            var tokensPath = path + "/token_library.json";

            // トークンデータが存在する場合、GameManager.Instance.playerDecksにデッキデータを代入する処理
            if (File.Exists(tokensPath))
            {
                //外部データ(デッキファイル)の読み込み
                var file = new StreamReader(tokensPath);
                var fileContents = file.ReadToEnd();
                //fsJsonParserはJSONの単純な再帰的降下パーサー。
                //トークンデータの解析をする
                var data = fsJsonParser.Parse(fileContents);
                object deserialized = null;
                //トークンデータをマシン上で使える形に変換してる
                serializer.TryDeserialize(data, typeof(List<Deck>), ref deserialized).AssertSuccessWithoutWarnings();
                file.Close();
                GameManager.Instance.allPlayerTokens = deserialized as List<TokenPool>;
            }

            ///トークン
/*
            var tokenLibrary = LoadJSONFile<List<TokenSet>>(tokenLibraryPath);
            if (tokenLibrary != null)
            {
                tokenSets = tokenLibrary;
            }
            var token_max = 416;
            foreach (var set in tokenSets)
            {
                var currentMax = set.Tokens.Max(x => x.id);
                if (currentMax > token_max)
                {
                    token_max = currentMax;
                }
            }
            Token.currentId = token_max + 1;
            */

        }

        /// <summary>
        /// Loads the game configuration at runtime.
        /// 実行時にゲームの設定を読み込みます。
        /// </summary>
        public void LoadGameConfigurationAtRuntime()
        {
            //全てのカードデータ、デッキに構築できるカード枚数、1ターンの時間、ターン開始時の行動、ターン終了時の行動をTextAssetにキャストして読み込む
            var gamePropertiesJSON = Resources.Load<TextAsset>("game_properties");
            //gamePropertiesJSONがnullではないことを保証する
            Assert.IsTrue(gamePropertiesJSON != null);
            var gameProperties = LoadJSONString<GameProperties>(gamePropertiesJSON.text);
            if (gameProperties != null)
            {
                properties = gameProperties;
            }

            //デッキ、ハンド、ボード、墓地それぞれの最大値を設定する
            var gameZonesJSON = Resources.Load<TextAsset>("game_zones");
            Assert.IsTrue(gameZonesJSON != null);
            var zones = LoadJSONString<List<GameZoneType>>(gameZonesJSON.text);
            if (zones != null)
            {
                gameZones = zones;
            }

            //ライフとマナの最大値を設定する
            var playerStatsJSON = Resources.Load<TextAsset>("player_stats");
            Assert.IsTrue(playerStatsJSON != null);
            var stats = LoadJSONString<List<DefinitionStat>>(playerStatsJSON.text);
            if (stats != null)
            {
                playerStats = stats;
            }

            //ミニオンの属性、スタッツの扱い、ミニオンが死ぬ条件、スペルについてそれぞれ設定する
            var cardTypesJSON = Resources.Load<TextAsset>("card_types");
            Assert.IsTrue(cardTypesJSON != null);
            var types = LoadJSONString<List<CardType>>(cardTypesJSON.text);
            if (types != null)
            {
                cardTypes = types;
            }

            //keywordsの設定
            var keywordsJSON = Resources.Load<TextAsset>("keywords");
            Assert.IsTrue(keywordsJSON != null);
            var keywords = LoadJSONString<List<Keyword>>(keywordsJSON.text);
            if (keywords != null)
            {
                this.keywords = keywords;
            }


            //カード一覧を読み込む。
            var cardLibraryJSON = Resources.Load<TextAsset>("card_library");


//            var koumaCardLibraryJSON = Resources.Load<TextAsset>("kouma_card_library");
//            var hakugyokuCardLibraryJSON = Resources.Load<TextAsset>("hakugyoku_card_library"); 
//            var eienCardLibraryJSON = Resources.Load<TextAsset>("eien_card_library");        

            //nullでないことを確認
            Assert.IsTrue(cardLibraryJSON != null);
            //CardSetのListに読み込んだカード一覧のテキストを代入
            //カードセットには陣営の情報とカードの一覧情報を所持
            var cardLibrary = LoadJSONString<List<CardSet>>(cardLibraryJSON.text);

            /*
                        checkTeam=SelectTeamScene.GetTeamFlag();
                        if(checkTeam!=null){
                            if(checkTeam=="kouma"){
                               cardLibrary = LoadJSONString<List<CardSet>>(koumaCardLibraryJSON.text);
                            }else if(checkTeam=="hakugyoku"){
                                cardLibrary = LoadJSONString<List<CardSet>>(hakugyokuCardLibraryJSON.text);
                            }else if(checkTeam=="eien"){
                                cardLibrary = LoadJSONString<List<CardSet>>(eienCardLibraryJSON.text);
                            }else{

                            }
                        }

             */

            //            Debug.Log(cardLibrary[2].Id);

            //Cardクラスのリストでカードの一覧作成
            if (cardLibrary != null)
            {
                cardSets = cardLibrary;
                foreach (var set in cardSets)
                {
                    foreach (var card in set.cards)
                    {
                        cards.Add(card);
                    }
                }
            }


            //トークン一覧を読み込む。
            var tokenLibraryJSON = Resources.Load<TextAsset>("token_library");
            Assert.IsTrue(cardLibraryJSON != null);
            var tokenLibrary = LoadJSONString<List<CardSet>>(tokenLibraryJSON.text);
            //トークンの一覧作成
            if (tokenLibrary != null)
            {
                tokenSets = tokenLibrary;
                foreach (var set in tokenSets)
                {
                    foreach (var token in set.cards)
                    {
                        tokens.Add(token);
//                        Debug.Log(tokens[0]);
//                       currentTokenPool.AddToken(token);
                    }
                }
            }

        }

        /// <summary>
        /// Loads the JSON file from the specified path.
        /// 指定されたパスからJSONデータを読み込みます。
        /// →プログラム中で扱える型(string等)を返す
        /// </summary>
        /// <typeparam name="T">The type of data to load.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <returns>The data contained in the file.</returns>
        private T LoadJSONFile<T>(string path) where T : class
        {
            if (File.Exists(path))
            {
                //StreamReaderクラスのオブジェクト作成。
                //pathからデータを読み出すためのストリームを利用できるようになる
                //ストリームとは…流れ込んでくるデータを入力、流れ出ていくデータを出力として扱う抽象データ型
                var file = new StreamReader(path);
                //指定したパスのファイルをstring型で頭から最後まで読み込む
                var fileContents = file.ReadToEnd();
                //JSONファイルの解析
                var data = fsJsonParser.Parse(fileContents);
                object deserialized = null;
                serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();
                file.Close();
                return deserialized as T;
            }
            return null;
        }

        /// <summary>
        /// Loads the JSON data from the specified string.
        /// 指定された文字列からJSONデータを読み込みます。
        /// </summary>
        /// <typeparam name="T">The type of data to load.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>The data contained in the string.</returns>
        private T LoadJSONString<T>(string json) where T : class
        {
            var data = fsJsonParser.Parse(json);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();
            return deserialized as T;
        }

        /// <summary>
        /// Saves the game configuration to the specified path.
        /// ゲームの設定を指定されたパスに保存します。
        /// </summary>
        /// <param name="path">The path to the game configuration.</param>
        public void SaveGameConfiguration(string path)
        {
#if UNITY_EDITOR
            SaveJSONFile(path + "/game_properties.json", properties);
            SaveJSONFile(path + "/game_zones.json", gameZones);
            SaveJSONFile(path + "/player_stats.json", playerStats);
            SaveJSONFile(path + "/card_types.json", cardTypes);
            SaveJSONFile(path + "/keywords.json", keywords);
            SaveJSONFile(path + "/card_library.json", cardSets);

//            Debug.Log(cardSets);

            SaveJSONFile(path + "/heroPower_library.json", HeroPowerSets);
            SaveJSONFile(path + "/token_library.json", tokenSets);
            //            SaveJSONFile(path + "/kouma_card_library.json", cardSets);
            //            SaveJSONFile(path + "/hakugyoku_card_library.json", cardSets);
            //            SaveJSONFile(path + "/eien_card_library.json", cardSets);


            AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// Saves the game configuration to the path selected by the user.
        /// ユーザーが選択したパスにゲームの設定を保存します。
        /// </summary>
        public void SaveGameConfigurationAs()
        {
#if UNITY_EDITOR
//            var path = EditorUtility.OpenFolderPanel("Select game configuration folder", "", "");
            var path = EditorUtility.OpenFolderPanel("ファイルの保存先を指定", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                EditorPrefs.SetString("GameConfigurationPath", path);
                SaveGameConfiguration(path);
            }
#endif
        }

        /// <summary>
        /// Saves the specified data to the specified path.
        /// 指定されたデータを指定されたパスに保存します。
        /// </summary>
        /// <typeparam name="T">The type of data to save.</typeparam>
        /// <param name="path">The path where to save the data.</param>
        /// <param name="data">The data to save.</param>
        /// 要検討unicodeで保存する方法
        /// →DependenciesのfsJsonPrinterクラスで保存してる
        private void SaveJSONFile<T>(string path, T data) where T : class
        {
            fsData serializedData;
            serializer.TrySerialize(data, out serializedData).AssertSuccessWithoutWarnings();
            var file = new StreamWriter(path);
            var json = fsJsonPrinter.PrettyJson(serializedData);
            file.WriteLine(json);
            file.Close();
        }

        /// <summary>
        /// Returns the card with the specified identifier.
        /// 指定された識別子を持つカードを返します。
        /// </summary>
        /// <param name="id">The unique identifier of the card.</param>
        /// <returns>The card with the specified identifier.</returns>
        public Card GetCard(int id)
        {
            var libraryCard = cards.Find(x => x.id == id);
            return libraryCard;
        }

        /// <summary>
        /// Returns the card with the specified identifier.
        /// 渡された文字列の名前を持つカードを返します。
        /// </summary>
        /// <param name="name">JSONから引っ張ってきたunicode</param>
        /// <returns>The card with the specified identifier.</returns>
        public Card GetCardFromName(string name)
        {
            var libraryCard = cards.Find(x => x.name == name);
            return libraryCard;
        }



        /// <summary>
        /// 指定された識別子を持つヒロパを返す。
        /// </summary>
        public HeroPower GetHeroPower(int id)
        {
            var libraryHeroPower = heroPowers.Find(x => x.id == id);
            return libraryHeroPower;
        }

        /// <summary>
        /// 指定された識別子を持つトークンを返す。
        /// </summary>
        public Card GetToken(int id)
        {
            var libraryToken = cards.Find(x => x.id == id);
            return libraryToken;
        }



        /// <summary>
        /// Returns the number of cards in the configuration.
        /// ユーザ作成のデッキやゲーム内で使用するカードの総数等、構成内のカードの数を返します。
        /// </summary>
        /// <returns>The number of cards in the configuration.</returns>
        public int GetNumCards()
        {
            return cards.Count;
        }

        public int GetNumTeamCards(List<Card> card)
        {
            return card.Count;
        }
    }
}
