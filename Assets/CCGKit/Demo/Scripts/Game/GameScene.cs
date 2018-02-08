// Copyright (C) 2016-2017 David Pol. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

using CCGKit;

/// <summary>
/// This classes manages the game scene.
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


        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();

        
        string playerName = gameUI.GetPlayerName();
        string oppornentName = gameUI.GetOpponentName();
        if(playerName.Equals("宇佐見蓮子")){
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.sprite = OpponentAvaterImage;
        }
        
        if(oppornentName.Equals("マエリベリー・ハーン")){
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.sprite = PlayerAvaterImage;
        }

//        OpenPopup<PopupLoading>("PopupLoading", popup =>
//        {

//            popup.text.text = "Waiting for game to start...";
//            popup.text.text = "ゲーム開始まで待機中…";
//        });
        if (GameNetworkManager.Instance.isSinglePlayer)
        {
            Invoke("AddBot", 1.5f);
        }
    }

    private void AddBot()
    {
        ClientScene.AddPlayer(1);
    }

    public void CloseWaitingWindow()
    {
        ClosePopup();
    }

    /// <summary>
    /// Callback for when the end turn button is pressed.
    /// </summary>
    public void OnEndTurnButtonPressed()
    {
        var localPlayer = NetworkingUtils.GetLocalPlayer() as DemoHumanPlayer;
        if (localPlayer != null)
        {
            //var maxHandSize = GameManager.Instance.Config.properties.maxHandSize;
            /*if (localPlayer.HandSize > maxHandSize)
            {
                var diff = localPlayer.HandSize - maxHandSize;
                if (diff == 1)
                    WindowUtils.OpenAlertDialog("You need to discard " + diff + " card from your hand.");
                else
                    WindowUtils.OpenAlertDialog("You need to discard " + diff + " cards from your hand.");
            }*/
            localPlayer.StopTurn();
        }
    }

    /// <summary>
    /// Callback for when the exit game button is pressed.
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
    /// </summary>
    public void OnChatButtonPressed()
    {
        chatPopup.Show();
    }
}
