using System.Collections.Generic;

using UnityEngine.Networking;

namespace CCGKit
{
    /// <summary>
    /// This class stores the current stat of a player in a game.
    /// このクラスは、ゲーム内のプレーヤーの現在の状態を格納します。
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// The unique identifier of this player.
        /// このプレーヤーの一意の識別子。
        /// </summary>
        public int id;

        /// <summary>
        /// The unique connection identifier of this player.
        /// このプレーヤーの一意の接続識別子。
        /// </summary>
        public int connectionId;

        /// <summary>
        /// The unique network instance identifier of this player.
        /// このプレーヤーの一意のネットワークインスタンス識別子。
        /// </summary>
        public NetworkInstanceId netId;

        /// <summary>
        /// The nickname of this player.
        /// プレイヤーのニックネーム
        /// </summary>
        public string nickname;

        /// <summary>
        /// True if this player is currently connected to the server; false otherwise.
        /// このプレーヤーが現在サーバーに接続されている場合はtrue。 そうでなければfalse。
        /// </summary>
        public bool isConnected;

        /// <summary>
        /// True if this player is controlled by a human; false otherwise (AI).
        /// このプレイヤーが人間によって制御されている場合は真です。 そうでなければfalse（AI）。
        /// </summary>
        public bool isHuman;

        /// <summary>
        /// The stats of this player, indexed by id.
        /// idによってインデックスされたこのプレーヤーの統計情報。
        /// </summary>
        public Dictionary<int, Stat> stats = new Dictionary<int, Stat>();

        /// <summary>
        /// The stats of this player, indexed by name.
        /// このプレーヤーの統計情報、名前で索引付けされています。
        /// </summary>
        public Dictionary<string, Stat> namedStats = new Dictionary<string, Stat>();

        /// <summary>
        /// The zones of this player, indexed by id.
        /// idによってインデックスされたこのプレーヤーのゾーン。
        /// </summary>
        public Dictionary<int, RuntimeZone> zones = new Dictionary<int, RuntimeZone>();

        /// <summary>
        /// The zones of this player, indexed by name.
        /// このプレーヤーのゾーンは、名前で索引付けされています。
        /// </summary>
        public Dictionary<string, RuntimeZone> namedZones = new Dictionary<string, RuntimeZone>();

        /// <summary>
        /// The current card instance identifier of this player.
        /// このプレーヤーの現在のカードインスタンス識別子。
        /// </summary>
        public int currentCardInstanceId;

        /// <summary>
        /// The current turn number of this player.
        /// このプレーヤーの現在のターン番号。
        /// </summary>
        public int numTurn;

        /// <summary>
        /// Returns the card with the specified instance identifier in the specified zone.
        /// 指定されたゾーン内の指定されたインスタンス識別子を持つカードを返します。
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
