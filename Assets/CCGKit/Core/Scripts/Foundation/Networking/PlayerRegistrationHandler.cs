using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace CCGKit
{
    /// <summary>
    /// This server handler manages the registration of new players into an open game.
    /// ���̃T�[�o�n���h���́A�V�K�v���C���̓o�^���I�[�v���Q�[���ɊǗ����܂��B
    /// </summary>
    public class PlayerRegistrationHandler : ServerHandler
    {
        public PlayerRegistrationHandler(Server server) : base(server)
        {
        }

        public override void RegisterNetworkHandlers()
        {
            base.RegisterNetworkHandlers();
            NetworkServer.RegisterHandler(NetworkProtocol.RegisterPlayer, OnRegisterPlayer);
        }

        public override void UnregisterNetworkHandlers()
        {
            base.UnregisterNetworkHandlers();
            NetworkServer.UnregisterHandler(NetworkProtocol.RegisterPlayer);
        }

        protected virtual void OnRegisterPlayer(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<RegisterPlayerMessage>();
            Assert.IsNotNull(msg);

            // If this player is already registered, ignore this message.
            //���̃v���[���[�����łɓo�^����Ă���ꍇ�́A���̃��b�Z�[�W�𖳎����Ă��������B
            var player = server.gameState.players.Find(x => x.netId == msg.netId);
            if (player != null)
            {
                return;
            }

            // Create a new player info for the registered player.
            //�o�^���ꂽ�v���[���[�̐V�����v���[���[�����쐬���܂��B
            player = new PlayerInfo();
            player.id = server.gameState.players.Count;
            player.connectionId = netMsg.conn.connectionId;
            player.netId = msg.netId;
            player.nickname = msg.name;
            player.isConnected = true;
            player.isHuman = msg.isHuman;

            var gameConfig = GameManager.Instance.config;

            // Set the player stats based on the generic player definition.
            //�W�F�l���b�N�v���[���[�̒�`�Ɋ�Â��ăv���[���[�̓��v����ݒ肵�܂��B
            foreach (var stat in gameConfig.playerStats)
            {
                var statCopy = new Stat();
                statCopy.name = stat.name;
                statCopy.statId = stat.id;
                statCopy.originalValue = stat.originalValue;
                statCopy.baseValue = stat.baseValue;
                statCopy.minValue = stat.minValue;
                statCopy.maxValue = stat.maxValue;
                player.stats[stat.id] = statCopy;
                player.namedStats[stat.name] = statCopy;
            }

            // Set the player zones based on the generic zone definitions.
            //�W�F�l���b�N�]�[���̒�`�Ɋ�Â��ăv���[���]�[����ݒ肵�܂��B
            var personalZones = gameConfig.gameZones.FindAll(x => x.owner != ZoneOwner.Shared);
            foreach (var zone in personalZones)
            {
                var zoneCopy = new RuntimeZone();
                zoneCopy.name = zone.name;
                zoneCopy.zoneId = zone.id;
                if (zone.hasMaxSize)
                {
                    zoneCopy.maxCards = zone.maxSize;
                }
                else
                {
                    zoneCopy.maxCards = int.MaxValue;
                }
                player.zones[zone.id] = zoneCopy;
                player.namedZones[zone.name] = zoneCopy;
            }

            foreach (var condition in gameConfig.properties.endGameConditions)
            {
                if (condition is PlayerStatEndGameCondition)
                {
                    var playerStatCondition = condition as PlayerStatEndGameCondition;
                    player.stats[playerStatCondition.statId].onValueChanged += (oldValue, newValue) =>
                    {
                        if (playerStatCondition.IsTrue(player))
                        {
                            server.EndGame(player, playerStatCondition.type);
                        }
                    };
                }
                else if (condition is CardsInZoneEndGameCondition)
                {
                    var cardsCondition = condition as CardsInZoneEndGameCondition;
                    player.zones[cardsCondition.zoneId].onZoneChanged += value =>
                    {
                        if (cardsCondition.IsTrue(player))
                        {
                            server.EndGame(player, cardsCondition.type);
                        }
                    };
                }
            }

            // Add the default deck.
            //�f�t�H���g�̃f�b�L��ǉ����܂��B
            var deckZoneId = gameConfig.gameZones.Find(x => x.name == "Deck").id;
            var deck = player.zones[deckZoneId];
            foreach (var id in msg.deck)
            {
                var card = new RuntimeCard();
                card.cardId = id;
                card.instanceId = player.currentCardInstanceId++;
                card.ownerPlayer = player;
                // Copy stats.
                var libraryCard = gameConfig.GetCard(id);
                foreach (var stat in libraryCard.stats)
                {
                    var statCopy = new Stat();
                    statCopy.statId = stat.statId;
                    statCopy.name = stat.name;
                    statCopy.originalValue = stat.originalValue;
                    statCopy.baseValue = stat.baseValue;
                    statCopy.minValue = stat.minValue;
                    statCopy.maxValue = stat.maxValue;
                    card.stats[stat.statId] = statCopy;
                    card.namedStats[stat.name] = statCopy;
                }
                // Copy keywords.
                foreach (var keyword in libraryCard.keywords)
                {
                    var keywordCopy = new RuntimeKeyword();
                    keywordCopy.keywordId = keyword.keywordId;
                    keywordCopy.valueId = keyword.valueId;
                    card.keywords.Add(keywordCopy);
                }

                deck.cards.Add(card);
            }

            // Add the new player to the server's list of players.
            //�V�����v���[���[���T�[�o�[�̃v���[���[�̃��X�g�ɒǉ����܂��B
            server.gameState.players.Add(player);
            Logger.Log("Player with id " + player.id + " has joined the game.");

            // When the appropriate number of players is registered, the game can start.
            //�K�؂Ȑ��̑I�肪�o�^�����ƁA�Q�[�����J�n���邱�Ƃ��ł���B
            if (server.gameState.players.Count == 2)
            {
                server.StartGame();
            }
        }
    }
}
