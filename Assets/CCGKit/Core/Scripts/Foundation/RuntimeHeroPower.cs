using System;
using System.Collections.Generic;

/// <summary>
/// RuntimeCardクラスのパクリ
/// </summary>
namespace CCGKit
{
    /// <summary>
    /// This class represents a runtime instance of a card.
    /// このクラスは、カードのランタイムインスタンス(プログラムの実行に必要な部品の集合)を表します。
    /// </summary>
    public class RuntimeHeroPower
    {

        /// <summary>
        /// The card identifier of this card.
        /// ヒロパの固有ID
        /// </summary>
        public int HeroPowerId;

        /// <summary>
        /// The instance identifier of this card.
        /// このヒロパのインスタンス識別子。
        /// </summary>
        public int instanceId;

        /// <summary>
        /// The stats of this card, indexed by id.
        /// このヒロパのスタッツはidで示されます。
        /// </summary>
        public Dictionary<int, Stat> stats = new Dictionary<int, Stat>();

        /// <summary>
        /// The stats of this card, indexed by name.
        /// このヒロパのスタッツは名前で示されます。
        /// </summary>
        public Dictionary<string, Stat> namedStats = new Dictionary<string, Stat>();


        /// <summary>
        /// The player that owns this card.
        /// このヒロパを所有しているプレイヤー。
        /// </summary>
        public PlayerInfo ownerPlayer;

        /// <summary>
        /// The type of this card.
        /// ヒロパタイプ
        /// </summary>
        public CardType HeroPowerType
        {
            get
            {
                var gameConfig = GameManager.Instance.config;
                var libraryHeroPower = gameConfig.GetHeroPower(HeroPowerId);
                return gameConfig.cardTypes.Find(x => x.id == libraryHeroPower.HeroPowerTypeId);
            }
        }

    }
}
