using UnityEngine.Networking;

using CCGKit;

/// <summary>
///  This server handler is responsible for managing the network aspects of a combat between
///  two cards.
///このサーバーハンドラは、2枚のカード間の戦闘のネットワーク面を管理します。
/// 
/// Combat follow this sequence:
/// 戦闘は以下のシーケンスに従う
///     - A player selects a card that is eligible as an attacker during his turn and sends
///       this information to the server.
///     - プレイヤーは自分のターンの間に攻撃者として適格なカードを選択し、この情報をサーバに送信する。
///     - A player then selects a target player or card as the attack's target and sends this
///       information to the server.
///     -プレイヤーは攻撃の対象となるターゲットプレーヤーまたはカードを選択し、この情報をサーバーに送信します。
///     - The server then proceeds with resolving the attack authoritatively and updating all
///       the clients afterwards.
///     - その後、サーバーは攻撃を正式に解決し、その後すべてのクライアントを更新します。
///
/// This process is currently very much suited to the traditional way of resolving combats in
/// CCGs (the attacker's attack value is substracted from the attacked's defense value, and
/// vice versa). This is something we would like to expand upon in future updates to allow for
/// more varied/complex mechanics.
/// このプロセスは現在、CCGの戦闘を解決する伝統的な方法（攻撃者の攻撃値は攻撃された防御値から差し引かれ、逆も同様）
/// に非常に適しています。 これは、より多様な/複雑な仕組みを可能にするために将来のアップデートで拡張したいものです。
/// </summary>
public class CombatHandler : ServerHandler
{
    public CombatHandler(Server server) : base(server)
    {
    }

    public override void RegisterNetworkHandlers()
    {
        base.RegisterNetworkHandlers();
        NetworkServer.RegisterHandler(NetworkProtocol.FightPlayer, OnFightPlayer);
        NetworkServer.RegisterHandler(NetworkProtocol.FightCreature, OnFightCreature);
    }

    public override void UnregisterNetworkHandlers()
    {
        base.UnregisterNetworkHandlers();
        NetworkServer.UnregisterHandler(NetworkProtocol.FightCreature);
        NetworkServer.UnregisterHandler(NetworkProtocol.FightPlayer);
    }

    /// <summary>
    /// ミニオンがプレイヤーに対して攻撃する時の処理
    /// </summary>
    /// <param name="netMsg"></param>
    public virtual void OnFightPlayer(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<FightPlayerMessage>();
        if (msg != null)
        {
            // Only the current player can fight.
            if (netMsg.conn.connectionId != server.gameState.currentPlayer.connectionId)
            {
                return;
            }

            var playerAttackedMsg = new PlayerAttackedMessage();
            playerAttackedMsg.attackingPlayerNetId = msg.attackingPlayerNetId;
            playerAttackedMsg.attackingCardInstanceId = msg.cardInstanceId;
            server.SafeSendToClient(server.gameState.currentOpponent, NetworkProtocol.PlayerAttacked, playerAttackedMsg);

            server.effectSolver.FightPlayer(msg.attackingPlayerNetId, msg.cardInstanceId);
        }
    }

    /// <summary>
    /// ミニオンがミニオンを攻撃する時の処理
    /// </summary>
    /// <param name="netMsg"></param>
    public virtual void OnFightCreature(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<FightCreatureMessage>();
        if (msg != null)
        {
            // Only the current player can fight.
            if (netMsg.conn.connectionId != server.gameState.currentPlayer.connectionId)
            {
                return;
            }

            var creatureAttackedMsg = new CreatureAttackedMessage();

            creatureAttackedMsg.attackingPlayerNetId = msg.attackingPlayerNetId;
            creatureAttackedMsg.attackingCardInstanceId = msg.attackingCardInstanceId;
            creatureAttackedMsg.attackedCardInstanceId = msg.attackedCardInstanceId;
            server.SafeSendToClient(server.gameState.currentOpponent, NetworkProtocol.CreatureAttacked, creatureAttackedMsg);

            var attackingCard = server.gameState.currentPlayer.namedZones["Board"].cards.Find(x => x.instanceId == msg.attackingCardInstanceId);
            var attackedCard = server.gameState.currentOpponent.namedZones["Board"].cards.Find(x => x.instanceId == msg.attackedCardInstanceId);
            if (attackingCard != null && attackedCard != null)
            {
                server.effectSolver.FightCreature(msg.attackingPlayerNetId, attackingCard, attackedCard);
            }
        }
    }
}