using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using FullSerializer;
using TMPro;

using CCGKit;

public class SelectTeamScene : BaseScene {

    [SerializeField]
    private GameObject SelectTeamGroup;

    /*
	[SerializeField]
    public static bool koumaFlag = false;
	 [SerializeField]
    public static bool hakugyokuFlag = false;
		 [SerializeField]
    public static bool eienFlag = false;
    */

    public enum Team : int { kouma = 0, hakugyoku, eien, neutral, all = 100, };

    [SerializeField]
    public static bool tfFlag = false;


    //選ばれていないときは全て選択
    [SerializeField]
    public static int selectTeam = (int)Team.all;


    [SerializeField]
	public static string flag;

    [SerializeField]
    public static int BGMflag;

    public Button kButton;
	public Button hButton;
	public Button eButton;

    // Use this for initialization
    private void Start () {

        BGMflag = 0;

        //選択したヒーローの情報を保持しておきたいので、このオブジェクトは消さない
        DontDestroyOnLoad(this);

        //フェードインから開始
        FadeScript fadeout = GameObject.Find("fadein_out_panel").GetComponent<FadeScript>();
        fadeout.InitIn();
        fadeout.isFadeIn = true;

        SoundController.setloopDefine = 0.493f;
        SoundController.setendDefine = 87.767f;
        //BGM再生。AUDIO.BGM_BATTLEがBGMのファイル名
        SoundController.Instance.PlayBGM("DeckBuild", SoundController.BGM_FADE_SPEED_RATE_HIGH);
        SoundController.Instance.ChangeVolume(0.2F, 0.2F);

    }

    
    // Update is called once per frame
    private void Update()
    {
        
        if (BGMflag < 1) {
            SoundController.setloopDefine = 0.493f;
            SoundController.setendDefine = 87.767f;
            //BGM再生。AUDIO.BGM_BATTLEがBGMのファイル名
            SoundController.Instance.PlayBGM("DeckBuild", SoundController.BGM_FADE_SPEED_RATE_HIGH);
                SoundController.Instance.ChangeVolume(0.2F, 0.2F);
                BGMflag++;
        }
        

    }
    
    public void OnKoumaButtonPressed()
    {
        selectTeam = (int)Team.kouma;
        //ボタンが押されたのでデッキビルドに行けるようにする
        tfFlag = true;
    }

	    public void OnhakugyokuButtonPressed()
    {
        selectTeam = (int)Team.hakugyoku;
        //ボタンが押されたのでデッキビルドに行けるようにする
        tfFlag = true;
    }

	public void OneienButtonPressed()
    {
        selectTeam = (int)Team.eien;
        //ボタンが押されたのでデッキビルドに行けるようにする
        tfFlag = true;
    }
	
    /*
    public void OnGoSelectClassButtonPressed()
    {
		if(koumaFlag==false){

		}
		if(koumaFlag==true){
        SceneManager.LoadScene("KoumaDeckBuildScene");
		}else if(hakugyokuFlag==true){
        SceneManager.LoadScene("HakugyokuDeckBuildScene");
		}else if(eienFlag==true){
        SceneManager.LoadScene("EienDeckBuildScene");
		}
	}
    */

	[SerializeField]
	static public int GetTeamFlag()
    {
        return selectTeam;
	}

	    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("DeckBuilder");
        tfFlag = false;
    }
	    public void OnNextButtonPressed()
    {
        SceneManager.LoadScene("DeckBuilder");
    }

}
