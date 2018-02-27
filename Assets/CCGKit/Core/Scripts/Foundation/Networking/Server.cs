using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

namespace CCGKit
{
    /// <summary>
    /// The authoritative game server, which is responsible for driving the game data and logic in a
    /// multiplayer game. It provides the fundamental functionality needed in an online collectible card
    /// game, namely management of the turn sequence and application of card effects. The entire project
    /// structure revolves around the fact that the server is authoritative; in order to prevent hacking,
    /// clients are fundamentally limited to sending the player input to the server and updating the visual
    /// state of the game on screen while all the critical game logic is performed on the server side.
    ///
    /// マルチプレイヤーゲームでゲームデータとロジックを操作する権威のあるゲームサーバー。 これは、オンラインコレクタブルカードゲームで必要とされる基本的な機能、
    /// すなわちターンシーケンスの管理とカードエフェクトのアプリケーションを提供します。 プロジェクト全体の構造は、
    /// サーバーが正式なものであるという事実に基づいています。 ハッキングを防止するために、クライアントは基本的に、
    /// サーバー側にすべてのクリティカルなゲームロジックが実行されている間に、プレーヤーの入力をサーバーに送信し、画面上のゲームの視覚的な状態を更新することに制限されます。
    /// 
    /// The goal is to provide useful default behavior that can be applied to a wide spectrum of games while
    /// also allowing further specialization via subclassing.
    /// 
    /// 目標は、さまざまなゲームに適用できる便利なデフォルトの動作を提供する一方で、サブクラス化によるさらに特殊化を可能にすることです。
    /// </summary>
    public class Server : NetworkBehaviour
    {
        /// <summary>
        /// The duration of a turn in a game (in seconds).
        /// ゲーム中のターンの持続時間（秒単位）。
        /// </summary>
        /// // このget,setの書き方はオートプロパティと呼ばれる
        /// // 外部からturnDurationを操作(値の代入、取得)したい時に必要
        /// //参照：http://unitygeek.hatenablog.com/entry/2017/04/15/143053
        /// //プロパティとして値を代入してしまえば、以降は「"turnDuration"」で値を参照できる
        /// //例：  Parameter = 5;                        //プロパティとして代入
        /// //      Debug.Log("Parameter " + Parameter);  //プロパティとして値を取得
        /// //結果：Parameter 5
        public int turnDuration { get; private set; }

        /// <summary>
        /// Index of the current player in the list of players.
        /// プレイヤーのリスト内の現在のプレーヤーのインデックス。
        /// </summary>
        protected int currentPlayerIndex;

        /// <summary>
        /// Holds the entire state of the game.
        /// ゲーム全体の状態を保持します。
        /// </summary>
        public GameState gameState = new GameState();

        /// <summary>
        /// The effect solver used to resolve card effects.
        /// カードエフェクトを解決するために使用されるエフェクトソルバー。
        /// </summary>
        public EffectSolver effectSolver { get; protected set; }

        /// <summary>
        /// List of server handler classes.
        /// サーバー・ハンドラー・クラスのリスト。
        /// </summary>
        protected List<ServerHandler> handlers = new List<ServerHandler>();

        /// <summary>
        /// Current turn.
        /// 現在のターン。
        /// </summary>
        protected int currentTurn;

        /// <summary>
        /// True if the game has finished; false otherwise.
        /// ゲームが終了した場合はtrue。 そうでなければfalse。
        /// </summary>
        protected bool gameFinished;

        /// <summary>
        /// Cached reference to the currently-executing turn coroutine.
        /// 現在実行中のターンコルーチンへのキャッシュされた参照。
        /// </summary>
        protected Coroutine turnCoroutine;

        /// <summary>
        /// Called when the server starts listening.
        /// サーバーが応答の確認を開始するときに呼び出されます。
        /// </summary>
        public override void OnStartServer()
        {
            //base:継承元であるNetworkBehaviour
            base.OnStartServer();

            LoadGameConfiguration();
            AddServerHandlers();
            RegisterServerHandlers();
        }

        /// <summary>
        /// Loads the game configuration.
        /// ゲームの設定(ターン数やカード上限、カード一覧等)を読み込みます。
        /// </summary>
        protected virtual void LoadGameConfiguration()
        {
            var gameConfig = GameManager.Instance.config;
            turnDuration = gameConfig.properties.turnDuration;
        }

        /// <summary>
        /// Adds the server handlers that are actually responsible of implementing the server's logic.
        /// 実際にサーバーのロジックの実装を担当するサーバーハンドラーを追加します。
        /// </summary>
        protected virtual void AddServerHandlers()
        {
            handlers.Add(new PlayerRegistrationHandler(this));
            handlers.Add(new TurnSequenceHandler(this));
            handlers.Add(new EffectSolverHandler(this));
            handlers.Add(new ChatHandler(this));
        }

        /// <summary>
        /// Registers the network handlers for the messages the server is interested in listening to.
        /// サーバーがリッスンに興味のあるメッセージのネットワークハンドラーを登録します
        /// </summary>
        protected virtual void RegisterServerHandlers()
        {
            foreach (var handler in handlers)
            {
                handler.RegisterNetworkHandlers();
            }
        }

        /// <summary>
        /// Unregisters the network handlers for the messages the server is interested in listening to.
        /// サーバーがリッスンに興味のあるメッセージのネットワークハンドラーの登録を解除します。
        /// </summary>
        protected virtual void UnregisterServerHandlers()
        {
            foreach (var handler in handlers)
            {
                handler.UnregisterNetworkHandlers();
            }
            handlers.Clear();
        }

        /// <summary>
        /// This function is called when the NetworkBehaviour will be destroyed.
        /// この関数は、NetworkBehaviourが破棄されるときに呼び出されます。
        /// </summary>
        protected virtual void OnDestroy()
        {
            UnregisterServerHandlers();
        }

        /// <summary>
        /// Starts the multiplayer game. This is automatically called when the appropriate number of players
        /// have joined a room.
        /// マルチプレイヤーゲームを開始します。 これは、適切な数のプレーヤーが部屋に入ったときに自動的に呼び出されます。
        /// </summary>
        public virtual void StartGame()
        {
            Logger.Log("Game has started.");

            // Start with turn 1.
            currentTurn = 1;

            var players = gameState.players;

            // Create an array with all the player nicknames.
            //すべてのプレイヤーのニックネームで配列を作成します。
            var playerNicknames = new List<string>(players.Count);
            foreach (var player in players)
            {
                playerNicknames.Add(player.nickname);
            }

            // Set the current player and opponents.
            // 現在のプレイヤーと相手を設定します。
            gameState.currentPlayer = players[currentPlayerIndex];
            gameState.currentOpponent = players.Find(x => x != gameState.currentPlayer);

            var rngSeed = System.Environment.TickCount;
            effectSolver = new EffectSolver(gameState, rngSeed);

            foreach (var player in players)
            {
                effectSolver.SetTriggers(player);
                foreach (var zone in player.zones)
                {
                    foreach (var card in zone.Value.cards)
                    {
                        effectSolver.SetDestroyConditions(card);
                        effectSolver.SetTriggers(card);
                    }
                }
            }

            // Execute the game start actions.
            // ゲームの開始アクションを実行します。
            foreach (var action in GameManager.Instance.config.properties.gameStartActions)
            {
                ExecuteGameAction(action);
            }

            // Send a StartGame message to all the connected players.
            //接続されているすべてのプレイヤーにStartGameメッセージを送信します。
            for (var i = 0; i < players.Count; i++)
            {
                var player = players[i];
                var msg = new StartGameMessage();
                msg.recipientNetId = player.netId;
                msg.playerIndex = i;
                msg.turnDuration = turnDuration;
                msg.nicknames = playerNicknames.ToArray();
                msg.player = GetPlayerNetworkState(player);
                msg.opponent = GetOpponentNetworkState(players.Find(x => x != player));
                msg.rngSeed = rngSeed;
                SafeSendToClient(player, NetworkProtocol.StartGame, msg);
            }

            // Start running the turn sequence coroutine.
            //ターンシーケンスコルーチンを起動します。
            turnCoroutine = StartCoroutine(RunTurn());
        }

        public virtual void SendPlayerInfoToAllClients(PlayerInfo player)
        {
            var playerStateMsg = new PlayerGameStateMessage();
            playerStateMsg.player = GetPlayerNetworkState(player);
            SafeSendToClient(player, NetworkProtocol.PlayerState, playerStateMsg);
        }

        protected virtual NetPlayerInfo GetPlayerNetworkState(PlayerInfo player)
        {
            var netPlayer = new NetPlayerInfo();
            netPlayer.id = player.id;
            netPlayer.netId = player.netId;

            // Copy player stats.
            //プレーヤーのスタッツ情報をコピーします。
            var stats = new NetStat[player.stats.Count];
            var idx = 0;
            foreach (var entry in player.stats)
            {
                var stat = entry.Value;
                stats[idx++] = NetworkingUtils.GetNetStat(stat);
            }
            netPlayer.stats = stats;

            var gameConfig = GameManager.Instance.config;
            // Copy player zones.
            var staticZones = new List<NetStaticZone>();
            var dynamicZones = new List<NetDynamicZone>();
            foreach (var zonePair in player.zones)
            {
                var zone = zonePair.Value;
                var zoneDefinition = gameConfig.gameZones.Find(x => x.id == zone.zoneId);
                if (zoneDefinition.type == ZoneType.Static)
                {
                    var staticZone = new NetStaticZone();
                    staticZone.zoneId = zone.zoneId;
                    if (zoneDefinition.ownerVisibility == ZoneOwnerVisibility.Visible)
                    {
                        staticZone.cards = new NetStaticCard[zone.cards.Count];
                        var i = 0;
                        foreach (var card in zone.cards)
                        {
                            var netCard = new NetStaticCard();
                            netCard.cardId = card.cardId;
                            netCard.instanceId = card.instanceId;
                            staticZone.cards[i++] = netCard;
                        }
                    }
                    staticZone.numCards = zone.cards.Count;
                    staticZones.Add(staticZone);
                }
                else if (zoneDefinition.type == ZoneType.Dynamic)
                {
                    var dynamicZone = new NetDynamicZone();
                    dynamicZone.zoneId = zone.zoneId;
                    dynamicZone.cards = new NetCard[zone.cards.Count];
                    for (var j = 0; j < zone.cards.Count; j++)
                    {
                        var card = zone.cards[j];
                        var netCard = new NetCard();
                        netCard.cardId = card.cardId;
                        netCard.instanceId = card.instanceId;
                        netCard.stats = new NetStat[card.stats.Count];
                        idx = 0;
                        foreach (var entry in card.stats)
                        {
                            netCard.stats[idx++] = NetworkingUtils.GetNetStat(entry.Value);
                        }
                        netCard.keywords = new NetKeyword[card.keywords.Count];
                        idx = 0;
                        foreach (var entry in card.keywords)
                        {
                            netCard.keywords[idx++] = NetworkingUtils.GetNetKeyword(entry);
                        }
                        dynamicZone.cards[j] = netCard;
                    }
                    dynamicZone.numCards = zone.cards.Count;
                    dynamicZones.Add(dynamicZone);
                }
            }
            netPlayer.staticZones = staticZones.ToArray();
            netPlayer.dynamicZones = dynamicZones.ToArray();

            return netPlayer;
        }

        protected virtual NetPlayerInfo GetOpponentNetworkState(PlayerInfo player)
        {
            var netOpponent = new NetPlayerInfo();
            netOpponent.id = player.id;
            netOpponent.netId = player.netId;

            // Copy player stats.
            var stats = new NetStat[player.stats.Count];
            var idx = 0;
            foreach (var entry in player.stats)
            {
                var stat = entry.Value;
                stats[idx++] = NetworkingUtils.GetNetStat(stat);
            }
            netOpponent.stats = stats;

            // Copy player zones.
            var gameConfig = GameManager.Instance.config;
            var staticZones = new List<NetStaticZone>();
            var dynamicZones = new List<NetDynamicZone>();
            foreach (var zonePair in player.zones)
            {
                var zone = zonePair.Value;
                var zoneDefinition = gameConfig.gameZones.Find(x => x.id == zone.zoneId);
                if (zoneDefinition.type == ZoneType.Static)
                {
                    var staticZone = new NetStaticZone();
                    staticZone.zoneId = zone.zoneId;
                    if (zoneDefinition.opponentVisibility == ZoneOpponentVisibility.Visible)
                    {
                        staticZone.cards = new NetStaticCard[zone.cards.Count];
                        var i = 0;
                        foreach (var card in zone.cards)
                        {
                            var netCard = new NetStaticCard();
                            netCard.cardId = card.cardId;
                            netCard.instanceId = card.instanceId;
                            staticZone.cards[i++] = netCard;
                        }
                    }
                    staticZone.numCards = zone.cards.Count;
                    staticZones.Add(staticZone);
                }
                else if (zoneDefinition.type == ZoneType.Dynamic)
                {
                    var dynamicZone = new NetDynamicZone();
                    dynamicZone.zoneId = zone.zoneId;
                    if (zoneDefinition.opponentVisibility == ZoneOpponentVisibility.Visible)
                    {
                        dynamicZone.cards = new NetCard[zone.cards.Count];
                        for (var j = 0; j < zone.cards.Count; j++)
                        {
                            var card = zone.cards[j];
                            var netCard = new NetCard();
                            netCard.cardId = card.cardId;
                            netCard.instanceId = card.instanceId;
                            netCard.stats = new NetStat[card.stats.Count];
                            idx = 0;
                            foreach (var entry in card.stats)
                            {
                                netCard.stats[idx++] = NetworkingUtils.GetNetStat(entry.Value);
                            }
                            netCard.keywords = new NetKeyword[card.keywords.Count];
                            idx = 0;
                            foreach (var entry in card.keywords)
                            {
                                netCard.keywords[idx++] = NetworkingUtils.GetNetKeyword(entry);
                            }
                            dynamicZone.cards[j] = netCard;
                        }
                    }
                    dynamicZone.numCards = zone.cards.Count;
                    dynamicZones.Add(dynamicZone);
                }
            }
            netOpponent.staticZones = staticZones.ToArray();
            netOpponent.dynamicZones = dynamicZones.ToArray();

            return netOpponent;
        }

        /// <summary>
        /// Ends the current game.
        /// 現在のゲームを終了します。
        /// </summary>
        /// <param name="player">The player that has won/lost.</param>
        /// <param name="type">The result of the game (win/loss).</param>
        public virtual void EndGame(PlayerInfo player, EndGameType type)
        {
            gameFinished = true;
            var msg = new EndGameMessage();
            switch (type)
            {
                case EndGameType.Win:
                    msg.winnerPlayerIndex = player.netId;
                    break;

                case EndGameType.Loss:
                    msg.winnerPlayerIndex = gameState.players.Find(x => x != player).netId;
                    break;
            }
            NetworkServer.SendToAll(NetworkProtocol.EndGame, msg);
        }

        /// <summary>
        /// Runs the coroutine that authoritatively drives the turn sequence.
        /// ターンシーケンスを正式に駆動するコルーチンを実行します。
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator RunTurn()
        {
            while (!gameFinished)
            {
                StartTurn();
                yield return new WaitForSeconds(turnDuration);
                EndTurn();
            }
        }

        /// <summary>
        /// Starts a new game turn.
        /// 新しいゲームターンを開始します。
        /// </summary>
        protected virtual void StartTurn()
        {
            Logger.Log("Start turn for player " + currentPlayerIndex + ".");

            var players = gameState.players;

            // Update the current player and opponents.
            //現在のプレイヤーと相手を更新します。
            gameState.currentPlayer = players[currentPlayerIndex];
            gameState.currentOpponent = players.Find(x => x != gameState.currentPlayer);

            gameState.currentPlayer.numTurn += 1;

            // Execute the turn start actions.
            //ターン開始アクションを実行します。
            foreach (var action in GameManager.Instance.config.properties.turnStartActions)
            {
                ExecuteGameAction(action);
            }

            // Run any code that needs to be executed at turn start time.
            //ターン開始時に実行する必要のあるコードを実行します。
            PerformTurnStartStateInitialization();

            // Let the server handlers know the turn has started.
            //ターンが開始したことをサーバーハンドラーに知らせます。
            for (var i = 0; i < handlers.Count; i++)
            {
                handlers[i].OnStartTurn();
            }

            effectSolver.OnTurnStarted();

            // Send a StartTurn message to all the connected players.
            //接続したすべてのプレーヤーにStartTurnメッセージを送信します。
            for (var i = 0; i < players.Count; i++)
            {
                var player = players[i];
                var msg = new StartTurnMessage();
                msg.recipientNetId = player.netId;
                msg.isRecipientTheActivePlayer = player == gameState.currentPlayer;
                msg.turn = currentTurn;
                msg.player = GetPlayerNetworkState(player);
                msg.opponent = GetOpponentNetworkState(players.Find(x => x != player));
                SafeSendToClient(player, NetworkProtocol.StartTurn, msg);
            }
        }

        /// <summary>
        /// This method can be used by subclasses to perform turn-start-specific initialization logic.
        /// このメソッドは、ターン・スタート固有の初期化ロジックを実行するためにサブクラスで使用できます。
        /// </summary>
        protected virtual void PerformTurnStartStateInitialization()
        {
        }

        /// <summary>
        /// Ends the current game turn.
        /// 現在のゲームターンを終了する。
        /// </summary>
        protected virtual void EndTurn()
        {
            Logger.Log("End turn for player " + currentPlayerIndex + ".");

            // Let the server handlers know the turn has ended.
            //ターンが終了したことをサーバハンドラに知らせます。
            for (var i = 0; i < handlers.Count; i++)
                handlers[i].OnEndTurn();

            effectSolver.OnTurnEnded();

            var players = gameState.players;

            foreach (var player in players)
            {
                foreach (var entry in player.stats)
                {
                    entry.Value.OnEndTurn();
                }
            }

            foreach (var zone in players[currentPlayerIndex].zones)
            {
                foreach (var card in zone.Value.cards)
                {
                    foreach (var stat in card.stats)
                    {
                        stat.Value.OnEndTurn();
                    }
                }
            }

            // Send the EndTurn message to all players.
            //すべてのプレイヤーにEndTurnメッセージを送信します。
            foreach (var player in players)
            {
                var msg = new EndTurnMessage();
                msg.recipientNetId = player.netId;
                msg.isRecipientTheActivePlayer = player == players[currentPlayerIndex];
                SafeSendToClient(player, NetworkProtocol.EndTurn, msg);
            }

            // Switch to next player.
            //次のプレイヤーに切り替えます。
            currentPlayerIndex += 1;
            if (currentPlayerIndex == players.Count)
            {
                currentPlayerIndex = 0;
                // Increase turn count.
                //ターンを増やす
                currentTurn += 1;
            }
        }

        /// <summary>
        /// Stops the current turn.
        /// 現在のターンを停止します。
        /// </summary>
        public virtual void StopTurn()
        {
            if (turnCoroutine != null)
                StopCoroutine(turnCoroutine);
            EndTurn();
            turnCoroutine = StartCoroutine(RunTurn());
        }

        /// <summary>
        /// Called when a player with the specified connection identifier connects to the server.
        /// 指定された接続識別子を持つプレーヤーがサーバーに接続すると呼び出されます。
        /// </summary>
        /// <param name="connectionId">The player's connection identifier.</param>
        /// プレーヤーの接続識別子。
        public virtual void OnPlayerConnected(int connectionId)
        {
            Logger.Log("Player with id " + connectionId + " connected to server.");
            /*var player = Players.Find(x => x.ConnectionId == connectionId);
            if (player != null)
                player.IsConnected = true;*/
        }

        /// <summary>
        /// Called when a player with the specified connection identifier disconnects from the server.
        /// 指定された接続識別子を持つプレーヤーがサーバーから切断されたときに呼び出されます。
        /// </summary>
        /// <param name="connectionId">The player's connection identifier.</param>
        public virtual void OnPlayerDisconnected(int connectionId)
        {
            Logger.Log("Player with id " + connectionId + " disconnected from server.");
            /*var player = Players.Find(x => x.ConnectionId == connectionId);
            if (player != null)
                player.IsConnected = false;*/
        }

        public virtual void SafeSendToClient(PlayerInfo player, short msgType, MessageBase msg)
        {
            if (player != null && player.isConnected)
            {
                NetworkServer.SendToClient(player.connectionId, msgType, msg);
            }
        }

        /// <summary>
        /// Executes the specified game action.
        /// 指定されたゲームアクションを実行します。
        /// </summary>
        /// <param name="action">Game action to execute.</param>
        protected void ExecuteGameAction(GameAction action)
        {
            var targetPlayers = new List<PlayerInfo>();
            switch (action.target)
            {
                case GameActionTarget.CurrentPlayer:
                    targetPlayers.Add(gameState.currentPlayer);
                    break;

                case GameActionTarget.CurrentOpponent:
                    targetPlayers.Add(gameState.currentOpponent);
                    break;

                case GameActionTarget.AllPlayers:
                    targetPlayers = gameState.players;
                    break;
            }
            //targetPlayersの数だけアクションを解決する
            foreach (var player in targetPlayers)
            {
                action.Resolve(gameState, player);
            }
        }
    }
}
