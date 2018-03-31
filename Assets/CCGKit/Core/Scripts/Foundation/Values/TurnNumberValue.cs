namespace CCGKit
{
    /// <summary>
    /// A value that is equal to the turn number.
    /// �^�[���ԍ��Ɠ������l�B
    /// </summary>
    public class TurnNumberValue : Value
    {
        /// <summary>
        /// Returns the integer value of this value.
        /// ���̒l�̐����l��Ԃ��܂��B
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
