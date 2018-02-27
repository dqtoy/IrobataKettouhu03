namespace CCGKit
{
    /// <summary>
    /// The base class for values.
    /// 値の基本クラス。
    /// </summary>
    public abstract class Value
    {
        /// <summary>
        /// Returns the integer value of this value.
        /// この値の整数値を返します。
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The state of the player.</param>
        /// <returns></returns>
        public abstract int GetValue(GameState state, PlayerInfo player);
    }
}
