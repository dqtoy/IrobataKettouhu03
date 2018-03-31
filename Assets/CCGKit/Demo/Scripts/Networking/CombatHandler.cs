using UnityEngine.Networking;

using CCGKit;

/// <summary>
///  This server handler is responsible for managing the network aspects of a combat between
///  two cards.
///���̃T�[�o�[�n���h���́A2���̃J�[�h�Ԃ̐퓬�̃l�b�g���[�N�ʂ��Ǘ����܂��B
/// 
/// Combat follow this sequence:
/// �퓬�͈ȉ��̃V�[�P���X�ɏ]��
///     - A player selects a card that is eligible as an attacker during his turn and sends
///       this information to the server.
///     - �v���C���[�͎����̃^�[���̊ԂɍU���҂Ƃ��ēK�i�ȃJ�[�h��I�����A���̏����T�[�o�ɑ��M����B
///     - A player then selects a target player or card as the attack's target and sends this
///       information to the server.
///     -�v���C���[�͍U���̑ΏۂƂȂ�^�[�Q�b�g�v���[���[�܂��̓J�[�h��I�����A���̏����T�[�o�[�ɑ��M���܂��B
///     - The server then proceeds with resolving the attack authoritatively and updating all
///       the clients afterwards.
///     - ���̌�A�T�[�o�[�͍U���𐳎��ɉ������A���̌シ�ׂẴN���C�A���g���X�V���܂��B
///
/// This process is currently very much suited to the traditional way of resolving combats in
/// CCGs (the attacker's attack value is substracted from the attacked's defense value, and
/// vice versa). This is something we would like to expand upon in future updates to allow for
/// more varied/complex mechanics.
/// ���̃v���Z�X�͌��݁ACCG�̐퓬����������`���I�ȕ��@�i�U���҂̍U���l�͍U�����ꂽ�h��l���獷��������A�t�����l�j
/// �ɔ��ɓK���Ă��܂��B ����́A��葽�l��/���G�Ȏd�g�݂��\�ɂ��邽�߂ɏ����̃A�b�v�f�[�g�Ŋg�����������̂ł��B
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
    /// �~�j�I�����v���C���[�ɑ΂��čU�����鎞�̏���
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
    /// �~�j�I�����~�j�I�����U�����鎞�̏���
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