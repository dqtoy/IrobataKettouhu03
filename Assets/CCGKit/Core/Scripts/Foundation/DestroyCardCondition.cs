namespace CCGKit
{
    /// <summary>
    /// This class represents a condition for a card to be destroyed.
    /// ���̃N���X�́A�j�󂳂��J�[�h�̏�����\���܂��B
    /// </summary>
    public class DestroyCardCondition
    {
        /// <summary>
        /// Returns a readable string representing this condition.
        /// ���̏�����\���ǂݍ��݉\�ȕ������Ԃ��܂��B
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this condition.</returns>
        /// <returns>���̏�����\��������B</ returns>
        public virtual string GetReadableString(GameConfiguration config)
        {
            return "Destroy card condition";
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
    }
}
