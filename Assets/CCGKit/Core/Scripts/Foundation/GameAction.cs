namespace CCGKit
{
    /// <summary>
    /// The available game action targets.
    /// 利用可能なゲームアクションターゲット。
    /// </summary>
    public enum GameActionTarget
    {
        CurrentPlayer,
        CurrentOpponent,
        AllPlayers
    }

    /// <summary>
    /// The base class for game actions.
    /// ゲームアクションの基本クラス。
    /// 抽象クラスなので外部からインスタンスの生成は不可
    /// </summary>
    public abstract class GameAction
    {
        /// <summary>
        /// The name of this action.
        /// このアクションの名前
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// The target of this action.
        /// このアクションのターゲット
        /// </summary>
        public GameActionTarget target;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        public GameAction(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Resolves this action.
        /// 操作を解決する
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The player on which to resolve this action.</param>
        public abstract void Resolve(GameState state, PlayerInfo player);
    }
}
