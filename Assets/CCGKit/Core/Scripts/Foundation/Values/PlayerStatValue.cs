namespace CCGKit
{
    /// <summary>
    /// A value that is equal to the value of a player stat.
    /// player statの値に等しい値。
    /// </summary>
    public class PlayerStatValue : Value
    {
        /// <summary>
        /// The stat of this value.
        /// スタッツの値
        /// </summary>
        [PlayerStatField("Player stat")]
        public int statId;

        /// <summary>
        /// Returns the integer value of this value.
        /// この値の整数値を返す
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The state of the player.</param>
        /// <returns>The integer value of this value.</returns>
        public override int GetValue(GameState state, PlayerInfo player)
        {
            var stat = player.stats[statId];
            return stat.effectiveValue;
        }
    }
}
