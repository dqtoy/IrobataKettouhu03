namespace CCGKit
{
    /// <summary>
    /// A random number.
    /// ����
    /// </summary>
    public class RandomValue : Value
    {
        /// <summary>
        /// The minimum value of this value.
        /// ���̒l�̍ŏ��l�B
        /// </summary>
        [IntField("Min")]
        public int min;

        /// <summary>
        /// The maximum value of this value.
        /// ���̒l�̍ő�l�B
        /// </summary>
        [IntField("Max")]
        public int max;

        /// <summary>
        /// Returns the integer value of this value.
        /// ���̒l�̐����l��Ԃ��܂��B
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
