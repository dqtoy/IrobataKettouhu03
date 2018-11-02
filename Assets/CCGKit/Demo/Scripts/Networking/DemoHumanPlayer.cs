using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.Rendering;

using DG.Tweening;
using TMPro;

using CCGKit;
using UnityEngine.SceneManagement;

/// <summary>
/// The demo player is a subclass of the core HumanPlayer type which extends it with demo-specific
/// functionality. Most of which is straightforward updating of the user interface when receiving
/// new state from the server.
/// デモプレーヤーは、デモ固有の機能を拡張したコアHumanPlayerタイプのサブクラスです。 その大部分は、サーバーから新しい状態を受け取ったときのユーザーインターフェイスの直接的な更新です。
/// </summary>
public class DemoHumanPlayer : DemoPlayer
{
    [SerializeField]
    private GameObject creatureCardViewPrefab;

    [SerializeField]
    private GameObject spellCardViewPrefab;

    [SerializeField]
    private GameObject opponentCardPrefab;

    //マリガン用
    [SerializeField]
    private GameObject MulliganCardPrefab;
    //マリガン中かどうか判定
    private bool mulliganFlag = false;


    [SerializeField]
    private GameObject boardCreaturePrefab;

    [SerializeField]
    private GameObject spellTargetingArrowPrefab;

    [SerializeField]
    private GameObject fightTargetingArrowPrefab;

    [SerializeField]
    private GameObject opponentTargetingArrowPrefab;

    //ヒロパ追加
    [SerializeField]
    private GameObject PlayerHeroPowerPrefab;
    [SerializeField]
    private GameObject opponentHeroPowerPrefab;

    protected List<CardView> playerHandCards = new List<CardView>();
    protected List<GameObject> opponentHandCards = new List<GameObject>();
    protected List<BoardCreature> playerBoardCards = new List<BoardCreature>();
    protected List<BoardCreature> opponentBoardCards = new List<BoardCreature>();
    protected List<BoardCreature> playerGraveyardCards = new List<BoardCreature>();
    protected List<BoardCreature> opponentGraveyardCards = new List<BoardCreature>();

    protected BoardCreature currentCreature;
    protected CardView currentSpellCard;

    protected GameUI gameUI;
    protected PopupChat chatPopup;

    protected float accTime;
    protected float secsAccTime;

    public Stat lifeStat { get; protected set; }
    public Stat manaStat { get; protected set; }

    protected Stat opponentLifeStat { get; set; }
    protected Stat opponentManaStat { get; set; }

    private bool SenteGoteFlag;

    public RuntimeZone tokenZone;
    public RuntimeZone deckZone;
    public RuntimeZone handZone;
    public RuntimeZone boardZone;

    //追加
    public RuntimeZone MulliganZone;

    public RuntimeZone graveyardZone;
    public RuntimeZone opponentDeckZone;
    public RuntimeZone opponentHandZone;
    public RuntimeZone opponentBoardZone;
    public RuntimeZone opponentGraveyardZone;

    public bool isCardSelected;
    protected GameObject currentCardPreview;
    protected bool isPreviewActive;
    protected int currentPreviewedCardId;
    protected Coroutine createPreviewCoroutine;

    protected override void Awake()
    {
        base.Awake();


        Assert.IsNotNull(creatureCardViewPrefab);
        Assert.IsNotNull(spellCardViewPrefab);
        Assert.IsNotNull(opponentCardPrefab);
        //追加
        Assert.IsNotNull(MulliganCardPrefab);

        Assert.IsNotNull(boardCreaturePrefab);
        Assert.IsNotNull(spellTargetingArrowPrefab);
        Assert.IsNotNull(fightTargetingArrowPrefab);
        Assert.IsNotNull(opponentTargetingArrowPrefab);

        isHuman = true;
    }

    protected override void Start()
    {
        base.Start();


        //        chatPopup = GameObject.Find("PopupChat").GetComponent<PopupChat>();
        Debug.Log("Start開始");



        // Random クラスの新しいインスタンスを生成する
        System.Random cRandom = new System.Random();
        // 0.0 以上 1.0 以下の乱数を取得する
        double dRandom = cRandom.NextDouble();
        //先手後手フラグ
        if (0 <= dRandom && dRandom < 0.5)
        {
            Debug.Log("先手");
            SenteGoteFlag = true;
        }
        else
        {
            Debug.Log("後手");
            SenteGoteFlag = false;
        }




    }
    /// <summary>
    /// 
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

//        Debug.Log("OnStartLocalPlayer開始");

        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
        Assert.IsNotNull(gameUI);

        foreach (var entry in playerInfo.stats)
        {
            if (entry.Value.name == "Life")
            {
                lifeStat = entry.Value;
            }
            else if (entry.Value.name == "Mana")
            {
                manaStat = entry.Value;
            }
        }
        foreach (var entry in opponentInfo.stats)
        {
            if (entry.Value.name == "Life")
            {
                opponentLifeStat = entry.Value;
            }
            else if (entry.Value.name == "Mana")
            {
                opponentManaStat = entry.Value;
            }
        }

        lifeStat.onValueChanged += (oldValue, newValue) =>
        {
            gameUI.SetPlayerHealth(lifeStat.effectiveValue);
        };
        manaStat.onValueChanged += (oldValue, newValue) =>
        {
            gameUI.SetPlayerMana(manaStat.effectiveValue);
            UpdateHandCardsHighlight();
        };

        opponentLifeStat.onValueChanged += (oldValue, newValue) =>
        {
            gameUI.SetOpponentHealth(opponentLifeStat.effectiveValue);
        };
        opponentManaStat.onValueChanged += (oldValue, newValue) =>
        {
            gameUI.SetOpponentMana(opponentManaStat.effectiveValue);
        };


        //トークンプール作成
        tokenZone = playerInfo.namedZones["TokenPool"];
        tokenZone.onZoneChanged += numCards =>
        {
            gameUI.SetBoardToken(numCards);
        };

        //デッキ作成
        deckZone = playerInfo.namedZones["Deck"];
        deckZone.onZoneChanged += numCards =>
        {
            gameUI.SetPlayerDeckCards(numCards);
        };

        //追加
        //マリガン用
        //        MulliganZone = playerInfo.namedZones["Mulligan"];




        //手札作成
        handZone = playerInfo.namedZones["Hand"];
        //handZone.onZoneChanged = handZone.onZoneChanged + numCardsのこと
        //マップ関数？とラムダ式の複合技？
        //一時変数だからnumCardsは宣言不要
        /*
         *
          funciotn lambda(Action<int> numCards)
          {
            return gameUI.SetPlayerHandCards(numCards)
          }
          handZone.onZoneChanged += lambda(handZone.onZoneChanged)
         *
         */
        handZone.onZoneChanged += numCards =>
        {
            gameUI.SetPlayerHandCards(numCards);
//            Debug.Log("handZone.onZoneChanged==" + handZone.onZoneChanged);
        };
        //マリガンゾーンにカードを追加
        handZone.onCardAdded += card =>
        {
            AddCardToHand(card);
            RearrangeMulliganZone();
//            Debug.Log("handZone.onCardAdded==" + handZone.onCardAdded);

        };

        //後手の場合デッキからもう一枚カードを引き、コインを手札に追加
        if (SenteGoteFlag)
        {

        }


        //ボタンを追加

        //カードをクリックしたらフラグを立てる

        //確定ボタンを押す
        //→GameSceneに追加


        //フラグの立ったカードをデッキに移動させる
        //移動したカードの枚数分デッキからカードを引く
        //時間カウントをリセット




        //手札にカード追加
        //handZone.onCardAdded = handZone.onCardAdded + card のこと
        //ラムダ式。

        //private void func()
        //{
        //  AddCardToHand(card);
        //  RearrangeHand();
        //}
        // handZone.onCardAdded += card(func);

        //と同じ意味

        //20181031マリガン確認用に一時的にコメントアウト
        //→コメントアウトすることでマリガン用カードを場に広げることに成功
        //handZone.onCardAdded += card =>
        //{
        //    AddCardToHand(card);
        //    RearrangeHand();
        //};



        //手札のカード削除
        handZone.onCardRemoved += card =>
        {
            var handCard = playerHandCards.Find(x => x.card == card);
            if (handCard != null)
            {
                playerHandCards.Remove(handCard);
                RearrangeHand();
            }
        };

        boardZone = playerInfo.namedZones["Board"];
        boardZone.onCardRemoved += card =>
        {
            var graveyardPos = GameObject.Find("GraveyardPlayer").transform.position + new Vector3(0.0f, -0.2f, 0.0f);
            var boardCard = playerBoardCards.Find(x => x.card == card);
            if (boardCard != null)
            {
                playerGraveyardCards.Add(boardCard);
                playerBoardCards.Remove(boardCard);
                boardCard.transform.DOKill();
                boardCard.transform.DOMove(graveyardPos, 0.7f);
                boardCard.SetHighlightingEnabled(false);
                boardCard.StopSleepingParticles();
                RearrangeBottomBoard();
                boardCard.GetComponent<SortingGroup>().sortingLayerName = "BoardCards";
                boardCard.GetComponent<SortingGroup>().sortingOrder = playerGraveyardCards.Count;
                Destroy(boardCard.GetComponent<BoxCollider2D>());
            }
            else if (currentSpellCard != null && card == currentSpellCard.card)
            {
                currentSpellCard.SetHighlightingEnabled(false);
                currentSpellCard.GetComponent<SortingGroup>().sortingLayerName = "BoardCards";
                currentSpellCard.GetComponent<SortingGroup>().sortingOrder = playerGraveyardCards.Count;
                Destroy(currentSpellCard.GetComponent<BoxCollider2D>());
                currentSpellCard.transform.DOMove(graveyardPos, 0.5f);
                currentSpellCard.transform.DOScale(new Vector2(0.6f, 0.6f), 0.5f);
                currentSpellCard.GetComponent<HandCard>().enabled = false;
                currentSpellCard = null;
            }
        };

        graveyardZone = playerInfo.namedZones["Graveyard"];
        graveyardZone.onZoneChanged += numCards =>
        {
            gameUI.SetPlayerGraveyardCards(numCards);
        };

        opponentDeckZone = opponentInfo.namedZones["Deck"];
        opponentDeckZone.onZoneChanged += numCards =>
        {
            gameUI.SetOpponentDeckCards(numCards);
        };

        opponentHandZone = opponentInfo.namedZones["Hand"];
        opponentHandZone.onZoneChanged += numCards =>
        {
            gameUI.SetOpponentHandCards(numCards);
        };
        opponentHandZone.onCardRemoved += card =>
        {
            var randomIndex = UnityEngine.Random.Range(0, opponentHandCards.Count);
            var randomCard = opponentHandCards[randomIndex];
            opponentHandCards.Remove(randomCard);
            Destroy(randomCard);
            RearrangeOpponentHand();
        };

        opponentBoardZone = opponentInfo.namedZones["Board"];
        opponentBoardZone.onCardRemoved += card =>
        {
            var graveyardPos = GameObject.Find("GraveyardOpponent").transform.position + new Vector3(0.0f, -0.2f, 0.0f);
            var boardCard = opponentBoardCards.Find(x => x.card == card);
            if (boardCard != null)
            {
                opponentGraveyardCards.Add(boardCard);
                opponentBoardCards.Remove(boardCard);
                boardCard.transform.DOKill();
                boardCard.transform.DOMove(graveyardPos, 0.7f);
                boardCard.SetHighlightingEnabled(false);
                boardCard.StopSleepingParticles();
                RearrangeTopBoard();
                boardCard.GetComponent<SortingGroup>().sortingLayerName = "BoardCards";
                boardCard.GetComponent<SortingGroup>().sortingOrder = opponentGraveyardCards.Count;
                Destroy(boardCard.GetComponent<BoxCollider2D>());
            }
            else if (currentSpellCard != null && card == currentSpellCard.card)
            {
                currentSpellCard.SetHighlightingEnabled(false);
                currentSpellCard.GetComponent<SortingGroup>().sortingLayerName = "BoardCards";
                currentSpellCard.GetComponent<SortingGroup>().sortingOrder = opponentGraveyardCards.Count;
                Destroy(currentSpellCard.GetComponent<BoxCollider2D>());
                var sequence = DOTween.Sequence();
                sequence.PrependInterval(2.0f);
                sequence.Append(currentSpellCard.transform.DOMove(graveyardPos, 0.5f));
                sequence.Append(currentSpellCard.transform.DOScale(new Vector2(0.6f, 0.6f), 0.5f));
                sequence.OnComplete(() =>
                {
                    currentSpellCard = null;
                });
            }
        };

        opponentGraveyardZone = opponentInfo.namedZones["Graveyard"];
        opponentGraveyardZone.onZoneChanged += numCards =>
        {
            gameUI.SetOpponentGraveyardCards(numCards);
        };
    }
    /// <summary>
    /// ゲーム開始時の処理
    /// </summary>
    /// <param name="msg"></param>
    public override void OnStartGame(StartGameMessage msg)
    {
        base.OnStartGame(msg);

        GameObject.Find("Player/Avatar").GetComponent<PlayerAvatar>().playerInfo = playerInfo;
        GameObject.Find("Opponent/Avatar").GetComponent<PlayerAvatar>().playerInfo = opponentInfo;

        //自分が後手だったら相手のカード3枚
        if (SenteGoteFlag == false) {
            opponentHandZone.numCards = 3;
        }
        else
        {
            opponentHandZone.numCards = 4;
        }


        for (var i = 0; i < opponentHandZone.numCards; i++)
        {
            AddCardToOpponentHand();
        }

        RearrangeOpponentHand();

        // Update the UI as appropriate.
        //必要に応じてUIを更新します。
        gameUI.SetPlayerHealth(lifeStat.effectiveValue);
        gameUI.SetOpponentHealth(opponentLifeStat.effectiveValue);
        gameUI.SetPlayerMana(manaStat.effectiveValue);
        gameUI.SetOpponentMana(opponentManaStat.effectiveValue);

        gameUI.SetPlayerHandCards(handZone.cards.Count);
        gameUI.SetPlayerGraveyardCards(graveyardZone.numCards);
        gameUI.SetPlayerDeckCards(deckZone.numCards);
        gameUI.SetOpponentHandCards(opponentHandZone.numCards);
        gameUI.SetOpponentGraveyardCards(opponentGraveyardZone.numCards);
        gameUI.SetOpponentDeckCards(opponentDeckZone.numCards);

        // Set the player nicknames in the UI.
        //プレイヤーのニックネームをUIで設定します。

        
        SelectHeroScene shc = new SelectHeroScene();
        bool rFlag = shc.GetRenkoNameFlag();
        if(rFlag){
                gameUI.SetPlayerName("宇佐見蓮子");
                gameUI.SetOpponentName("マエリベリー・ハーン");
        }else{
                gameUI.SetPlayerName("マエリベリー・ハーン");
                gameUI.SetOpponentName("宇佐見蓮子");
        }
        
        

        for (var i = 0; i < msg.nicknames.Length; i++)
        {
            var nickname = msg.nicknames[i];

            if (i == msg.playerIndex)
            {
                gameUI.SetPlayerName(nickname);
            }
            else
            {
                gameUI.SetOpponentName(nickname);
            }
        }

        float rnd;
        rnd = UnityEngine.Random.value;
        var gameScene = GameObject.Find("GameScene");
        if (rnd > 0.5f || rFlag)
        {
            gameScene = GameObject.Find("GameScene");
        }
        else if (rnd > 0.5f || rFlag == false)
        {
            gameScene = GameObject.Find("GameScene");
        } else if (rnd < 0.5f || rFlag) {
            gameScene = GameObject.Find("GameScene");
        } else if (rnd < 0.5f || rFlag == false)
        {
            gameScene = GameObject.Find("GameScene");
        }
        else {
            gameScene = GameObject.Find("GameScene");
        }


        if (gameScene != null)
        {
#if USING_MASTER_SERVER_KIT
            if (gameScene.GetComponent<MSK_GameScene>() != null)
            {
                gameScene.GetComponent<MSK_GameScene>().CloseWaitingWindow();
            }
#else
            if (gameScene.GetComponent<GameScene>() != null)
            {
                gameScene.GetComponent<GameScene>().CloseWaitingWindow();
            }
#endif
        }
        //ターン終了ボタンの描画
        var endTurnButton = GameObject.Find("EndTurnButton");
        if (endTurnButton != null)
        {
            endTurnButton.GetComponent<EndTurnButton>().player = this;
        }

        //ヒロパボタンの描画
        var heroPowerButton = GameObject.Find("PlayerHeroPower");
        if (heroPowerButton != null)
        {
//            heroPowerButton.GetComponent<HeroPower>().player = this;
        }

    }
    /// <summary>
    /// ターン開始時の処理
    /// </summary>
    /// <param name="msg"></param>
    public override void OnStartTurn(StartTurnMessage msg)
    {
//        Debug.Log("OnStartTurn開始");

        base.OnStartTurn(msg);

        //各ヒーロー、ターンエンドボタン、ヒロパ活性化
        gameUI.SetPlayerActive(msg.isRecipientTheActivePlayer);
        gameUI.SetOpponentActive(!msg.isRecipientTheActivePlayer);
        gameUI.SetEndTurnButtonEnabled(msg.isRecipientTheActivePlayer);
        gameUI.SetHeroPowerEnabled(msg.isRecipientTheActivePlayer);

        //一旦カードを全て削除
        foreach (var card in opponentHandCards)
        {
            Destroy(card);
        }
        opponentHandCards.Clear();
        //手札にカードを追加
        for (var i = 0; i < opponentHandZone.numCards; i++)
        {
            AddCardToOpponentHand();
        }
        RearrangeOpponentHand();

        if (msg.isRecipientTheActivePlayer)
        {
            UpdateHandCardsHighlight();

            foreach (var card in playerBoardCards)
            {
                card.OnStartTurn();
            }

            var scene = GameObject.Find("GameScene").GetComponent<GameScene>();
            scene.OpenPopup<PopupTurnStart>("PopupTurnStart", null, false);

            gameUI.StartTurnCountdown(turnDuration);
        }
        else
        {
            foreach (var card in opponentBoardCards)
            {
                card.OnStartTurn();
            }

            foreach (var card in playerHandCards)
            {
                card.SetHighlightingEnabled(false);
            }
            foreach (var card in playerBoardCards)
            {
                card.SetHighlightingEnabled(false);
            }

            gameUI.HideTurnCountdown();
        }
    }

    /// <summary>
    /// マリガンに関する描画処理
    /// </summary>
    /// <param name="onComplete"></param>
    //virtualは仮想関数の印。継承先(派生クラス)でも同様のメソッドが作られることを明示している。
    protected virtual void RearrangeMulliganZone(Action onComplete = null)
    {
        var boardWidth = 0.0f;
        var spacing = 0.5f;
        var cardWidth = 0.0f;
        foreach (var card in playerHandCards)
        {
            cardWidth = card.GetComponent<SpriteRenderer>().bounds.size.x;
            boardWidth += cardWidth;
            boardWidth += spacing;
        }
        boardWidth -= spacing;

        var newPositions = new List<Vector2>(playerHandCards.Count);
        var pivot = GameObject.Find("MulliganZone").transform.position;
        for (var i = 0; i < playerHandCards.Count; i++)
        {
            var card = playerHandCards[i];
            newPositions.Add(new Vector2(pivot.x - boardWidth / 2 + cardWidth / 2, pivot.y));
            pivot.x += boardWidth / playerHandCards.Count;
        }

        var sequence = DOTween.Sequence();
        for (var i = 0; i < playerHandCards.Count; i++)
        {
            var card = playerHandCards[i];
            sequence.Insert(0, card.transform.DOMove(newPositions[i], 0.4f).SetEase(Ease.OutSine));
        }
        sequence.OnComplete(() =>
        {
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }



    /// <summary>
    /// プレイヤーの手札に関する描画処理
    /// </summary>
    protected virtual void RearrangeHand()
    {
        var handWidth = 0.0f;
        var spacing = -1.0f;
        foreach (var card in playerHandCards)
        {
            handWidth += card.GetComponent<SpriteRenderer>().bounds.size.x;
            handWidth += spacing;
        }
        handWidth -= spacing;

        var pivot = Camera.main.ViewportToWorldPoint(new Vector3(0.55f, 0.08f, 0.0f));
        var totalTwist = -20f;
        if (playerHandCards.Count == 1)
        {
            totalTwist = 0;
        }
        var twistPerCard = totalTwist / playerHandCards.Count;
        float startTwist = -1f * (totalTwist / 2);
        var scalingFactor = 0.06f;
        for (var i = 0; i < playerHandCards.Count; i++)
        {
            var card = playerHandCards[i];
            var twist = startTwist + (i * twistPerCard);
            var nudge = Mathf.Abs(twist);
            nudge *= scalingFactor;
            card.transform.position = new Vector2(pivot.x - handWidth / 2, pivot.y - nudge);
            card.transform.rotation = Quaternion.Euler(0, 0, twist);
            pivot.x += handWidth / playerHandCards.Count;
            card.GetComponent<SortingGroup>().sortingLayerName = "HandCards";
            card.GetComponent<SortingGroup>().sortingOrder = i;
        }
    }

    /// <summary>
    /// 対戦相手の手札に関する描画処理
    /// </summary>
    protected virtual void RearrangeOpponentHand()
    {
        var handWidth = 0.0f;
        var spacing = -1.0f;
        foreach (var card in opponentHandCards)
        {
            handWidth += card.GetComponent<SpriteRenderer>().bounds.size.x;
            handWidth += spacing;
        }
        handWidth -= spacing;

        var pivot = Camera.main.ViewportToWorldPoint(new Vector3(0.55f, 1, 0.0f));
        var totalTwist = -20f;
        if (opponentHandCards.Count == 1)
        {
            totalTwist = 0;
        }
        var twistPerCard = totalTwist / opponentHandCards.Count;
        float startTwist = -1f * (totalTwist / 2);
        var scalingFactor = 0.06f;
        for (var i = 0; i < opponentHandCards.Count; i++)
        {
            var card = opponentHandCards[i];
            var twist = startTwist + (i * twistPerCard);
            var nudge = Mathf.Abs(twist);
            nudge *= scalingFactor;
            card.transform.position = new Vector2(pivot.x - handWidth / 2, pivot.y + nudge);
            card.transform.rotation = Quaternion.Euler(0, 0, -twist);
            pivot.x += handWidth / opponentHandCards.Count;
            card.GetComponent<SortingGroup>().sortingOrder = i;
        }
    }
    /// <summary>
    /// 対戦相手のボードに関する描画処理
    /// </summary>
    /// <param name="onComplete"></param>
    protected virtual void RearrangeTopBoard(Action onComplete = null)
    {
        var boardWidth = 0.0f;
        var spacing = 0.5f;
        var cardWidth = 0.0f;
        foreach (var card in opponentBoardCards)
        {
            cardWidth = card.GetComponent<SpriteRenderer>().bounds.size.x;
            boardWidth += cardWidth;
            boardWidth += spacing;
        }
        boardWidth -= spacing;

        var newPositions = new List<Vector2>(opponentBoardCards.Count);
        var pivot = GameObject.Find("OpponentBoard").transform.position;
        for (var i = 0; i < opponentBoardCards.Count; i++)
        {
            var card = opponentBoardCards[i];
            newPositions.Add(new Vector2(pivot.x - boardWidth / 2 + cardWidth / 2, pivot.y));
            pivot.x += boardWidth / opponentBoardCards.Count;
        }

        var sequence = DOTween.Sequence();
        for (var i = 0; i < opponentBoardCards.Count; i++)
        {
            var card = opponentBoardCards[i];
            sequence.Insert(0, card.transform.DOMove(newPositions[i], 0.4f).SetEase(Ease.OutSine));
        }
        sequence.OnComplete(() =>
        {
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }
    /// <summary>
    /// プレイヤーのボードに関する描画処理
    /// </summary>
    /// <param name="onComplete"></param>
    //virtualは仮想関数の印。継承先(派生クラス)でも同様のメソッドが作られることを明示している。
    protected virtual void RearrangeBottomBoard(Action onComplete = null)
    {
        var boardWidth = 0.0f;
        var spacing = 0.5f;
        var cardWidth = 0.0f;
        foreach (var card in playerBoardCards)
        {
            cardWidth = card.GetComponent<SpriteRenderer>().bounds.size.x;
            boardWidth += cardWidth;
            boardWidth += spacing;
        }
        boardWidth -= spacing;

        var newPositions = new List<Vector2>(playerBoardCards.Count);
        var pivot = GameObject.Find("PlayerBoard").transform.position;
        for (var i = 0; i < playerBoardCards.Count; i++)
        {
            var card = playerBoardCards[i];
            newPositions.Add(new Vector2(pivot.x - boardWidth / 2 + cardWidth / 2, pivot.y));
            pivot.x += boardWidth / playerBoardCards.Count;
        }

        var sequence = DOTween.Sequence();
        for (var i = 0; i < playerBoardCards.Count; i++)
        {
            var card = playerBoardCards[i];
            sequence.Insert(0, card.transform.DOMove(newPositions[i], 0.4f).SetEase(Ease.OutSine));
        }
        sequence.OnComplete(() =>
        {
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    /// <summary>
    /// ターン終了時の処理
    /// </summary>
    /// <param name="msg"></param>
    public override void OnEndTurn(EndTurnMessage msg)
    {
        base.OnEndTurn(msg);

        if (msg.isRecipientTheActivePlayer)
        {
            gameUI.SetEndTurnButtonEnabled(false);

            foreach (var card in playerBoardCards)
            {
                card.OnEndTurn();
            }

            if (currentCreature != null)
            {
                playerBoardCards.Remove(currentCreature);
                RearrangeBottomBoard();

                playerInfo.namedZones["Hand"].AddCard(currentCreature.card);
                playerInfo.namedZones["Board"].RemoveCard(currentCreature.card);

                Destroy(currentCreature.gameObject);
                currentCreature = null;
            }

            if (currentSpellCard != null)
            {
                Destroy(currentSpellCard.GetComponent<BoardSpell>());
                currentSpellCard = null;
                RearrangeHand();
            }
        }

        gameUI.StopCountdown();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void StopTurn()
    {
        var msg = new StopTurnMessage();
        client.Send(NetworkProtocol.StopTurn, msg);
    }

    /// <summary>
    /// 毎フレーム呼び出される
    /// </summary>
    protected virtual void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (!gameStarted)
        {
            return;
        }

/*
        if (chatPopup.isVisible)
        {
            return;
        }
*/

        //マウスの場所を検知
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


//        Debug.Log(isCardSelected);
        
        
        
        //左クリックされたら
        if (Input.GetMouseButtonDown(0))
        {
            //マリガン中かどうか判定
            if (mulliganFlag) {
                //クリックされたものがマリガン用カードかどうか判定

            }

            //今プレイヤーのターン中且つスペカが発動中でなければ
            else if (isActivePlayer && currentSpellCard == null)
            {
                //マウスがクリックしたポジションとRaycast(オブジェクト)がぶつかってないか確認(Vector2.zeroはオブジェクトの原点)
                var hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
                //Objectを用意(中身は空)
                var hitCards = new List<GameObject>();


                foreach (var hit in hits)
                {
                    //hit.colliderは内部のプロパティをチェックして衝突があったかbooleanで返す
                    if (hit.collider != null &&
                        hit.collider.gameObject != null &&
                        hit.collider.gameObject.GetComponent<CardView>() != null &&
                        !hit.collider.gameObject.GetComponent<CardView>().isPreview &&
                        hit.collider.gameObject.GetComponent<CardView>().CanBePlayed(this))
                    {
                        //活性化しているカードだった場合、gameObjectをhitcardに加える
                        hitCards.Add(hit.collider.gameObject);
                    }
                }
                //アクティブなカード上でクリックしたかどうかの判定
                if (hitCards.Count > 0)
                {
                    DestroyCardPreview();
                    hitCards = hitCards.OrderBy(x => x.GetComponent<SortingGroup>().sortingOrder).ToList();
                    var topmostCardView = hitCards[hitCards.Count - 1].GetComponent<CardView>();
                    var topmostHandCard = topmostCardView.GetComponent<HandCard>();
                    if (topmostHandCard != null)
                    {
                        topmostCardView.GetComponent<HandCard>().OnSelected();
                    }
                }
            }
        }




        //カードの上にマウスカーソルが乗っていなければ
        else if (!isCardSelected)
        {
            var hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
            var hitCards = new List<GameObject>();
            //選択中のハンドのカード
            var hitHandCard = false;
            //選択中の、ボードのカード
            var hitBoardCard = false;

            //20181031マリガン用に追加
            var hitMulliganCard = false;

            foreach (var hit in hits)
            {
                //オブジェクト上でクリックしたなら
                if (hit.collider != null &&
                    hit.collider.gameObject != null &&
                    hit.collider.gameObject.GetComponent<CardView>() != null)
                {
                    hitCards.Add(hit.collider.gameObject);
                    hitHandCard = true;
                }
            }

            //クリックしたのがマリガンカードじゃない場合
//            if (!hitMulliganCard)
//            {
                //クリックしたのがハンドのカードでなければボードのミニオンを選択

                if (!hitHandCard)
                {
                    foreach (var hit in hits)
                    {
                        if (hit.collider != null &&
                            hit.collider.gameObject != null &&
                            hit.collider.gameObject.GetComponent<BoardCreature>() != null)
                        {
                            hitCards.Add(hit.collider.gameObject);
                            hitBoardCard = true;
                        }
                    }
                }
                //ハンドのカードを選択中であれば
                if (hitHandCard)
                {
                    if (hitCards.Count > 0)
                    {
                        hitCards = hitCards.OrderBy(x => x.GetComponent<SortingGroup>().sortingOrder).ToList();
                        var topmostCardView = hitCards[hitCards.Count - 1].GetComponent<CardView>();
                        if (!topmostCardView.isPreview)
                        {
                            if (!isPreviewActive || topmostCardView.card.instanceId != currentPreviewedCardId)
                            {
                                DestroyCardPreview();
                                CreateCardPreview(topmostCardView.card, topmostCardView.transform.position, topmostCardView.IsHighlighted());
                            }
                        }
                    }
                }
                else if (hitBoardCard)
                {
                    if (hitCards.Count > 0)
                    {
                        hitCards = hitCards.OrderBy(x => x.GetComponent<SortingGroup>().sortingOrder).ToList();
                        var selectedBoardCreature = hitCards[hitCards.Count - 1].GetComponent<BoardCreature>();
                        if (!isPreviewActive || selectedBoardCreature.card.instanceId != currentPreviewedCardId)
                        {
                            DestroyCardPreview();
                            CreateCardPreview(selectedBoardCreature.card, selectedBoardCreature.transform.position);
                        }
                    }
                }
                
                else
                {
                    DestroyCardPreview();
                }

            
            }
            //クリックしたカードがマリガンカードだったら入れ替えフラグを立てる
//            else if (hitMulliganCard)
//            {
//                var MulliganExchangeChecHitCards = new List<GameObject>();
//                var MulliganExchangeCheck = 0;

//            }
            else
            {
                DestroyCardPreview();
            }
//        }
    }

    /// <summary>
    /// カードのプレハブの描画
    /// </summary>
    /// <param name="card"></param>
    /// <param name="pos"></param>
    /// <param name="highlight"></param>
    public virtual void CreateCardPreview(RuntimeCard card, Vector3 pos, bool highlight = false)
    {
        isPreviewActive = true;
        currentPreviewedCardId = card.instanceId;
        //カードの描画
        createPreviewCoroutine = StartCoroutine(CreateCardPreviewAsync(card, pos, highlight));
    }

    /// <summary>
    /// カードの非同期のプレビュー(原文ママ)
    /// カードを召喚する際、描画する処理
    /// </summary>
    /// <param name="card"></param>
    /// <param name="pos"></param>
    /// <param name="highlight"></param>
    /// <returns></returns>
    protected virtual IEnumerator CreateCardPreviewAsync(RuntimeCard card, Vector3 pos, bool highlight)
    {
        //yield:コルーチンの途中で一定時間(この場合0.3秒)待つための記法
        yield return new WaitForSeconds(0.3f);

        var gameConfig = GameManager.Instance.config;
        var libraryCard = gameConfig.GetCard(card.cardId);
        var cardType = gameConfig.cardTypes.Find(x => x.id == libraryCard.cardTypeId);
        ///カードのタイプがCreatureだったらCreature用Prefabを表示
        if (cardType.name == "Creature")
        {
            //Instantiateはこのクラス内の変数が紐付けられたPrefabを描画するための処理
            //この場合はDemoPlayerPrefab内のCreatureCard
            currentCardPreview = Instantiate(creatureCardViewPrefab as GameObject);
        }
        ///カードのタイプがSpellだったらSpell用Prefabを表示
        else if (cardType.name == "Spell")
        {
            currentCardPreview = Instantiate(spellCardViewPrefab as GameObject);
        }
        var cardView = currentCardPreview.GetComponent<CardView>();
        //カードの情報を取り込む
        cardView.PopulateWithInfo(card);
        //使えるか否か判断してハイライトを設定
        cardView.SetHighlightingEnabled(highlight);
        cardView.isPreview = true;

        //配置設定
        var newPos = pos;
        newPos.y += 2.0f;
        currentCardPreview.transform.position = newPos;
        currentCardPreview.transform.localRotation = Quaternion.Euler(Vector3.zero);
        currentCardPreview.transform.localScale = new Vector2(1.5f, 1.5f);
        currentCardPreview.GetComponent<SortingGroup>().sortingOrder = 1000;
        currentCardPreview.layer = LayerMask.NameToLayer("Ignore Raycast");
        currentCardPreview.transform.DOMoveY(newPos.y + 1.0f, 0.1f);
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void DestroyCardPreview()
    {
        StartCoroutine(DestroyCardPreviewAsync());
        if (createPreviewCoroutine != null)
        {
            StopCoroutine(createPreviewCoroutine);
        }
        isPreviewActive = false;
    }


    /// <summary>
    /// 非同期でカードを消す処理
    /// </summary>
    /// <returns></returns>
    /// ※IEnumeratorやyieldは遅延評価(あとで値や型が決まるやつ)するループを簡単に書くための記述法
    protected virtual IEnumerator DestroyCardPreviewAsync()
    {
        //currentCardPreviewは今選択しているカード(ハンドやボード関係なし)
        if (currentCardPreview != null)
        {
            var oldCardPreview = currentCardPreview;
            foreach (var renderer in oldCardPreview.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.DOFade(0.0f, 0.2f);
            }
            foreach (var text in oldCardPreview.GetComponentsInChildren<TextMeshPro>())
            {
                text.DOFade(0.0f, 0.2f);
            }
            //0.5秒待ってから次の処理
            yield return new WaitForSeconds(0.5f);
            Destroy(oldCardPreview.gameObject);
        }
    }


    /// <summary>
    /// 呼び出し元から受け取ったRuntimeCardのカードを手札に加える
    /// </summary>
    /// <param name="card"></param>
    protected virtual void AddCardToMulliganZone(RuntimeCard card)
    {
        var gameConfig = GameManager.Instance.config;
        var libraryCard = gameConfig.GetCard(card.cardId);
        var cardType = gameConfig.cardTypes.Find(x => x.id == libraryCard.cardTypeId);
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
        cardView.PopulateWithInfo(card);

        var handCard = go.AddComponent<HandCard>();
        handCard.ownerPlayer = this;
        handCard.boardZone = GameObject.Find("PlayerBoard");

        playerHandCards.Add(cardView);

        go.GetComponent<SortingGroup>().sortingOrder = playerHandCards.Count;
    }


    /// <summary>
    /// 呼び出し元から受け取ったRuntimeCardのカードを手札に加える
    /// </summary>
    /// <param name="card"></param>
    protected virtual void AddCardToHand(RuntimeCard card)
    {
        var gameConfig = GameManager.Instance.config;
        var libraryCard = gameConfig.GetCard(card.cardId);
        var cardType = gameConfig.cardTypes.Find(x => x.id == libraryCard.cardTypeId);
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
        cardView.PopulateWithInfo(card);

        var handCard = go.AddComponent<HandCard>();
        handCard.ownerPlayer = this;
        handCard.boardZone = GameObject.Find("PlayerBoard");

        playerHandCards.Add(cardView);

        go.GetComponent<SortingGroup>().sortingOrder = playerHandCards.Count;
    }



    /// <summary>
    /// 相手の手札にカードを加える
    /// </summary>
    protected virtual void AddCardToOpponentHand()
    {
        var go = Instantiate(opponentCardPrefab as GameObject);
        opponentHandCards.Add(go);
        go.GetComponent<SortingGroup>().sortingOrder = opponentHandCards.Count;
    }

    /// <summary>
    /// エリア間(ハンドからボードに)のカードの移動を処理する
    /// プレハブのインスタンス等も含む。
    /// ドローなどもここ
    /// </summary>
    /// <param name="card">使用可能なカード</param>
    public void PlayCard(CardView card)
    {
        if (card.CanBePlayed(this))
        {
            gameUI.endTurnButton.SetEnabled(false);

            var gameConfig = GameManager.Instance.config;
            var libraryCard = gameConfig.GetCard(card.card.cardId);
            //スペルかミニオンか
            var cardType = gameConfig.cardTypes.Find(x => x.id == libraryCard.cardTypeId);
            if (cardType.name == "Creature")
            {
                //ミニオンをボード上にプレハブ化
                var boardCreature = Instantiate(boardCreaturePrefab);

                var board = GameObject.Find("PlayerBoard");
                boardCreature.tag = "PlayerOwned";
                boardCreature.transform.parent = board.transform;
                boardCreature.transform.position = new Vector2(1.9f * playerBoardCards.Count, 0);
                //
                boardCreature.GetComponent<BoardCreature>().ownerPlayer = this;
                //ミニオンのテキストやイラスト、キーワードの描画
                boardCreature.GetComponent<BoardCreature>().PopulateWithInfo(card.card);

                //手札からプレイしたカードを消す
                playerHandCards.Remove(card);
                RearrangeHand();
                playerBoardCards.Add(boardCreature.GetComponent<BoardCreature>());

                //手札のカードのオブジェクトを削除
                Destroy(card.gameObject);

                currentCreature = boardCreature.GetComponent<BoardCreature>();

                //ボードの描画処理。RearrangeBottomBoardの()内はActionクラスで、libraryCard(特定のミニオンのID)が持つカードの能力を探してる
                RearrangeBottomBoard(() =>{var triggeredAbilities = libraryCard.abilities.FindAll(x => x is TriggeredAbility);
                    TriggeredAbility targetableAbility = null;
                    foreach (var ability in triggeredAbilities)
                    {
                        var triggeredAbility = ability as TriggeredAbility;
                        var trigger = triggeredAbility.trigger as OnCardEnteredZoneTrigger;
                        if (trigger != null && trigger.zoneId == boardZone.zoneId && triggeredAbility.target is IUserTarget)
                        {
                            targetableAbility = triggeredAbility;
                            break;
                        }
                    }

                    // Preemptively move the card so that the effect solver can properly check the availability of targets
                    // by also taking into account this card (that is trying to be played).
                    //プレイしようとしているカードも考慮して、エフェクトソルバーがターゲットの使用可能性を適切にチェックできるように、先制してカードを移動します。
                    playerInfo.namedZones["Hand"].RemoveCard(card.card);
                    playerInfo.namedZones["Board"].AddCard(card.card);

                    //ターゲットを取るアビリティがあり、且つターゲットがいる場合
                    if (targetableAbility != null && effectSolver.AreTargetsAvailable(targetableAbility.effect, card.card, targetableAbility.target))
                    {
                        var targetingArrow = Instantiate(spellTargetingArrowPrefab).GetComponent<SpellTargetingArrow>();
                        boardCreature.GetComponent<BoardCreature>().abilitiesTargetingArrow = targetingArrow;
                        targetingArrow.effectTarget = targetableAbility.target;
                        targetingArrow.targetType = targetableAbility.target.GetTarget();
                        targetingArrow.onTargetSelected += () =>
                        {
                            PlayCreatureCard(card.card, targetingArrow.targetInfo);
                            effectSolver.MoveCard(netId, card.card, "Hand", "Board", targetingArrow.targetInfo);
                            currentCreature = null;
                            gameUI.endTurnButton.SetEnabled(true);
                        };
                        targetingArrow.Begin(boardCreature.transform.localPosition);
                    }
                    else
                    {
                        PlayCreatureCard(card.card);
                        effectSolver.MoveCard(netId, card.card, "Hand", "Board");
                        currentCreature = null;
                        gameUI.endTurnButton.SetEnabled(true);
                    }
                    boardCreature.GetComponent<BoardCreature>().fightTargetingArrowPrefab = fightTargetingArrowPrefab;
                });
            }
            //スペルの場合
            else if (cardType.name == "Spell")
            {
                var spellsPivot = GameObject.Find("PlayerSpellsPivot");
                var sequence = DOTween.Sequence();
                sequence.Append(card.transform.DOMove(spellsPivot.transform.position, 0.5f));
                sequence.Insert(0, card.transform.DORotate(Vector3.zero, 0.2f));
                sequence.Play().OnComplete(() =>
                {
                    card.GetComponent<SortingGroup>().sortingLayerName = "BoardCards";
                    card.GetComponent<SortingGroup>().sortingOrder = 1000;

                    var boardSpell = card.gameObject.AddComponent<BoardSpell>();

                    var triggeredAbilities = libraryCard.abilities.FindAll(x => x is TriggeredAbility);
                    TriggeredAbility targetableAbility = null;
                    foreach (var ability in triggeredAbilities)
                    {
                        var triggeredAbility = ability as TriggeredAbility;
                        var trigger = triggeredAbility.trigger as OnCardEnteredZoneTrigger;
                        //トリガーがあり、ボードに入ることがトリガーで、且つユーザーがターゲットを指定している場合
                        if (trigger != null && trigger.zoneId == boardZone.zoneId && triggeredAbility.target is IUserTarget)
                        {
                            targetableAbility = triggeredAbility;
                            break;
                        }
                    }

                    currentSpellCard = card;
                    //ターゲットを取るアビリティがあり、且つターゲットがいる場合、ターゲットを指定してスペルの処理をする
                    if (targetableAbility != null && effectSolver.AreTargetsAvailable(targetableAbility.effect, card.card, targetableAbility.target))
                    {
                        var targetingArrow = Instantiate(spellTargetingArrowPrefab).GetComponent<SpellTargetingArrow>();
                        boardSpell.targetingArrow = targetingArrow;
                        targetingArrow.effectTarget = targetableAbility.target;
                        targetingArrow.targetType = targetableAbility.target.GetTarget();
                        targetingArrow.onTargetSelected += () =>
                        {
                            PlaySpellCard(card.card, targetingArrow.targetInfo);
                            effectSolver.MoveCard(netId, card.card, "Hand", "Board", targetingArrow.targetInfo);
                            currentSpellCard = null;
                            gameUI.endTurnButton.SetEnabled(true);
                        };
                        targetingArrow.Begin(boardSpell.transform.localPosition);
                    }
                    else
                    //そうでなければそのまま処理
                    {
                        PlaySpellCard(card.card);
                        effectSolver.MoveCard(netId, card.card, "Hand", "Board");
                        currentSpellCard = null;
                        gameUI.endTurnButton.SetEnabled(true);
                    }
                });
            }
        }
        else
        {
            card.GetComponent<HandCard>().ResetToInitialPosition();
        }
    }
    /// <summary>
    /// 受け取ったカードIDからカードをボードに召喚する
    /// </summary>
    /// <param name="card">指定中のカードの固有ID</param>
    public void SummonMinion(int cardId) {
       
        /*
            gameUI.endTurnButton.SetEnabled(false);

            var gameConfig = GameManager.Instance.config;
            var libraryCard = gameConfig.GetCard(cardId);
            //スペルかミニオンか
            var cardType = gameConfig.cardTypes.Find(x => x.id == libraryCard.cardTypeId);
            if (cardType.name == "Creature")
            {
                //ミニオンをボード上にプレハブ化
                var boardCreature = Instantiate(boardCreaturePrefab);

                var board = GameObject.Find("PlayerBoard");
                boardCreature.tag = "PlayerOwned";
                boardCreature.transform.parent = board.transform;
                boardCreature.transform.position = new Vector2(1.9f * playerBoardCards.Count, 0);
                boardCreature.GetComponent<BoardCreature>().ownerPlayer = this;
//                boardCreature.GetComponent<BoardCreature>().PopulateWithInfo(card.card);

            currentCreature = boardCreature.GetComponent<BoardCreature>();

                //ボードの描画処理。RearrangeBottomBoardの()内はActionクラスで、カードの能力を探してる
                RearrangeBottomBoard(() => {
                    var triggeredAbilities = libraryCard.abilities.FindAll(x => x is TriggeredAbility);
                    TriggeredAbility targetableAbility = null;
                    foreach (var ability in triggeredAbilities)
                    {
                        var triggeredAbility = ability as TriggeredAbility;
                        var trigger = triggeredAbility.trigger as OnCardEnteredZoneTrigger;
                        if (trigger != null && trigger.zoneId == boardZone.zoneId && triggeredAbility.target is IUserTarget)
                        {
                            targetableAbility = triggeredAbility;
                            break;
                        }
                    }

                    // Preemptively move the card so that the effect solver can properly check the availability of targets
                    // by also taking into account this card (that is trying to be played).
                    //プレイしようとしているカードも考慮して、エフェクトソルバーがターゲットの使用可能性を適切にチェックできるように、先制してカードを移動します。
                    playerInfo.namedZones["Hand"].RemoveCard(card.card);
                    playerInfo.namedZones["Board"].AddCard(card.card);

                    //ターゲットを取るアビリティがあり、且つターゲットがいる場合
                    if (targetableAbility != null && effectSolver.AreTargetsAvailable(targetableAbility.effect, card.card, targetableAbility.target))
                    {
                        var targetingArrow = Instantiate(spellTargetingArrowPrefab).GetComponent<SpellTargetingArrow>();
                        boardCreature.GetComponent<BoardCreature>().abilitiesTargetingArrow = targetingArrow;
                        targetingArrow.effectTarget = targetableAbility.target;
                        targetingArrow.targetType = targetableAbility.target.GetTarget();
                        targetingArrow.onTargetSelected += () =>
                        {
                            PlayCreatureCard(card.card, targetingArrow.targetInfo);
                            effectSolver.MoveCard(netId, card.card, "Hand", "Board", targetingArrow.targetInfo);
                            currentCreature = null;
                            gameUI.endTurnButton.SetEnabled(true);
                        };
                        targetingArrow.Begin(boardCreature.transform.localPosition);
                    }
                    else
                    {
                        PlayCreatureCard(card.card);
                        effectSolver.MoveCard(netId, card.card, "Hand", "Board");
                        currentCreature = null;
                        gameUI.endTurnButton.SetEnabled(true);
                    }
                    boardCreature.GetComponent<BoardCreature>().fightTargetingArrowPrefab = fightTargetingArrowPrefab;
                });
            }
            //スペルの場合
            else if (cardType.name == "Spell")
            {
                var spellsPivot = GameObject.Find("PlayerSpellsPivot");
                var sequence = DOTween.Sequence();
                sequence.Append(card.transform.DOMove(spellsPivot.transform.position, 0.5f));
                sequence.Insert(0, card.transform.DORotate(Vector3.zero, 0.2f));
                sequence.Play().OnComplete(() =>
                {
                    card.GetComponent<SortingGroup>().sortingLayerName = "BoardCards";
                    card.GetComponent<SortingGroup>().sortingOrder = 1000;

                    var boardSpell = card.gameObject.AddComponent<BoardSpell>();

                    var triggeredAbilities = libraryCard.abilities.FindAll(x => x is TriggeredAbility);
                    TriggeredAbility targetableAbility = null;
                    foreach (var ability in triggeredAbilities)
                    {
                        var triggeredAbility = ability as TriggeredAbility;
                        var trigger = triggeredAbility.trigger as OnCardEnteredZoneTrigger;
                        //トリガーがあり、ボードに入ることがトリガーで、且つユーザーがターゲットを指定している場合
                        if (trigger != null && trigger.zoneId == boardZone.zoneId && triggeredAbility.target is IUserTarget)
                        {
                            targetableAbility = triggeredAbility;
                            break;
                        }
                    }

                    currentSpellCard = card;
                    //ターゲットを取るアビリティがあり、且つターゲットがいる場合、ターゲットを指定してスペルの処理をする
                    if (targetableAbility != null && effectSolver.AreTargetsAvailable(targetableAbility.effect, card.card, targetableAbility.target))
                    {
                        var targetingArrow = Instantiate(spellTargetingArrowPrefab).GetComponent<SpellTargetingArrow>();
                        boardSpell.targetingArrow = targetingArrow;
                        targetingArrow.effectTarget = targetableAbility.target;
                        targetingArrow.targetType = targetableAbility.target.GetTarget();
                        targetingArrow.onTargetSelected += () =>
                        {
                            PlaySpellCard(card.card, targetingArrow.targetInfo);
                            effectSolver.MoveCard(netId, card.card, "Hand", "Board", targetingArrow.targetInfo);
                            currentSpellCard = null;
                            gameUI.endTurnButton.SetEnabled(true);
                        };
                        targetingArrow.Begin(boardSpell.transform.localPosition);
                    }
                    else
                    //そうでなければそのまま処理
                    {
                        PlaySpellCard(card.card);
                        effectSolver.MoveCard(netId, card.card, "Hand", "Board");
                        currentSpellCard = null;
                        gameUI.endTurnButton.SetEnabled(true);
                    }
                });
            }
        
        else
        {
            card.GetComponent<HandCard>().ResetToInitialPosition();
        }
            */
        
    }

    /// <summary>
    /// カードの使用可否状態をアップデート
    /// </summary>
    protected void UpdateHandCardsHighlight()
    {
        foreach (var card in playerHandCards)
        {
            if (card.CanBePlayed(this))
            {
                card.SetHighlightingEnabled(true);
            }
            else
            {
                card.SetHighlightingEnabled(false);
            }
        }
    }

    /// <summary>
    /// ゲーム終了時の挙動
    /// </summary>
    /// <param name="msg"></param>
    public override void OnEndGame(EndGameMessage msg)
    {
        base.OnEndGame(msg);

        var scene = GameObject.Find("GameScene").GetComponent<GameScene>();
        scene.OpenPopup<PopupOneButton>("PopupOneButton", popup =>
        {
            if (msg.winnerPlayerIndex == playerInfo.netId)
            {
                popup.text.text = "人生の勝利者";
            }
            else
            {
                popup.text.text = "負け犬";
            }
            popup.buttonText.text = "閉じる";
            popup.button.onClickEvent.AddListener(() =>
            {
                if (NetworkingUtils.GetLocalPlayer().isServer)
                {
                    NetworkManager.singleton.StopHost();
                }
                else
                {
                    NetworkManager.singleton.StopClient();
                }

                //追記
                SceneManager.LoadScene("Home");  
            }
            
            
            

            );
          
        });
        
    }

    /// <summary>
    /// カード移動時の処理
    /// </summary>
    /// <param name="msg">カードの固有ID等を持ったメッセージ。ネットワーク越しに受け渡しをする</param>
    /// Playerクラスのオーバーライド
    public override void OnCardMoved(CardMovedMessage msg)
    {
        base.OnCardMoved(msg);

        //カード設定読み込み
        var gameConfig = GameManager.Instance.config;
        var libraryCard = gameConfig.GetCard(msg.card.cardId);
        var cardType = gameConfig.cardTypes.Find(x => x.id == libraryCard.cardTypeId);

        //ランダムな値
        var randomIndex = UnityEngine.Random.Range(0, opponentHandCards.Count);
        //ランダムなカード
        var randomCard = opponentHandCards[randomIndex];
        opponentHandCards.Remove(randomCard);
        Destroy(randomCard);
        RearrangeOpponentHand();
        gameUI.SetOpponentHandCards(opponentHandCards.Count);

        if (cardType.name == "Creature")
        {
            var opponentBoard = opponentInfo.namedZones["Board"];
            effectSolver.SetDestroyConditions(opponentBoard.cards[opponentBoard.cards.Count - 1]);
            effectSolver.SetTriggers(opponentBoard.cards[opponentBoard.cards.Count - 1]);
            var boardCreature = Instantiate(boardCreaturePrefab);
            boardCreature.tag = "OpponentOwned";
            boardCreature.GetComponent<BoardCreature>().PopulateWithInfo(opponentBoard.cards[opponentBoard.cards.Count - 1]);
            boardCreature.transform.parent = GameObject.Find("OpponentBoard").transform;
            opponentBoardCards.Add(boardCreature.GetComponent<BoardCreature>());
            RearrangeTopBoard(() =>
            {
                opponentHandZone.numCards -= 1;

                var cost = libraryCard.costs.Find(x => x is PayResourceCost);
                if (cost != null)
                {
                    var payResourceCost = cost as PayResourceCost;
                    opponentManaStat.baseValue -= payResourceCost.value;
                }

                if (msg.targetInfo != null && msg.targetInfo.Length >= 2)
                {
                    var targetingArrow = Instantiate(opponentTargetingArrowPrefab).GetComponent<OpponentTargetingArrow>();
                    targetingArrow.Begin(boardCreature.transform.position);
                    var playerCard = playerInfo.zones[msg.targetInfo[0]].cards.Find(x => x.instanceId == msg.targetInfo[1]);
                    var opponentCard = opponentInfo.zones[msg.targetInfo[0]].cards.Find(x => x.instanceId == msg.targetInfo[1]);
                    if (playerCard != null)
                    {
                        var playerBoardCard = playerBoardCards.Find(x => x.card.instanceId == playerCard.instanceId);
                        targetingArrow.SetTarget(playerBoardCard.gameObject);
                    }
                    else if (opponentCard != null)
                    {
                        var opponentBoardCard = opponentBoardCards.Find(x => x.card.instanceId == opponentCard.instanceId);
                        targetingArrow.SetTarget(opponentBoardCard.gameObject);
                    }

                    effectSolver.MoveCard(opponentInfo.netId, opponentBoard.cards[opponentBoard.cards.Count - 1], "Hand", "Board", new List<int>(msg.targetInfo));

                    StartCoroutine(RemoveOpponentTargetingArrow(targetingArrow));
                }
                else
                {
                    effectSolver.MoveCard(opponentInfo.netId, opponentBoard.cards[opponentBoard.cards.Count - 1], "Hand", "Board", new List<int>(msg.targetInfo));
                }
            });
        }
        else if (cardType.name == "Spell")
        {
            var opponentBoard = opponentInfo.namedZones["Board"];
            effectSolver.SetDestroyConditions(opponentBoard.cards[opponentBoard.cards.Count - 1]);
            effectSolver.SetTriggers(opponentBoard.cards[opponentBoard.cards.Count - 1]);
            var spellCard = Instantiate(spellCardViewPrefab);
            spellCard.transform.position = GameObject.Find("OpponentSpellsPivot").transform.position;
            spellCard.GetComponent<SpellCardView>().PopulateWithInfo(opponentBoard.cards[opponentBoard.cards.Count - 1]);
            spellCard.GetComponent<SpellCardView>().SetHighlightingEnabled(false);

            currentSpellCard = spellCard.GetComponent<SpellCardView>();

            var cost = libraryCard.costs.Find(x => x is PayResourceCost);
            if (cost != null)
            {
                var payResourceCost = cost as PayResourceCost;
                opponentManaStat.baseValue -= payResourceCost.value;
            }

            if (msg.targetInfo != null && msg.targetInfo.Length >= 2)
            {
                var targetingArrow = Instantiate(opponentTargetingArrowPrefab).GetComponent<OpponentTargetingArrow>();
                targetingArrow.Begin(spellCard.transform.position);
                var playerCard = playerInfo.zones[msg.targetInfo[0]].cards.Find(x => x.instanceId == msg.targetInfo[1]);
                var opponentCard = opponentInfo.zones[msg.targetInfo[0]].cards.Find(x => x.instanceId == msg.targetInfo[1]);
                if (playerCard != null)
                {
                    var playerBoardCard = playerBoardCards.Find(x => x.card.instanceId == playerCard.instanceId);
                    targetingArrow.SetTarget(playerBoardCard.gameObject);
                }
                else if (opponentCard != null)
                {
                    var opponentBoardCard = opponentBoardCards.Find(x => x.card.instanceId == opponentCard.instanceId);
                    targetingArrow.SetTarget(opponentBoardCard.gameObject);
                }

                effectSolver.MoveCard(opponentInfo.netId, opponentBoard.cards[opponentBoard.cards.Count - 1], "Hand", "Board", new List<int>(msg.targetInfo));

                StartCoroutine(RemoveOpponentTargetingArrow(targetingArrow));
            }
            else
            {
                effectSolver.MoveCard(opponentInfo.netId, opponentBoard.cards[opponentBoard.cards.Count - 1], "Hand", "Board", new List<int>(msg.targetInfo));
            }
        }
    }
    /// <summary>
    /// 相手のスペカを消すために2秒待つ
    /// </summary>
    /// <param name="spellCard"></param>
    /// <returns></returns>
    private IEnumerator RemoveOpponentSpellCard(SpellCardView spellCard)
    {
        yield return new WaitForSeconds(2.0f);
    }

    /// <summary>
    /// 相手の矢印を消すため2秒待つ
    /// </summary>
    /// <param name="arrow"></param>
    /// <returns></returns>
    private IEnumerator RemoveOpponentTargetingArrow(TargetingArrow arrow)
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(arrow.gameObject);
    }

    /// <summary>
    /// プレイヤーが攻撃された時の処理
    /// </summary>
    /// <param name="msg"></param>
    public override void OnPlayerAttacked(PlayerAttackedMessage msg)
    {
        base.OnPlayerAttacked(msg);

        var attackingCard = opponentBoardCards.Find(x => x.card.instanceId == msg.attackingCardInstanceId);
        if (attackingCard != null)
        {
            CombatAnimation.PlayFightAnimation(attackingCard.gameObject, GameObject.Find("Player/Avatar"), 0.1f, () =>
            {
                effectSolver.FightPlayer(msg.attackingPlayerNetId, msg.attackingCardInstanceId);
            });
        }
    }

    /// <summary>
    /// ミニオンがミニオンに攻撃された時の処理
    /// </summary>
    /// <param name="msg"></param>
    public override void OnCreatureAttacked(CreatureAttackedMessage msg)
    {
        base.OnCreatureAttacked(msg);
        var attackingCard = opponentBoardCards.Find(x => x.card.instanceId == msg.attackingCardInstanceId);
        var attackedCard = playerBoardCards.Find(x => x.card.instanceId == msg.attackedCardInstanceId);
        if (attackingCard != null && attackedCard != null)
        {
            CombatAnimation.PlayFightAnimation(attackingCard.gameObject, attackedCard.gameObject, 0.5f, () =>
            {
                effectSolver.FightCreature(msg.attackingPlayerNetId, attackingCard.card, attackedCard.card);
            });
        }
    }
    /// <summary>
    /// チャット受け取った時の処理
    /// </summary>
    /// <param name="senderNetId"></param>
    /// <param name="text"></param>
    public override void OnReceiveChatText(NetworkInstanceId senderNetId, string text)
    {
        chatPopup.SendText(senderNetId, text);
    }






}
