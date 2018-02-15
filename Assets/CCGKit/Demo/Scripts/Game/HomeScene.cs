using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

using FullSerializer;
using TMPro;

#if ENABLE_MASTER_SERVER_KIT

using System.Collections;

using MasterServerKit;

#endif

using CCGKit;

/// <summary>
/// For every scene in the demo game, we create a specific game object to handle the user-interface
/// logic belonging to that scene. The home scene just contains the button delegates that trigger
/// transitions to other scenes or exit the game.
/// デモゲームのすべてのシーンについて、特定のゲームオブジェクトを作成して、
/// そのシーンに属するユーザーインターフェイスロジックを処理します。
/// ホームシーンには、他のシーンへのトランジションをトリガーする、
/// またはゲームを終了するボタンデリゲートが含まれています。
/// </summary>
public class HomeScene : BaseScene
{
    [SerializeField]
    private TextMeshProUGUI versionText;

    private fsSerializer serializer = new fsSerializer();

    [SerializeField]
    private TMP_InputField gameNameInputField;

    [SerializeField]
    private TMP_InputField passwordInputField;

    /// <summary>
    /// プログラムの前提として満たされるべき条件を記述し、実行時にそれが満たされていない場合にエラーや例外を発生させたり、メッセージを表示して処理を中断したりする機能
    /// アサーションチェック
    /// </summary>
    private void Awake()
    {
        Assert.IsNotNull(versionText);
    }

    private void Start()
    {

        SoundController.setloopDefine=5.454f;
        SoundController.setendDefine=101.818f;

        //BGM再生。AUDIO.BGM_BATTLEがBGMのファイル名
        SoundController.Instance.PlayBGM ("GS-premaster",SoundController.BGM_FADE_SPEED_RATE_HIGH);
   	    SoundController.Instance.ChangeVolume (0.2F,0.2F);

        Application.targetFrameRate = 60;

        versionText.text = "Ver" + CCGKitInfo.version;
        //C:\Users\imoho\AppData\LocalLow\chorimpoo\囲炉端決闘符/decks.json
        var decksPath = Application.persistentDataPath + "/decks.json";

        // デッキデータが存在する場合、GameManager.Instance.playerDecksにデッキデータを代入する処理
        if (File.Exists(decksPath))
        {
            //外部データ(デッキファイル)の読み込み
            var file = new StreamReader(decksPath);
            var fileContents = file.ReadToEnd();
            //fsJsonParserはJSONの単純な再帰的降下パーサー。
            //デッキデータの解析をする
            var data = fsJsonParser.Parse(fileContents);
            object deserialized = null;
            //デッキデータをマシン上で使える形に変換してる
            serializer.TryDeserialize(data, typeof(List<Deck>), ref deserialized).AssertSuccessWithoutWarnings();
            file.Close();
            GameManager.Instance.playerDecks = deserialized as List<Deck>;
        }

        GameNetworkManager.Instance.Initialize();

#if ENABLE_MASTER_SERVER_KIT
        if (!GameManager.Instance.isPlayerLoggedIn)
        {
            StartCoroutine(LoadMSKHome());
        }
#endif
    }

#if ENABLE_MASTER_SERVER_KIT

    private IEnumerator LoadMSKHome()
    {
        var async = SceneManager.LoadSceneAsync("MSK_Home", LoadSceneMode.Additive);
        while (!async.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        ClientAPI.ConnectToMasterServer(() =>
        {
            Debug.Log("Connected to master server.");
        },
        () =>
        {
            Debug.Log("Could not connect to master server.");
        });
        OpenPopup<PopupLogin>("PopupLogin", popup =>
        {
        });
    }

#endif

    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene("SelectHero");
    }

    public void OnOnlinePlayButtonPressed()
    {
        OpenPopup<PopupTwoButtons>("PopupTwoButtons", popup =>
        {
            popup.text.text = "接続";
            popup.buttonText.text = "サーバ";
            popup.button2Text.text = "クライアント";
            //button(サーバ)が押されたら以下を実行
            popup.button.onClickEvent.AddListener(() => {
                //falseにすることで対人戦であることを定義
                GameNetworkManager.Instance.isSinglePlayer = false;
                CreateGame();
            });
            popup.button2.onClickEvent.AddListener(() => {
                popup.Close(); });
        });

        //SceneManager.LoadScene("SelectHero");
    }

    public void OnDecksButtonPressed()
    {
        SceneManager.LoadScene("DeckBuilder");
    }

    public void OnQuitButtonPressed()
    {
        OpenPopup<PopupTwoButtons>("PopupTwoButtons", popup =>
        {
            popup.text.text = "ゲームを終了しますか?";
            popup.buttonText.text = "はい";
            popup.button2Text.text = "いいえ";
            popup.button.onClickEvent.AddListener(() => {
                Application.Quit(); });
            popup.button2.onClickEvent.AddListener(() => {
                popup.Close(); });
        });
    }

    private void CreateGame()
    {
        OpenPopup<PopupLoading>("PopupLoading", popup =>
        {
            popup.text.text = "対戦相手を探しています";
        });
        var gameName = gameNameInputField.text;
        if (string.IsNullOrEmpty(gameName))
        {
            gameName = "New game";
        }
        var password = passwordInputField.text;
#if ENABLE_MASTER_SERVER_KIT
        ClientAPI.CreateGameRoom(gameName, 2, password, (ip, port) =>
        {
            ClientAPI.JoinGameServer(ip, port);
        },
        error =>
        {
        });
#else
        GameNetworkManager.Instance.matchMaker.CreateMatch(gameName, 2, true, password, string.Empty, string.Empty, 0, 0, OnMatchCreate);
#endif
    }

    private void OnMatchCreate(bool success, string extendedInfo, MatchInfo responseData)
    {
        if (success)
        {
            GameNetworkManager.Instance.OnMatchCreate(success, extendedInfo, responseData);
        }
        else
        {
            ClosePopup();
            OpenPopup<PopupOneButton>("PopupOneButton", popup =>
            {
                popup.text.text = "The game could not be created.";
                popup.buttonText.text = "OK";
                popup.button.onClickEvent.AddListener(() => { popup.Close(); });
            });
        }
    }

}
