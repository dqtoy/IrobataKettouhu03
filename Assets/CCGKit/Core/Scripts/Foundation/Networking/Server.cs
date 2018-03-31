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
    /// �}���`�v���C���[�Q�[���ŃQ�[���f�[�^�ƃ��W�b�N�𑀍삷�錠�Ђ̂���Q�[���T�[�o�[�B ����́A�I�����C���R���N�^�u���J�[�h�Q�[���ŕK�v�Ƃ�����{�I�ȋ@�\�A
    /// ���Ȃ킿�^�[���V�[�P���X�̊Ǘ��ƃJ�[�h�G�t�F�N�g�̃A�v���P�[�V������񋟂��܂��B �v���W�F�N�g�S�̂̍\���́A
    /// �T�[�o�[�������Ȃ��̂ł���Ƃ��������Ɋ�Â��Ă��܂��B �n�b�L���O��h�~���邽�߂ɁA�N���C�A���g�͊�{�I�ɁA
    /// �T�[�o�[���ɂ��ׂẴN���e�B�J���ȃQ�[�����W�b�N�����s����Ă���ԂɁA�v���[���[�̓��͂��T�[�o�[�ɑ��M���A��ʏ�̃Q�[���̎��o�I�ȏ�Ԃ��X�V���邱�Ƃɐ�������܂��B
    /// 
    /// The goal is to provide useful default behavior that can be applied to a wide spectrum of games while
    /// also allowing further specialization via subclassing.
    /// 
    /// �ڕW�́A���܂��܂ȃQ�[���ɓK�p�ł���֗��ȃf�t�H���g�̓����񋟂������ŁA�T�u�N���X���ɂ�邳��ɓ��ꉻ���\�ɂ��邱�Ƃł��B
    /// </summary>
    public class Server : NetworkBehaviour
    {
        /// <summary>
        /// The duration of a turn in a game (in seconds).
        /// �Q�[�����̃^�[���̎������ԁi�b�P�ʁj�B
        /// </summary>
        /// // ����get,set�̏������̓I�[�g�v���p�e�B�ƌĂ΂��
        /// // �O������turnDuration�𑀍�(�l�̑���A�擾)���������ɕK�v
        /// //�Q�ƁFhttp://unitygeek.hatenablog.com/entry/2017/04/15/143053
        /// //�v���p�e�B�Ƃ��Ēl�������Ă��܂��΁A�ȍ~�́u"turnDuration"�v�Œl���Q�Ƃł���
        /// //��F  Parameter = 5;                        //�v���p�e�B�Ƃ��đ��
        /// //      Debug.Log("Parameter " + Parameter);  //�v���p�e�B�Ƃ��Ēl���擾
        /// //���ʁFParameter 5
        public int turnDuration { get; private set; }

        /// <summary>
        /// Index of the current player in the list of players.
        /// �v���C���[�̃��X�g���̌��݂̃v���[���[�̃C���f�b�N�X�B
        /// </summary>
        protected int currentPlayerIndex;

        /// <summary>
        /// Holds the entire state of the game.
        /// �Q�[���S�̂̏�Ԃ�ێ����܂��B
        /// </summary>
        public GameState gameState = new GameState();

        /// <summary>
        /// The effect solver used to resolve card effects.
        /// �J�[�h�G�t�F�N�g���������邽�߂Ɏg�p�����G�t�F�N�g�\���o�[�B
        /// </summary>
        public EffectSolver effectSolver { get; protected set; }

        /// <summary>
        /// List of server handler classes.
        /// �T�[�o�[�E�n���h���[�E�N���X�̃��X�g�B
        /// </summary>
        protected List<ServerHandler> handlers = new List<ServerHandler>();

        /// <summary>
        /// Current turn.
        /// ���݂̃^�[���B
        /// </summary>
        protected int currentTurn;

        /// <summary>
        /// True if the game has finished; false otherwise.
        /// �Q�[�����I�������ꍇ��true�B �����łȂ����false�B
        /// </summary>
        protected bool gameFinished;

        /// <summary>
        /// Cached reference to the currently-executing turn coroutine.
        /// ���ݎ��s���̃^�[���R���[�`���ւ̃L���b�V�����ꂽ�Q�ƁB
        /// </summary>
        protected Coroutine turnCoroutine;

        /// <summary>
        /// Called when the server starts listening.
        /// �T�[�o�[�������̊m�F���J�n����Ƃ��ɌĂяo����܂��B
        /// </summary>
        public override void OnStartServer()
        {
            //base:�p�����ł���NetworkBehaviour
            base.OnStartServer();

            LoadGameConfiguration();
            AddServerHandlers();
            RegisterServerHandlers();
        }

        /// <summary>
        /// Loads the game configuration.
        /// �Q�[���̐ݒ�(�^�[������J�[�h����A�J�[�h�ꗗ��)��ǂݍ��݂܂��B
        /// </summary>
        protected virtual void LoadGameConfiguration()
        {
            var gameConfig = GameManager.Instance.config;
            turnDuration = gameConfig.properties.turnDuration;
        }

        /// <summary>
        /// Adds the server handlers that are actually responsible of implementing the server's logic.
        /// ���ۂɃT�[�o�[�̃��W�b�N�̎�����S������T�[�o�[�n���h���[��ǉ����܂��B
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
        /// �T�[�o�[�����b�X���ɋ����̂��郁�b�Z�[�W�̃l�b�g���[�N�n���h���[��o�^���܂�
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
        /// �T�[�o�[�����b�X���ɋ����̂��郁�b�Z�[�W�̃l�b�g���[�N�n���h���[�̓o�^���������܂��B
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
        /// ���̊֐��́ANetworkBehaviour���j�������Ƃ��ɌĂяo����܂��B
        /// </summary>
        protected virtual void OnDestroy()
        {
            UnregisterServerHandlers();
        }

        /// <summary>
        /// Starts the multiplayer game. This is automatically called when the appropriate number of players
        /// have joined a room.
        /// �}���`�v���C���[�Q�[�����J�n���܂��B ����́A�K�؂Ȑ��̃v���[���[�������ɓ������Ƃ��Ɏ����I�ɌĂяo����܂��B
        /// </summary>
        public virtual void StartGame()
        {
            Logger.Log("Game has started.");

            // Start with turn 1.
            currentTurn = 1;

            var players = gameState.players;

            // Create an array with all the player nicknames.
            //���ׂẴv���C���[�̃j�b�N�l�[���Ŕz����쐬���܂��B
            var playerNicknames = new List<string>(players.Count);
            foreach (var player in players)
            {
                playerNicknames.Add(player.nickname);
            }

            // Set the current player and opponents.
            // ���݂̃v���C���[�Ƒ����ݒ肵�܂��B
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
            // �Q�[���̊J�n�A�N�V���������s���܂��B
            foreach (var action in GameManager.Instance.config.properties.gameStartActions)
            {
                ExecuteGameAction(action);
            }

            // Send a StartGame message to all the connected players.
            //�ڑ�����Ă��邷�ׂẴv���C���[��StartGame���b�Z�[�W�𑗐M���܂��B
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
            //�^�[���V�[�P���X�R���[�`�����N�����܂��B
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
            //�v���[���[�̃X�^�b�c�����R�s�[���܂��B
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
        /// ���݂̃Q�[�����I�����܂��B
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
        /// �^�[���V�[�P���X�𐳎��ɋ쓮����R���[�`�������s���܂��B
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
        /// �V�����Q�[���^�[�����J�n���܂��B
        /// </summary>
        protected virtual void StartTurn()
        {
            Logger.Log("Start turn for player " + currentPlayerIndex + ".");

            var players = gameState.players;

            // Update the current player and opponents.
            //���݂̃v���C���[�Ƒ�����X�V���܂��B
            gameState.currentPlayer = players[currentPlayerIndex];
            gameState.currentOpponent = players.Find(x => x != gameState.currentPlayer);

            gameState.currentPlayer.numTurn += 1;

            // Execute the turn start actions.
            //�^�[���J�n�A�N�V���������s���܂��B
            foreach (var action in GameManager.Instance.config.properties.turnStartActions)
            {
                ExecuteGameAction(action);
            }

            // Run any code that needs to be executed at turn start time.
            //�^�[���J�n���Ɏ��s����K�v�̂���R�[�h�����s���܂��B
            PerformTurnStartStateInitialization();

            // Let the server handlers know the turn has started.
            //�^�[�����J�n�������Ƃ��T�[�o�[�n���h���[�ɒm�点�܂��B
            for (var i = 0; i < handlers.Count; i++)
            {
                handlers[i].OnStartTurn();
            }

            effectSolver.OnTurnStarted();

            // Send a StartTurn message to all the connected players.
            //�ڑ��������ׂẴv���[���[��StartTurn���b�Z�[�W�𑗐M���܂��B
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
        /// ���̃��\�b�h�́A�^�[���E�X�^�[�g�ŗL�̏��������W�b�N�����s���邽�߂ɃT�u�N���X�Ŏg�p�ł��܂��B
        /// </summary>
        protected virtual void PerformTurnStartStateInitialization()
        {
        }

        /// <summary>
        /// Ends the current game turn.
        /// ���݂̃Q�[���^�[�����I������B
        /// </summary>
        protected virtual void EndTurn()
        {
            Logger.Log("End turn for player " + currentPlayerIndex + ".");

            // Let the server handlers know the turn has ended.
            //�^�[�����I���������Ƃ��T�[�o�n���h���ɒm�点�܂��B
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
            //���ׂẴv���C���[��EndTurn���b�Z�[�W�𑗐M���܂��B
            foreach (var player in players)
            {
                var msg = new EndTurnMessage();
                msg.recipientNetId = player.netId;
                msg.isRecipientTheActivePlayer = player == players[currentPlayerIndex];
                SafeSendToClient(player, NetworkProtocol.EndTurn, msg);
            }

            // Switch to next player.
            //���̃v���C���[�ɐ؂�ւ��܂��B
            currentPlayerIndex += 1;
            if (currentPlayerIndex == players.Count)
            {
                currentPlayerIndex = 0;
                // Increase turn count.
                //�^�[���𑝂₷
                currentTurn += 1;
            }
        }

        /// <summary>
        /// Stops the current turn.
        /// ���݂̃^�[�����~���܂��B
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
        /// �w�肳�ꂽ�ڑ����ʎq�����v���[���[���T�[�o�[�ɐڑ�����ƌĂяo����܂��B
        /// </summary>
        /// <param name="connectionId">The player's connection identifier.</param>
        /// �v���[���[�̐ڑ����ʎq�B
        public virtual void OnPlayerConnected(int connectionId)
        {
            Logger.Log("Player with id " + connectionId + " connected to server.");
            /*var player = Players.Find(x => x.ConnectionId == connectionId);
            if (player != null)
                player.IsConnected = true;*/
        }

        /// <summary>
        /// Called when a player with the specified connection identifier disconnects from the server.
        /// �w�肳�ꂽ�ڑ����ʎq�����v���[���[���T�[�o�[����ؒf���ꂽ�Ƃ��ɌĂяo����܂��B
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
        /// �w�肳�ꂽ�Q�[���A�N�V���������s���܂��B
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
            //targetPlayers�̐������A�N�V��������������
            foreach (var player in targetPlayers)
            {
                action.Resolve(gameState, player);
            }
        }
    }
}
