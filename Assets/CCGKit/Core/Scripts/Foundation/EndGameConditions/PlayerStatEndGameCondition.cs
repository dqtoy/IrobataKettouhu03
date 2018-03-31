namespace CCGKit
{
    /// <summary>
    /// This type represents end game conditions that result from a change
    /// in the value of a player's attribute.
    /// ���̃^�C�v�́A�v���C���[�̑����̒l�̕ω����琶����ŏI�I�ȃQ�[��������\���܂��B
    /// </summary>
    public class PlayerStatEndGameCondition : EndGameCondition
    {
        /// <summary>
        /// The stat of this condition.
        /// ���̏�Ԃ̃X�^�b�c
        /// </summary>
        [PlayerStatField("Stat")]
        [Order(2)]
        public int statId;

        /// <summary>
        /// The operator of this condition.
        /// ���̏�Ԃ̉��Z�q
        /// </summary>
        [EnumField("Operator", width = 150)]
        [Order(3)]
        public ConditionOperator op;

        /// <summary>
        /// The value of this condition.
        /// ���̏�Ԃ̒l
        /// </summary>
        [IntField("Value")]
        [Order(4)]
        public int value;

        /// <summary>
        /// Returns a readable string representing this condition.
        /// ���̏�Ԃ�\���ǂݍ��݉\�ȕ������Ԃ��܂��B
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this condition.</returns>
        public override string GetReadableString(GameConfiguration config)
        {
            var stat = config.playerStats.Find(x => x.id == statId);
            if (stat != null)
            {
                return stat.name + " " + GetReadableConditionOperator(op) + " " + value;
            }
            return "";
        }

        /// <summary>
        /// Returns true if this condition has been met on the specified player and false otherwise.
        /// ���̏�Ԃ��w�肳�ꂽ�v���[���Ŗ�������Ă���ꍇ��true��Ԃ��A�����łȂ��ꍇ��false��Ԃ��܂��B
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>True if this condition has been met on the specified player; false otherwise.</returns>
        public override bool IsTrue(PlayerInfo player)
        {
            var stat = player.stats[statId];
            switch (op)
            {
                case ConditionOperator.LessThan:
                    return stat.effectiveValue < value;

                case ConditionOperator.LessThanOrEqualTo:
                    return stat.effectiveValue <= value;

                case ConditionOperator.EqualTo:
                    return stat.effectiveValue == value;

                case ConditionOperator.GreaterThanOrEqualTo:
                    return stat.effectiveValue >= value;

                case ConditionOperator.GreaterThan:
                    return stat.effectiveValue > value;
            }
            return false;
        }
    }
}
