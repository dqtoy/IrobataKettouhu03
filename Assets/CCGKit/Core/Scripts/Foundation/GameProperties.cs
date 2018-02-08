using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// The general properties of a game.
    /// ゲームの一般的なプロパティ
    /// </summary>
    public class GameProperties
    {
        /// <summary>
        /// Duration of a game turn (in seconds).
        /// 1ターンの時間(秒)
        /// </summary>
        public int turnDuration;

        /// <summary>
        /// Minimum number of cards that need to be in a deck.
        /// デッキに入れなければならないカードの最小枚数。
        /// </summary>
        public int minDeckSize;

        /// <summary>
        /// Maximum number of cards that can be in a deck.
        /// デッキに入れなければならないカードの最大枚数。
        /// </summary>
        public int maxDeckSize;

        /// <summary>
        /// List of actions to perform when a game starts.
        /// ゲーム開始時に実行するアクションのリスト。
        /// </summary>
        public List<GameAction> gameStartActions = new List<GameAction>();

        /// <summary>
        /// List of actions to perform when a turn starts.
        /// ターン開始時に実行するアクションのリスト。
        /// </summary>
        public List<GameAction> turnStartActions = new List<GameAction>();

        /// <summary>
        /// List of actions to perform when a turn ends.
        /// ターン終了時に実行するアクションのリスト
        /// </summary>
        public List<GameAction> turnEndActions = new List<GameAction>();

        /// <summary>
        /// List of end game conditions of this game.
        /// ゲーム終了時の状態リスト
        /// </summary>
        public List<EndGameCondition> endGameConditions = new List<EndGameCondition>();
    }
}