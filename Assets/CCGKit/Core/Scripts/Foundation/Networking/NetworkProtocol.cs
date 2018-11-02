using UnityEngine.Networking;

namespace CCGKit
{
    /// <summary>
    /// This utility class defines all the unique identifiers for every network message in a game.
    /// ���̃��[�e�B���e�B�N���X�́A�Q�[�����̂��ׂẴl�b�g���[�N���b�Z�[�W�̈�ӂ̎��ʎq�����ׂĒ�`���܂��B
    /// </summary>
    public class NetworkProtocol
    {
        public static short RegisterPlayer = 1000;
        public static short StartGame = 1001;
        public static short EndGame = 1002;
        public static short StartTurn = 1003;
        public static short EndTurn = 1004;
        public static short StopTurn = 1005;

        public static short PlayerState = 1006;
        public static short OpponentState = 1007;

        public static short MoveCard = 1008;
        public static short CardMoved = 1009;

        public static short FightPlayer = 1010;
        public static short FightCreature = 1011;
        public static short PlayerAttacked = 1012;
        public static short CreatureAttacked = 1013;

        public static short ActivateAbility = 1014;

        public static short SendChatTextMessage = 1015;
        public static short BroadcastChatTextMessage = 1016;

        public static short PlayerDrewCards = 1017;
        public static short OpponentDrewCards = 1018;
    }

    // Every network message has a corresponding message class that carries the information needed
    // per message.
    //���ׂẴl�b�g���[�N���b�Z�[�W�ɂ́A���b�Z�[�W���ƂɕK�v�ȏ���ێ�����Ή����郁�b�Z�[�W�N���X������܂��B

    public class RegisterPlayerMessage : MessageBase
    {
        public NetworkInstanceId netId;
        public string name;
        public bool isHuman;
        public int[] deck;
    }

    public class StartGameMessage : MessageBase
    {
        public NetworkInstanceId recipientNetId;
        public int playerIndex;
        public int turnDuration;
        public string[] nicknames;
        public NetPlayerInfo player;
        public NetPlayerInfo opponent;
        public int rngSeed;
    }

    public class StartTurnMessage : MessageBase
    {
        public NetworkInstanceId recipientNetId;
        //isRecipientTheActivePlayer:���b�Z�[�W�̎�M�҂��A�N�e�B�u�ȃv���C���[�ł��邩
        public bool isRecipientTheActivePlayer;
        public int turn;
        public NetPlayerInfo player;
        public NetPlayerInfo opponent;
    }

    public class PlayerGameStateMessage : MessageBase
    {
        public NetPlayerInfo player;
    }

    public class OpponentGameStateMessage : MessageBase
    {
        public NetPlayerInfo[] opponents;
    }
    /// <summary>
    /// �J�[�h�̈ړ��ɕK�v�ȏ��S
    /// </summary>
    public class MoveCardMessage : MessageBase
    {
        public NetworkInstanceId playerNetId;
        public int cardInstanceId;
        public int originZoneId;
        public int destinationZoneId;
        public int[] targetInfo;
    }

    /// <summary>
    /// �J�[�h���ړ������ۃN���C�A���g�E�T�[�o�Ԃő���M����ϐ�
    /// </summary>
    public class CardMovedMessage : MessageBase
    {
        //NetworkInstanceId:�l�b�g���[�N�̂��ׂĂ̎Q���҂ɂ킽���ăl�b�g���[�N�����ꂽ�I�u�W�F�N�g�����ʂ��邽�߂Ɏg�p����܂��B�I�u�W�F�N�g���������ꂽ�Ƃ��ɃT�[�o�[�ɂ���Ď��s���Ɋ��蓖�Ă��܂��B
        public NetworkInstanceId playerNetId;
        //CCGKit�ŗ��p����A�J�[�h�����l�b�g���[�N��ł���肷�邽�߂̒l���������N���X
        public NetCard card;
        public int originZoneId;
        public int destinationZoneId;
        public int[] targetInfo;
    }

    public class SummonCardMessage : MessageBase
    {
        public int cardInstanceId;
    }

    public class PlayedCardMessage : MessageBase
    {
        public NetCard card;
    }

    public class FightPlayerMessage : MessageBase
    {
        public NetworkInstanceId attackingPlayerNetId;
        public int cardInstanceId;
    }

    public class FightCreatureMessage : MessageBase
    {
        public NetworkInstanceId attackingPlayerNetId;
        public int attackingCardInstanceId;
        public int attackedCardInstanceId;
    }

    public class PlayerAttackedMessage : MessageBase
    {
        public NetworkInstanceId attackingPlayerNetId;
        public int attackingCardInstanceId;
    }

    public class CreatureAttackedMessage : MessageBase
    {
        public NetworkInstanceId attackingPlayerNetId;
        public int attackingCardInstanceId;
        public int attackedCardInstanceId;


    }

    public class EndGameMessage : MessageBase
    {
        public NetworkInstanceId winnerPlayerIndex;
    }

    public class EndTurnMessage : MessageBase
    {
        public NetworkInstanceId recipientNetId;
        public bool isRecipientTheActivePlayer;
    }

    public class StopTurnMessage : MessageBase
    {
    }

    public class SendChatTextMessage : MessageBase
    {
        public NetworkInstanceId senderNetId;
        public string text;
    }

    public class BroadcastChatTextMessage : MessageBase
    {
        public NetworkInstanceId senderNetId;
        public string text;
    }

    public class MoveCardsMessage : MessageBase
    {
        public NetworkInstanceId recipientNetId;
        public string originZone;
        public string destinationZone;
        public int numCards;
    }

    public class ActivateAbilityMessage : MessageBase
    {
        public NetworkInstanceId playerNetId;
        public int zoneId;
        public int cardInstanceId;
        public int abilityIndex;
    }
    public class PlayerDrewCardsMessage : MessageBase
    {
        public NetworkInstanceId playerNetId;
        public NetCard[] cards;
    }

    public class OpponentDrewCardsMessage : MessageBase
    {
        public NetworkInstanceId playerNetId;
        public int numCards;
    }
}
