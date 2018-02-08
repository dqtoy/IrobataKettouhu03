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

	[SerializeField]
    public static bool koumaFlag = false;
	 [SerializeField]
    public static bool hakugyokuFlag = false;
		 [SerializeField]
    public static bool eienFlag = false;
	[SerializeField]
    public static bool tfFlag = false;

	[SerializeField]
	public static string flag;

	Button kButton;
	Button hButton;
	Button eButton;

	// Use this for initialization
	void Start () {

	SoundController.setloopDefine=5.454f;
    SoundController.setendDefine=101.818f;
	//BGM再生。AUDIO.BGM_BATTLEがBGMのファイル名
	SoundController.Instance.PlayBGM ("GS-premaster",SoundController.BGM_FADE_SPEED_RATE_HIGH);
	SoundController.Instance.ChangeVolume (0.2F,0.2F);
		
	}

	    public void OnKoumaButtonPressed()
    {
		bool FlagCheck = checkFlag();
		if(FlagCheck){
			koumaFlag=true;
			hakugyokuFlag=false;
			eienFlag=false;
		}
	}

	    public void OnhakugyokuButtonPressed()
    {
		bool FlagCheck = checkFlag();
		if(FlagCheck){
			koumaFlag=false;
			hakugyokuFlag=true;
			eienFlag=false;
		}
	}

	public void OneienButtonPressed()
    {
		bool FlagCheck = checkFlag();
		if(FlagCheck){
			koumaFlag=false;
			hakugyokuFlag=false;
			eienFlag=true;
		}
	}
	
	[SerializeField]
	public bool checkFlag(){

		if(koumaFlag||hakugyokuFlag||eienFlag){
			tfFlag=true;
		}else{
			tfFlag=false;			
		}

		return tfFlag;

	}
	// Update is called once per frame
	void Update () {
		
	}
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

	[SerializeField]
	static public string GetTeamFlag(){
		if(koumaFlag){
			flag="kouma";
		}else if(hakugyokuFlag){
			flag="hakugyoku";
		}else if(eienFlag){
			flag="eien";
		}
		return flag;
	}

	    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("DeckBuilder");
    }
	    public void OnNextButtonPressed()
    {
        SceneManager.LoadScene("DeckBuilder");
    }

}
