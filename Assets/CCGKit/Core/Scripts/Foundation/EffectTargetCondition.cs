namespace CCGKit
{
    /// <summary>
    /// The base class for conditions.
    /// �����̊�{�N���X�B
    /// </summary>
    public abstract class Condition
    {
        /// <summary>
        /// Returns a readable string representing this condition.
        /// ���̏�����\���ǂݍ��݉\�ȕ������Ԃ��܂��B
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this condition.</returns>
        public abstract string GetReadableString(GameConfiguration config);

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

    /// <summary>
    /// The base class for player conditions.
    /// �v���[�������̊�{�N���X�B
    /// </summary>
    public abstract class PlayerCondition : Condition
    {
        /// <summary>
        /// Returns true if this condition has been met on the specified player and false otherwise.
        /// ���̏������w�肳�ꂽ�v���[���Ŗ�������Ă���ꍇ��true��Ԃ��A�����łȂ��ꍇ��false��Ԃ��܂��B
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>True if this condition has been met on the specified player; false otherwise.</returns>
        public abstract bool IsTrue(PlayerInfo player);
    }

    /// <summary>
    /// The base class for card conditions.
    /// �J�[�h�����̊�{�N���X�B
    /// </summary>
    public abstract class CardCondition : Condition
    {
        /// <summary>
        /// Returns true if this condition has been met on the specified card and false otherwise.
        /// �w�肳�ꂽ�J�[�h�ł��̏�������������Ă���ꍇ��true��Ԃ��A�����łȂ��ꍇ��false��Ԃ��܂��B
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if this condition has been met on the specified card; false otherwise.</returns>
        /// <returns>���̏������w�肳�ꂽ�J�[�h�Ŗ�������Ă���ΐ^�B �����łȂ��ꍇ��false�ł��B</ returns>
        public abstract bool IsTrue(RuntimeCard card);
    }
    /// <summary>
    /// The base class for card conditions.
    /// �q���p�̃R���f�B�V�����̊�{�N���X�BCardCondition�N���X�̃p�N��
    /// </summary>
    public abstract class HeroPowerCondition : Condition
    {
        /// <summary>
        /// Returns true if this condition has been met on the specified card and false otherwise.
        /// �w�肳�ꂽ�J�[�h�ł��̏�������������Ă���ꍇ��true��Ԃ��A�����łȂ��ꍇ��false��Ԃ��܂��B
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if this condition has been met on the specified card; false otherwise.</returns>
        /// <returns>���̏������w�肳�ꂽ�J�[�h�Ŗ�������Ă���ΐ^�B �����łȂ��ꍇ��false�ł��B</ returns>
        public abstract bool IsTrue(RuntimeHeroPower HeroPower);
    }
    /// <summary>
    /// The base class for card conditions.
    /// �J�[�h�����̊�{�N���X�B
    /// </summary>
    public abstract class TokenCondition : Condition
    {
        /// <summary>
        /// Returns true if this condition has been met on the specified card and false otherwise.
        /// �w�肳�ꂽ�J�[�h�ł��̏�������������Ă���ꍇ��true��Ԃ��A�����łȂ��ꍇ��false��Ԃ��܂��B
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if this condition has been met on the specified card; false otherwise.</returns>
        /// <returns>���̏������w�肳�ꂽ�J�[�h�Ŗ�������Ă���ΐ^�B �����łȂ��ꍇ��false�ł��B</ returns>
        public abstract bool IsTrue(RuntimeToken token);
    }

}
