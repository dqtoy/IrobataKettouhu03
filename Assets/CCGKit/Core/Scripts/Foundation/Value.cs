namespace CCGKit
{
    /// <summary>
    /// The base class for values.
    /// �l�̊�{�N���X�B
    /// </summary>
    public abstract class Value
    {
        /// <summary>
        /// Returns the integer value of this value.
        /// ���̒l�̐����l��Ԃ��܂��B
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The state of the player.</param>
        /// <returns></returns>
        public abstract int GetValue(GameState state, PlayerInfo player);
    }
}
