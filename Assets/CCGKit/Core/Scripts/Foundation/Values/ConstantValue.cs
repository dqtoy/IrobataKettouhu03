namespace CCGKit
{
    /// <summary>
    /// A connstant number.
    /// 定数
    /// </summary>
    public class ConstantValue : Value
    {
        /// <summary>
        /// The constant value of this value.
        /// </summary>
        [IntField("Constant")]
        public int constant;

        /// <summary>
        /// Returns the integer value of this value.
        /// この値の整数値を返します。
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The state of the player.</param>
        /// <returns>The integer value of this value.</returns>
        public override int GetValue(GameState state, PlayerInfo player)
        {
            return constant;
        }
    }
}
