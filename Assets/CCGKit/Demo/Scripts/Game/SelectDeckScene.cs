using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using CCGKit;

using DG.Tweening;
using FullSerializer;
using TMPro;
public class SelectDeckScene : BaseScene {

    [SerializeField]
    private CanvasGroup settingsGroup;

    [SerializeField]
    private CanvasGroup createGameGroup;

    [SerializeField]
    private CanvasGroup findGamesGroup;

    [SerializeField]
    private TextMeshProUGUI currentDeckNameText;

    [SerializeField]
    private TextMeshProUGUI currentAIDeckNameText;

    private List<Deck> decks;
    private int currentDeckIndex;
	private int currentAIDeckIndex;

	private fsSerializer serializer = new fsSerializer();

	[SerializeField]
    private TMP_InputField playerNameInputField;

    [SerializeField]
    private TMP_InputField passwordInputField;

	[SerializeField]
    private Toggle passwordToggle;

	// Use this for initialization
	void Start () {
        SoundController.setloopDefine=5.454f;
        SoundController.setendDefine=101.818f;
	    //BGM再生。AUDIO.BGM_BATTLEがBGMのファイル名
	    SoundController.Instance.PlayBGM ("GS-premaster",SoundController.BGM_FADE_SPEED_RATE_HIGH);
    	SoundController.Instance.ChangeVolume (0.2F,0.2F);

		        var defaultDeckTextAsset = Resources.Load<TextAsset>("DefaultDeck");
        if (defaultDeckTextAsset != null)
        {
            GameManager.Instance.defaultDeck = JsonUtility.FromJson<Deck>(defaultDeckTextAsset.text);
        }

        var decksPath = Application.persistentDataPath + "/decks.json";
        if (File.Exists(decksPath))
        {
            var file = new StreamReader(decksPath);
            var fileContents = file.ReadToEnd();
            var data = fsJsonParser.Parse(fileContents);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(List<Deck>), ref deserialized).AssertSuccessWithoutWarnings();
            file.Close();
            decks = deserialized as List<Deck>;
        }

//        playerNameInputField.text = PlayerPrefs.GetString("player_name");
        if (decks != null && decks.Count > 0)
        {
            currentDeckIndex = PlayerPrefs.GetInt("default_deck");
            if (currentDeckIndex < decks.Count)
            {
                currentDeckNameText.text = decks[currentDeckIndex].name;
                PlayerPrefs.SetInt("default_deck", currentDeckIndex);
            }
            else
            {
                currentDeckNameText.text = decks[0].name;
                PlayerPrefs.SetInt("default_deck", 0);
            }
            currentAIDeckIndex = PlayerPrefs.GetInt("default_ai_deck");
            if (currentAIDeckIndex < decks.Count)
            {
                currentAIDeckNameText.text = decks[currentAIDeckIndex].name;
                PlayerPrefs.SetInt("default_ai_deck", currentAIDeckIndex);
            }
            else
            {
                currentAIDeckNameText.text = decks[0].name;
                PlayerPrefs.SetInt("default_ai_deck", 0);
            }
        }

#if !ENABLE_MASTER_SERVER_KIT
//        GameNetworkManager.Instance.StartMatchMaker();
#endif

 //       passwordInputField.gameObject.SetActive(passwordToggle.isOn);

//        OnSettingsButtonPressed();

		
	}
	
    public void OnSinglePlayerButtonPressed()
    {
        GameNetworkManager.Instance.isSinglePlayer = true;
        GameNetworkManager.Instance.StartHost();
    }

    public void OnPrevDeckButtonPressed()
    {
        if (decks != null && decks.Count > 0)
        {
            --currentDeckIndex;
            if (currentDeckIndex < 0)
            {
                currentDeckIndex = 0;
            }
            if (currentDeckIndex < decks.Count)
            {
                currentDeckNameText.text = decks[currentDeckIndex].name;
                PlayerPrefs.SetInt("default_deck", currentDeckIndex);
            }
        }
    }



    public void OnNextDeckButtonPressed()
    {
        if (decks != null && decks.Count > 0)
        {
            ++currentDeckIndex;
            if (currentDeckIndex == decks.Count)
            {
                currentDeckIndex = currentDeckIndex - 1;
            }
            if (currentDeckIndex < decks.Count)
            {
                currentDeckNameText.text = decks[currentDeckIndex].name;
                PlayerPrefs.SetInt("default_deck", currentDeckIndex);
            }
        }
    }

	    public void OnPrevAIDeckButtonPressed()
    {
        if (decks != null && decks.Count > 0)
        {
            --currentAIDeckIndex;
            if (currentAIDeckIndex < 0)
            {
                currentAIDeckIndex = 0;
            }
            if (currentAIDeckIndex < decks.Count)
            {
                currentAIDeckNameText.text = decks[currentAIDeckIndex].name;
                PlayerPrefs.SetInt("default_ai_deck", currentAIDeckIndex);
            }
        }
    }

    public void OnNextAIDeckButtonPressed()
    {
        if (decks != null && decks.Count > 0)
        {
            ++currentAIDeckIndex;
            if (currentAIDeckIndex == decks.Count)
            {
                currentAIDeckIndex = currentAIDeckIndex - 1;
            }
            if (currentAIDeckIndex < decks.Count)
            {
                currentAIDeckNameText.text = decks[currentAIDeckIndex].name;
                PlayerPrefs.SetInt("default_ai_deck", currentAIDeckIndex);
            }
        }
    }

    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("SelectHero");
    }

    public void OnSettingsButtonPressed()
    {
        HideGroup(findGamesGroup);
        HideGroup(createGameGroup);
        ShowGroup(settingsGroup);
    }

    private void ShowGroup(CanvasGroup group)
    {
        group.DOFade(1.0f, 0.4f);
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    private void HideGroup(CanvasGroup group)
    {
        group.DOFade(0.0f, 0.2f);
        group.interactable = false;
        group.blocksRaycasts = false;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
