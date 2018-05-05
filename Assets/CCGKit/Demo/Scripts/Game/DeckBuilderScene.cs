using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

using FullSerializer;
using TMPro;

using CCGKit;

public class DeckBuilderScene : BaseScene
{
    public List<Transform> cardPositions;

    [SerializeField]
    private GameObject creatureCardViewPrefab;

    [SerializeField]
    private GameObject spellCardViewPrefab;

    //デッキの新規作成ボタン(ScrollViewDeckList内の+ボタン)
    [SerializeField]
    private GameObject deckListContent;

    [SerializeField]
    private GameObject deckListItemPrefab;

    [SerializeField]
    private GameObject deckListAddItemPrefab;

    [SerializeField]
    private TextMeshProUGUI pageText;

    [SerializeField]
    private TMP_InputField deckNameInputField;

    [SerializeField]
    private GameObject cardListContent;

    [SerializeField]
    private GameObject cardListItemPrefab;

    [SerializeField]
    private TextMeshProUGUI numCardsText;

    //新規デッキ作成ボタン
    private GameObject createDeckItem;

    private DeckButton currentDeckButton;

    private fsSerializer serializer = new fsSerializer();

    private bool goSelectTeam = false;

    private int numPages;
    private int currentPage;

    [SerializeField]
    private int maxDeckSize=30;

    private void Awake()
    {
        Assert.IsNotNull(creatureCardViewPrefab);
        Assert.IsNotNull(spellCardViewPrefab);
        Assert.IsNotNull(deckListContent);
        Assert.IsNotNull(deckListItemPrefab);
        Assert.IsNotNull(deckListAddItemPrefab);
        Assert.IsNotNull(pageText);
        Assert.IsNotNull(deckNameInputField);
        Assert.IsNotNull(cardListContent);
        Assert.IsNotNull(cardListItemPrefab);
        Assert.IsNotNull(numCardsText);
    }
    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {

//        System.Threading.Thread.Sleep(1000);

        //フェードインから開始
        FadeScript fadeout = GameObject.Find("fadein_out_panel").GetComponent<FadeScript>();
        fadeout.InitIn();
        fadeout.isFadeIn = true;

        //BGMのループ地点指定
        SoundController.setloopDefine = 0.493f;
        SoundController.setendDefine = 87.767f;
        //BGM再生。AUDIO.BGM_BATTLEがBGMのファイル名
        SoundController.Instance.PlayBGM ("DeckBuild", SoundController.BGM_FADE_SPEED_RATE_HIGH);
    	SoundController.Instance.ChangeVolume (0.2F,0.2F);


        //createDeckItem(新規デッキ作成ボタン)をprefab化してsecneと紐付ける
        // CreateDeckButton.sceneは新規にデッキ作成の機能を持ってる
        createDeckItem = Instantiate(deckListAddItemPrefab) as GameObject;
        createDeckItem.transform.SetParent(deckListContent.transform, false);
        createDeckItem.GetComponent<CreateDeckButton>().scene = this;

        //「0ページ目のカードを読み込む」だが、実態は初期化処理。
        //ページに何枚表示するか、全てのカードの総数、カード種別の一覧を読み込んでいる
        LoadCards(0);
        //CeilToIntは小数点切り上げ＆整数化
        //ページ数の算出。カードの数をCardPlaceholderの数(8)で割る
        //8はCountプロパティで取得
        numPages = Mathf.CeilToInt(GameManager.Instance.config.GetNumCards() / (float)cardPositions.Count);
        pageText.text = "Page " + (currentPage + 1) + "/" + numPages;

        //List(playerDecks)(プレイヤーが既に構築しているデッキ)の要素全てを対象に処理する
        //foreach(変数宣言 in 配列名)配列中の各要素に対して1回ずつ処理が行われる。
        foreach (var deck in GameManager.Instance.playerDecks)
        {
            //deckListItemPrefabをコピー(prefab(インスタンス)化)
            var go = Instantiate(deckListItemPrefab) as GameObject;
            go.transform.SetParent(deckListContent.transform, false);
            createDeckItem.transform.SetAsLastSibling();
            //secneと紐付ける
            go.GetComponent<DeckButton>().scene = this;
            //プレイヤーが既に構築しているデッキを現在のデッキとしてセットする
            go.GetComponent<DeckButton>().SetDeck(deck);
        }
        //ゲームオブジェクト(DeckButton型deckListContentオブジェクト(デッキ一覧の+のとこ))の参照
        var firstDeckButton = deckListContent.GetComponentInChildren<DeckButton>();
        if (firstDeckButton != null)
        {
            SetActiveDeck(firstDeckButton);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("Home");
    }
    /// <summary>
    /// 新規作成ボタンを謳歌した時の処理
    /// </summary>
    public void OnCreateDeckButtonPressed()
    {
        CreateNewDeck();
        SceneManager.LoadScene("SelectTeam");
    }
    /// <summary>
    /// 新しいデッキを作成する
    /// </summary>
    public void CreateNewDeck()
    {
        var go = Instantiate(deckListItemPrefab) as GameObject;
        go.transform.SetParent(deckListContent.transform, false);
        //createDeckItemの表示を最前面にする
        createDeckItem.transform.SetAsLastSibling();
        go.GetComponent<DeckButton>().scene = this;

        var deckButton = go.GetComponent<DeckButton>();
        var deck = new Deck();
        GameManager.Instance.playerDecks.Add(deck);
        deckButton.SetDeck(deck);
        SetActiveDeck(deckButton);
    }
    /// <summary>
    /// 
    /// </summary>
    public void OnDeckNameInputFieldEndedEdit()
    {
        currentDeckButton.SetDeckName(deckNameInputField.text);
    }

    /// <summary>
    /// デッキ一覧の中から指定したデッキを現在のデッキに表示する
    /// </summary>
    /// <param name="button"></param>
    public void SetActiveDeck(DeckButton button)
    {
        if (currentDeckButton != null)
        {
            currentDeckButton.SetActive(false);
        }
        currentDeckButton = button;
        //ボタンの見た目を活性化する
        currentDeckButton.SetActive(true);

        //デッキの名前を既存のデッキ名に設定
        deckNameInputField.text = currentDeckButton.deck.name;

        //foreach(変数宣言 in 配列名)CardListItem中の各要素に対して1回ずつ処理が行われる。
        foreach (var item in cardListContent.GetComponentsInChildren<CardListItem>())
        {
            Destroy(item.gameObject);
        }
        //デッキ内のカード全てに対して実行
        foreach (var card in currentDeckButton.deck.cards)
        {
            //全てのカードデータを読み込む
            var libraryCard = GameManager.Instance.config.GetCard(card.id);
            //↓以下2行、Instantiate(cardListItemPrefab,cardListContent.transform, false)と同義
            var go = Instantiate(cardListItemPrefab) as GameObject;
            go.transform.SetParent(cardListContent.transform, false);
            //↑ここまで↑

            //全てのGameObjectの中からCardListItemを検索し、それぞれの変数に代入する
            //現在のデッキボタン情報を取得
            go.GetComponent<CardListItem>().deckButton = currentDeckButton;
            //構築中のデッキを表現する
            go.GetComponent<CardListItem>().card = libraryCard;
            go.GetComponent<CardListItem>().cardNameText.text = libraryCard.name;
            go.GetComponent<CardListItem>().cardAmountText.text = "x" + card.amount.ToString();
            go.GetComponent<CardListItem>().count = card.amount;
            //costに対し全検索をかけて、最初にPayResourceCost型に一致したx(マナの値)を返す
            var cost = libraryCard.costs.Find(x => x is PayResourceCost);
            if (cost != null)
            {
                var payResourceCost = cost as PayResourceCost;
                var manaCost = payResourceCost.value;
                go.GetComponent<CardListItem>().cardCostText.text = manaCost.ToString();
            }
        }
        //カードの枚数を表示
        UpdateNumCardsText();
    }
    /// <summary>
    /// 前のページに戻る
    /// </summary>
    public void OnPrevPageButtonPressed()
    {
        --currentPage;
        if (currentPage < 0)
        {
            currentPage = 0;
        }
        pageText.text = "Page " + (currentPage + 1) + "/" + numPages;
        LoadCards(currentPage);
    }
    /// <summary>
    /// 次のページに進む
    /// </summary>
    public void OnNextPageButtonPressed()
    {
        ++currentPage;
        if (currentPage == numPages)
        {
            currentPage = numPages - 1;
        }
        pageText.text = "Page " + (currentPage + 1) + "/" + numPages;
        LoadCards(currentPage);
    }
    /// <summary>
    /// カード一覧を読み込む
    /// </summary>
    /// <param name="page":カード一覧を読み込みたいページ></param>
    public void LoadCards(int page)
    {
        //初期化
        var gameConfig = GameManager.Instance.config;
        //ページ数✕カードポジションを取得する
        //→現在のページの1枚目のカードを読み込み
        var startIndex = page * cardPositions.Count;
        //2つ以上の値(現在のページの一枚目のカードのIDの数に表示されるべきカードの枚数を足したものか、カードの総数か)から最小値を返す
        var endIndex = Mathf.Min(startIndex + cardPositions.Count, gameConfig.GetNumCards());

        foreach (var card in FindObjectsOfType<CardView>())
        {
            Destroy(card.gameObject);
        }

        for (var i = startIndex; i < endIndex; i++)
        {
            var card = gameConfig.cards[i];
            var cardType = gameConfig.cardTypes.Find(x => x.id == card.cardTypeId);
            GameObject go = null;
            if (cardType.name == "Creature")
            {
                go = Instantiate(creatureCardViewPrefab as GameObject);
            }
            else if (cardType.name == "Spell")
            {
                go = Instantiate(spellCardViewPrefab as GameObject);
            }
            var cardView = go.GetComponent<CardView>();
            cardView.PopulateWithLibraryInfo(card);
            cardView.SetHighlightingEnabled(false);
            cardView.transform.position = cardPositions[i % cardPositions.Count].position;
            cardView.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            cardView.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
            cardView.GetComponent<SpriteRenderer>().sortingOrder = 1;
            cardView.GetComponent<SortingGroup>().sortingLayerName = "Default";
            cardView.GetComponent<SortingGroup>().sortingOrder = 1;

            var deckBuilderCard = go.AddComponent<DeckBuilderCard>();
            deckBuilderCard.scene = this;
            deckBuilderCard.card = card;
        }
    }



    /// <summary>
    /// トークン一覧を読み込む
    /// </summary>
    /// <param name="page":カード一覧を読み込みたいページ></param>
    public void LoadTokens(int page)
    {
        /*

        //初期化
        var gameConfig = GameManager.Instance.config;
        //ページ数✕カードポジションの数(8個)を取得する
        //→現在のページのカード数を読み込み
        var startIndex = page * cardPositions.Count;
        //2つ以上の値(カードポジションの8か、今のページに表示されてるカード枚数か)から最小値を返す
        var endIndex = Mathf.Min(startIndex + cardPositions.Count, gameConfig.GetNumCards());

        foreach (var card in FindObjectsOfType<CardView>())
        {
            Destroy(card.gameObject);
        }

        for (var i = startIndex; i < endIndex; i++)
        {
            //カードリストからカードを一枚読み込む
            var card = gameConfig.cards[i];
            var cardType = gameConfig.cardTypes.Find(x => x.id == card.cardTypeId);
            GameObject go = null;
            if (cardType.name == "Creature")
            {
                go = Instantiate(creatureCardViewPrefab as GameObject);
            }
            else if (cardType.name == "Spell")
            {
                go = Instantiate(spellCardViewPrefab as GameObject);
            }
            var cardView = go.GetComponent<CardView>();
            cardView.PopulateWithLibraryInfo(card);
            cardView.SetHighlightingEnabled(false);
            cardView.transform.position = cardPositions[i % cardPositions.Count].position;
            cardView.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            cardView.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
            cardView.GetComponent<SpriteRenderer>().sortingOrder = 1;
            cardView.GetComponent<SortingGroup>().sortingLayerName = "Default";
            cardView.GetComponent<SortingGroup>().sortingOrder = 1;

            var deckBuilderCard = go.AddComponent<DeckBuilderCard>();
            deckBuilderCard.scene = this;
            deckBuilderCard.card = card;
        }

        */
    }


    /// <summary>
    /// デッキにカードを追加する処理
    /// </summary>
    /// <param name="card"></param>
    public void AddCardToDeck(Card card)
    {
        if (currentDeckButton == null)
        {
            return;
        }

        var existingCards = currentDeckButton.deck.cards.Find(x => x.id == card.id);
        var maxCopies = card.GetIntProperty("MaxCopies");
        if (existingCards != null && existingCards.amount == maxCopies)
        {
            OpenPopup<PopupOneButton>("PopupOneButton", popup =>
            {
                popup.text.text = "カード枚数が上限に達しました";
                popup.buttonText.text = "閉じる";
                popup.button.onClickEvent.AddListener(() => { popup.Close(); });
            });
            return;
        }

        var itemFound = false;
        foreach (var item in cardListContent.GetComponentsInChildren<CardListItem>())
        {
            if (item.card == card)
            {
                itemFound = true;
                item.AddCard();
                break;
            }
        }

        if (!itemFound)
        {
            var go = Instantiate(cardListItemPrefab) as GameObject;
            go.transform.SetParent(cardListContent.transform, false);
            go.GetComponent<CardListItem>().deckButton = currentDeckButton;
            go.GetComponent<CardListItem>().card = card;
            go.GetComponent<CardListItem>().cardNameText.text = card.name;
            var cost = card.costs.Find(x => x is PayResourceCost);
            if (cost != null)
            {
                var payResourceCost = cost as PayResourceCost;
                var manaCost = payResourceCost.value;
                go.GetComponent<CardListItem>().cardCostText.text = manaCost.ToString();
            }
        }

        //デッキが30枚だったら警告を出してカードを追加しない
        var config = GameManager.Instance.config;
        var maxDeckSize = config.properties.maxDeckSize;
        var currentDeck = currentDeckButton.deck;


            var numCards = currentDeck.GetNumCards();
            if (numCards == maxDeckSize)
            {
                OpenPopup<PopupOneButton>("PopupOneButton", popup =>
                {
                    popup.text.text = "デッキを構築するカードの総数が30枚を超えています";
                    popup.buttonText.text = "閉じる";
                    popup.button.onClickEvent.AddListener(() => { popup.Close(); });
                });
                return;
            }
            else {
                 currentDeckButton.deck.AddCard(card);
                 currentDeckButton.UpdateDeckInfo();
        }


        //編集ここまで
        //currentDeckButton.deck.AddCard(card);
        //currentDeckButton.UpdateDeckInfo();
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnClearAllButtonPressed()
    {
        currentDeckButton.deck.cards.Clear();
        foreach (var item in cardListContent.GetComponentsInChildren<CardListItem>())
        {
            Destroy(item.gameObject);
        }
        currentDeckButton.UpdateDeckInfo();
    }

    /// <summary>
    /// 値をStringで返す
    /// </summary>
    public void UpdateNumCardsText()
    {
        if (currentDeckButton != null)
        {
            numCardsText.text = currentDeckButton.deck.GetNumCards().ToString();
        }
    }
    /// <summary>
    /// デッキを削除
    /// </summary>
    /// <param name="deck"></param>
    public void RemoveDeck(Deck deck)
    {
        GameManager.Instance.playerDecks.Remove(deck);
        foreach (var item in cardListContent.GetComponentsInChildren<CardListItem>())
        {
            Destroy(item.gameObject);
        }
    }
    /// <summary>
    /// デッキ構築完了後、saveボタン押した時の挙動。
    /// デッキ情報を格納したjsonファイルを作成する。
    /// </summary>
    public void OnDoneButtonPressed()
    {
        var config = GameManager.Instance.config;
        var minDeckSize = config.properties.minDeckSize;
        var maxDeckSize = config.properties.maxDeckSize;
        foreach (var deck in GameManager.Instance.playerDecks)
        {
            var numCards = deck.GetNumCards();
            if (numCards < minDeckSize)
            {
                OpenPopup<PopupOneButton>("PopupOneButton", popup =>
                {
                    popup.text.text = "デッキを構築するカードの総数が30枚に達していません";
                    popup.buttonText.text = "閉じる";
                    popup.button.onClickEvent.AddListener(() => { popup.Close(); });
                });
                return;
            }
            else if (numCards > maxDeckSize)
            {
                OpenPopup<PopupOneButton>("PopupOneButton", popup =>
                {
                    popup.text.text = "デッキを構築するカードの総数が30枚を超えています";
                    popup.buttonText.text = "閉じる";
                    popup.button.onClickEvent.AddListener(() => { popup.Close(); });
                });
                return;
            }
        }

        var decksPath = Application.persistentDataPath + "/decks.json";
//        Debug.Log(decksPath);
        fsData serializedData;
        serializer.TrySerialize(GameManager.Instance.playerDecks, out serializedData).AssertSuccessWithoutWarnings();
        var file = new StreamWriter(decksPath);
        var json = fsJsonPrinter.PrettyJson(serializedData);
        file.WriteLine(json);
        file.Close();
    }
}
