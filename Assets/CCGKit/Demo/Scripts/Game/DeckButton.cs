using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;

using CCGKit;

public class DeckButton : MonoBehaviour
{
    [SerializeField]
    protected Image activeBackground;

    [SerializeField]
    protected TextMeshProUGUI nameText;

    [SerializeField]
    protected TextMeshProUGUI numCardsText;

    [SerializeField]
    protected TextMeshProUGUI numSpellsText;

    [SerializeField]
    protected TextMeshProUGUI numCreaturesText;

    [HideInInspector]
    public DeckBuilderScene scene;

//    [HideInInspector]
//    public KoumaDeckBuilderScene kScene;
//    [HideInInspector]
//   public HakugyokuDeckBuilderScene hScene;    
//    [HideInInspector]
//    public EienDeckBuilderScene eScene;

/// <summary>
/// 現在のデッキ
/// </summary>
    public Deck deck { get; private set; }

    /// <summary>
    /// クリックしたデッキのカード一覧を表示する
    /// </summary>
    public void OnButtonPressed()
    {
        scene.SetActiveDeck(this);
    }

    public void OnDeleteButtonPressed()
    {
        scene.RemoveDeck(deck);
        Destroy(gameObject);
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            //透明度1.0まで0.5秒掛けてフェード(見た目をアクティブ化)する
            activeBackground.DOFade(1.0f, 0.5f);
        }
        else
        {
            //透明度0.0まで0.2秒掛けてフェード(見た目を非アクティブ化)する
            activeBackground.DOFade(0.0f, 0.2f);
        }
    }
    /// <summary>
    /// 現在操作しているデッキをセットする
    /// </summary>
    /// <param name="deck"></param>
    public void SetDeck(Deck deck)
    {
        this.deck = deck;
        nameText.text = deck.name;
        UpdateDeckInfo();
    }

    public void SetDeckName(string name)
    {
        deck.name = name;
        nameText.text = name;
    }
    /// <summary>
    /// クリックした構築済みデッキの情報に表示を更新する
    /// </summary>
    public void UpdateDeckInfo()
    {
        numCardsText.text = deck.GetNumCards().ToString() + " cards";
        numCreaturesText.text = deck.GetNumCards(GameManager.Instance.config, 0).ToString();
        numSpellsText.text = deck.GetNumCards(GameManager.Instance.config, 1).ToString();
        //sceneはDeckBuilderSceneのインスタンス
        scene.UpdateNumCardsText();
    }
}
