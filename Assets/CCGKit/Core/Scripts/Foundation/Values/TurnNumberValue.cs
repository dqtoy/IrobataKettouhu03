namespace CCGKit
{
    /// <summary>
    /// A value that is equal to the turn number.
    /// ターン番号と等しい値。
    /// </summary>
    public class TurnNumberValue : Value
    {
        /// <summary>
        /// Returns the integer value of this value.
        /// この値の整数値を返します。
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The state of the player.</param>
        /// <returns>The integer value of this value.</returns>
        public override int GetValue(GameState state, PlayerInfo player)
        {
            return player.numTurn;
        }
    }
}
