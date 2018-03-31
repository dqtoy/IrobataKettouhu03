using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace CCGKit
{
    /// <summary>
    /// This server handler manages the registration of new players into an open game.
    /// このサーバハンドラは、新規プレイヤの登録をオープンゲームに管理します。
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
            //このプレーヤーがすでに登録されている場合は、このメッセージを無視してください。
            var player = server.gameState.players.Find(x => x.netId == msg.netId);
            if (player != null)
            {
                return;
            }

            // Create a new player info for the registered player.
            //登録されたプレーヤーの新しいプレーヤー情報を作成します。
            player = new PlayerInfo();
            player.id = server.gameState.players.Count;
            player.connectionId = netMsg.conn.connectionId;
            player.netId = msg.netId;
            player.nickname = msg.name;
            player.isConnected = true;
            player.isHuman = msg.isHuman;

            var gameConfig = GameManager.Instance.config;

            // Set the player stats based on the generic player definition.
            //ジェネリックプレーヤーの定義に基づいてプレーヤーの統計情報を設定します。
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
            //ジェネリックゾーンの定義に基づいてプレーヤゾーンを設定します。
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
            //デフォルトのデッキを追加します。
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
            //新しいプレーヤーをサーバーのプレーヤーのリストに追加します。
            server.gameState.players.Add(player);
            Logger.Log("Player with id " + player.id + " has joined the game.");

            // When the appropriate number of players is registered, the game can start.
            //適切な数の選手が登録されると、ゲームを開始することができる。
            if (server.gameState.players.Count == 2)
            {
                server.StartGame();
            }
        }
    }
}
