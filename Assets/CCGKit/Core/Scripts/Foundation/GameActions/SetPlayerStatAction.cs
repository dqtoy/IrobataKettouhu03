namespace CCGKit
{
    /// <summary>
    /// Game action to set a player stat.
    /// プレーヤーの状態を設定するためのゲームアクション。
    /// </summary>
    public class SetPlayerStatAction : GameAction
    {
        /// <summary>
        /// The stat of this action.
        /// この行動のスタッツ
        /// </summary>
        [PlayerStatField("Stat")]
        public int statId;

        /// <summary>
        /// The value of this action.
        /// アクションの値
        /// </summary>
        [ValueField("Value")]
        public Value value;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SetPlayerStatAction() : base("Set player stat")
        {
        }

        /// <summary>
        /// Resolves this action.
        /// アクションの解決
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The player on which to resolve this action.</param>
        public override void Resolve(GameState state, PlayerInfo player)
        {
            player.stats[statId].baseValue = value.GetValue(state, player);
        }
    }
}
