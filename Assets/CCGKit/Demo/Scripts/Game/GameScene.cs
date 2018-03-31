
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

using CCGKit;

/// <summary>
/// This classes manages the game scene.
/// このクラスはゲームシーンを管理します。
/// </summary>
public class GameScene : BaseScene
{
    public PopupChat chatPopup;

    public Sprite OpponentAvaterImage;

    public Sprite PlayerAvaterImage;

    protected GameUI gameUI;

    

    private void Start()
    {

        

//        SelectHeroScene shc = new SelectHeroScene();
//        bool rFlag = shc.GetRenkoNameFlag();

        //BGM再生。AUDIO.BGM_BATTLEがBGMのファイル名
        SoundController.setloopDefine=18.181f;
        SoundController.setendDefine=90.909f;        
        SoundController.Instance.PlayBGM ("PB-premaster",SoundController.BGM_FADE_SPEED_RATE_HIGH);
    	SoundController.Instance.ChangeVolume (0.2F,0.2F);

        //ゲーム画面の生成
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();

        
        string playerName = gameUI.GetPlayerName();
        string oppornentName = gameUI.GetOpponentName();
        if(playerName.Equals("宇佐見蓮子")){
            SpriteRenderer rendererRenko = gameObject.GetComponent<SpriteRenderer>();
            rendererRenko.sprite = OpponentAvaterImage;
        }
        
        if(oppornentName.Equals("マエリベリー・ハーン")){
            SpriteRenderer rendererMery = gameObject.GetComponent<SpriteRenderer>();
            rendererMery.sprite = PlayerAvaterImage;
        }
/*
        OpenPopup<PopupLoading>("PopupLoading", popup =>
        {

//            popup.text.text = "Waiting for game to start...";
            popup.text.text = "ゲーム開始まで待機中…";
        });
*/

        if (GameNetworkManager.Instance.isSinglePlayer)
        {
            //AddBotメソッドを1.5秒後呼び出す
            Invoke("AddBot", 1.5f);
        }
        
//        CloseWaitingWindow();
    }

    private void AddBot()
    {
        //使い方：public static bool AddPlayer(short playerControllerId);
        //playerControllerId：ローカル プレイヤー ID(そのPC上でプレイヤーを操作しているユーザのこと)
        //この場合、AIを呼び出している
        ClientScene.AddPlayer(1);
    }

    public void CloseWaitingWindow()
    {
        //BaseSceneのメソッド
        ClosePopup();
    }

    /// <summary>
    /// Callback for when the end turn button is pressed.
    /// ターン終了ボタンが押されたときのコールバック。
    /// </summary>
    public void OnEndTurnButtonPressed()
    {
        var localPlayer = NetworkingUtils.GetLocalPlayer() as DemoHumanPlayer;
        if (localPlayer != null)
        {
            /*
            var maxHandSize = GameManager.Instance.Config.properties.maxHandSize;
            if (localPlayer.HandSize > maxHandSize)
            {
                var diff = localPlayer.HandSize - maxHandSize;
                if (diff == 1)
                    WindowUtils.OpenAlertDialog("You need to discard " + diff + " card from your hand.");
                else
                    WindowUtils.OpenAlertDialog("You need to discard " + diff + " cards from your hand.");
            }
            */
            localPlayer.StopTurn();
        }
    }

    /// <summary>
    /// Callback for when the exit game button is pressed.
    /// ゲーム終了ボタンが押されたときのコールバック。
    /// </summary>
    public void OnExitGameButtonPressed()
    {
        OpenPopup<PopupTwoButtons>("PopupTwoButtons", popup =>
        {
            popup.text.text = "ゲームを終了しますか？";
            popup.buttonText.text = "はい";
            popup.button2Text.text = "いいえ";


            popup.button.onClickEvent.AddListener(() =>
            {
                
                if (NetworkingUtils.GetLocalPlayer().isServer)
                {
                    GameNetworkManager.Instance.StopHost();
                }
                else
                {
                    GameNetworkManager.Instance.StopClient();
                }

            //追記

                SceneManager.LoadScene("Home");
            });
            
            
            popup.button2.onClickEvent.AddListener(() => { popup.Close(); });
        });
    }

    /// <summary>
    /// Callback for when the chat button is pressed.
    /// チャットボタンが押されたときのコールバック。
    /// </summary>
    public void OnChatButtonPressed()
    {
        chatPopup.Show();
    }
}
