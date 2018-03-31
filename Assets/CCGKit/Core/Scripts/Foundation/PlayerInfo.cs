using System.Collections.Generic;

using UnityEngine.Networking;

namespace CCGKit
{
    /// <summary>
    /// This class stores the current stat of a player in a game.
    /// ���̃N���X�́A�Q�[�����̃v���[���[�̌��݂̏�Ԃ��i�[���܂��B
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// The unique identifier of this player.
        /// ���̃v���[���[�̈�ӂ̎��ʎq�B
        /// </summary>
        public int id;

        /// <summary>
        /// The unique connection identifier of this player.
        /// ���̃v���[���[�̈�ӂ̐ڑ����ʎq�B
        /// </summary>
        public int connectionId;

        /// <summary>
        /// The unique network instance identifier of this player.
        /// ���̃v���[���[�̈�ӂ̃l�b�g���[�N�C���X�^���X���ʎq�B
        /// </summary>
        public NetworkInstanceId netId;

        /// <summary>
        /// The nickname of this player.
        /// �v���C���[�̃j�b�N�l�[��
        /// </summary>
        public string nickname;

        /// <summary>
        /// True if this player is currently connected to the server; false otherwise.
        /// ���̃v���[���[�����݃T�[�o�[�ɐڑ�����Ă���ꍇ��true�B �����łȂ����false�B
        /// </summary>
        public bool isConnected;

        /// <summary>
        /// True if this player is controlled by a human; false otherwise (AI).
        /// ���̃v���C���[���l�Ԃɂ���Đ��䂳��Ă���ꍇ�͐^�ł��B �����łȂ����false�iAI�j�B
        /// </summary>
        public bool isHuman;

        /// <summary>
        /// The stats of this player, indexed by id.
        /// id�ɂ���ăC���f�b�N�X���ꂽ���̃v���[���[�̓��v���B
        /// </summary>
        public Dictionary<int, Stat> stats = new Dictionary<int, Stat>();

        /// <summary>
        /// The stats of this player, indexed by name.
        /// ���̃v���[���[�̓��v���A���O�ō����t������Ă��܂��B
        /// </summary>
        public Dictionary<string, Stat> namedStats = new Dictionary<string, Stat>();

        /// <summary>
        /// The zones of this player, indexed by id.
        /// id�ɂ���ăC���f�b�N�X���ꂽ���̃v���[���[�̃]�[���B
        /// </summary>
        public Dictionary<int, RuntimeZone> zones = new Dictionary<int, RuntimeZone>();

        /// <summary>
        /// The zones of this player, indexed by name.
        /// ���̃v���[���[�̃]�[���́A���O�ō����t������Ă��܂��B
        /// </summary>
        public Dictionary<string, RuntimeZone> namedZones = new Dictionary<string, RuntimeZone>();

        /// <summary>
        /// The current card instance identifier of this player.
        /// ���̃v���[���[�̌��݂̃J�[�h�C���X�^���X���ʎq�B
        /// </summary>
        public int currentCardInstanceId;

        /// <summary>
        /// The current turn number of this player.
        /// ���̃v���[���[�̌��݂̃^�[���ԍ��B
        /// </summary>
        public int numTurn;

        /// <summary>
        /// Returns the card with the specified instance identifier in the specified zone.
        /// �w�肳�ꂽ�]�[�����̎w�肳�ꂽ�C���X�^���X���ʎq�����J�[�h��Ԃ��܂��B
        /// </summary>
        /// <param name="cardInstanceId">The instance identifier of this card.</param>
        /// <param name="zoneId">The zone identifier of this card.</param>
        /// <returns>The card with the specified instance identifier in the specified zone.</returns>
        public RuntimeCard GetCard(int cardInstanceId, int zoneId)
        {
            var card = zones[zoneId].cards.Find(x => x.instanceId == cardInstanceId);
            return card;
        }
    }
}
