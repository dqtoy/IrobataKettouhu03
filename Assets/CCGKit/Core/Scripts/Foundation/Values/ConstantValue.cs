namespace CCGKit
{
    /// <summary>
    /// A connstant number.
    /// �萔
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
        /// ���̒l�̐����l��Ԃ��܂��B
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
