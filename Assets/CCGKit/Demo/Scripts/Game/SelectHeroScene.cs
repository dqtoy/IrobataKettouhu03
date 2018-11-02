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
public class SelectHeroScene : BaseScene {


    [SerializeField]
    private GameObject SelectHeroGroup;
    [SerializeField]
    public static bool renkoFlag = false;
	 [SerializeField]
    public static bool merryFlag = false;

	public Button rButton;
	public Button mButton;


/*
   private void Awake()
    {
        Assert.IsNotNull(SelectHeloGroup);

    }
 */

	// Use this for initialization
	public void Start () {

        //フェードインから開始
        FadeScript fadeout = GameObject.Find("fadein_out_panel").GetComponent<FadeScript>();
        fadeout.InitIn();
        fadeout.isFadeIn = true;

        SoundController.setloopDefine=5.454f;
        SoundController.setendDefine=101.818f;
    	//BGM再生。AUDIO.BGM_BATTLEがBGMのファイル名
	    SoundController.Instance.PlayBGM ("GS-premaster",SoundController.BGM_FADE_SPEED_RATE_HIGH);
    	SoundController.Instance.ChangeVolume (0.2F,0.2F);




//		rButton     = GameObject.Find ("/Canvas/RenkoButton").GetComponent<Button> ();
//		mButton   = GameObject.Find ("/Canvas/MerryButton").GetComponent<Button> ();

//		rButton.Select();

	}

    public void OnRenkoButtonPressed()
    {
		if(renkoFlag==false&&merryFlag==false){
			renkoFlag=true;
			merryFlag=false;
		}else if(renkoFlag==true&&merryFlag==false){
			renkoFlag=true;
			merryFlag=false;
		}else if(renkoFlag==false&&merryFlag==true){
			renkoFlag=true;
			merryFlag=false;
		}
//		SetRenkoNameFlag();
	}

	public void OnMerryButtonPressed()
    {
		if(renkoFlag==false&&merryFlag==false){
			renkoFlag=false;
			merryFlag=true;
		}else if(renkoFlag==true&&merryFlag==false){
			renkoFlag=false;
			merryFlag=true;
		}else if(renkoFlag==false&&merryFlag==true){
			renkoFlag=false;
			merryFlag=true;
		}
//		SetRenkoNameFlag();
	}



	    public void OnGoSelectClassButtonPressed()
    {
		if(renkoFlag==true||merryFlag==true){
        SceneManager.LoadScene("SelectDeck");
		}
	}

	    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("Home");
    }


	public void SetRenkoNameFlag(){
//		this.renkoFlag=renkoFlag;
	}

	public bool GetRenkoNameFlag(){
		bool flag = renkoFlag;
		return flag;
	}



	// Update is called once per frame
	void Update () {
		
	}
}
