namespace CCGKit
{
    /// <summary>
    /// A random number.
    /// 乱数
    /// </summary>
    public class RandomValue : Value
    {
        /// <summary>
        /// The minimum value of this value.
        /// この値の最小値。
        /// </summary>
        [IntField("Min")]
        public int min;

        /// <summary>
        /// The maximum value of this value.
        /// この値の最大値。
        /// </summary>
        [IntField("Max")]
        public int max;

        /// <summary>
        /// Returns the integer value of this value.
        /// この値の整数値を返します。
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The state of the player.</param>
        /// <returns>The integer value of this value.</returns>
        public override int GetValue(GameState state, PlayerInfo player)
        {
            return state.effectSolver.GetRandomNumber(min, max + 1);
        }
    }
}
