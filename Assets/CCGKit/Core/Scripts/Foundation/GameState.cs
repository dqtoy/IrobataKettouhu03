using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class stores the current, complete state of a game.
    /// このクラスは、ゲームの現在の完全な状態を格納します。
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// The configuration of this game.
        /// ゲームの構成
        /// </summary>
        public GameConfiguration config;

        /// <summary>
        /// The players of this game.
        /// ゲームのプレイヤー情報
        /// </summary>
        public List<PlayerInfo> players = new List<PlayerInfo>();

        /// <summary>
        /// The current player of this game.
        /// 自プレイヤー情報
        /// </summary>
        public PlayerInfo currentPlayer;

        /// <summary>
        /// The current opponent of this game.
        /// 対戦相手情報
        /// </summary>
        public PlayerInfo currentOpponent;

        /// <summary>
        /// The effect solver to use in this game.
        /// このゲームで使用するエフェクトの解決者
        /// </summary>
        public EffectSolver effectSolver;
    }
}
