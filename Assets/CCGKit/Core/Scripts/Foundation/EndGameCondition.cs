namespace CCGKit
{
    /// <summary>
    /// The possible results of a game.
    /// �Q�[���̉\�Ȍ��ʁB
    /// </summary>
    public enum EndGameType
    {
        Win,
        Loss
    }

    /// <summary>
    /// This class defines a condition for the game to end.
    /// �����̏I�����������߂�N���X
    /// </summary>
    public class EndGameCondition
    {
        /// <summary>
        /// The end game type.
        /// </summary>
        [EnumField("Type", width = 50)]
        [Order(1)]
        public EndGameType type;

        /// <summary>
        /// Returns a readable string representing this condition.
        /// ���̏�����\���ǂݍ��݉\�ȕ������Ԃ��܂��B
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this condition.</returns>
        public virtual string GetReadableString(GameConfiguration config)
        {
            return "End game condition";
        }

        /// <summary>
        /// Returns a readable string representing the specified condition operator.
        /// �w�肵���������Z�q��\���ǂݍ��݉\�ȕ������Ԃ��܂��B
        /// </summary>
        /// <param name="op">The condition operator.</param>
        /// <returns>A readable string that represents the specified condition operator.</returns>
        public static string GetReadableConditionOperator(ConditionOperator op)
        {
            switch (op)
            {
                case ConditionOperator.LessThan:
                    return "<";

                case ConditionOperator.LessThanOrEqualTo:
                    return "<=";

                case ConditionOperator.EqualTo:
                    return "==";

                case ConditionOperator.GreaterThanOrEqualTo:
                    return ">=";

                default:
                    return ">";
            }
        }

        /// <summary>
        /// Returns true if this condition has been met on the specified player and false otherwise.
        /// ���̏������w�肳�ꂽ�v���[���Ŗ�������Ă���ꍇ��true��Ԃ��A�����łȂ��ꍇ��false��Ԃ��܂��B
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>True if this condition has been met on the specified player; false otherwise.</returns>
        public virtual bool IsTrue(PlayerInfo player)
        {
            return false;
        }
    }
}
