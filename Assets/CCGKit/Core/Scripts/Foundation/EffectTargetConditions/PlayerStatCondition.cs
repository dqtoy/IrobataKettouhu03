// Copyright (C) 2016-2017 David Pol. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// This class represents a condition in which a player stat is checked against a certain value.
    /// ���̃N���X�́A�v���[���[stat������̒l�ɑ΂��ă`�F�b�N����Ă����Ԃ�\���܂��B
    /// </summary>
    public class PlayerStatCondition : PlayerCondition
    {
        /// <summary>
        /// The stat of the player.
        /// �v���C���[�̃X�^�b�c
        /// </summary>
        [PlayerStatField("Stat")]
        [Order(2)]
        public int statId;

        /// <summary>
        /// The operator of this condition.
        /// ���̏����̉��Z�q
        /// </summary>
        [EnumField("Operator", width = 150)]
        [Order(3)]
        public ConditionOperator op;

        /// <summary>
        /// The value of this condition.
        /// ���̏����̒l
        /// </summary>
        [IntField("Value")]
        [Order(4)]
        public int value;

        /// <summary>
        /// Returns a readable string representing this condition.
        /// ���̏�����\���ǂݍ��݉\�ȕ������Ԃ��܂��B
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this condition.</returns>
        /// ���̏�Ԃ�\���ǂȕ�����B
        public override string GetReadableString(GameConfiguration config)
        {
            var stat = config.playerStats.Find(x => x.id == statId);
            return stat.name + " " + GetReadableConditionOperator(op) + " " + value;
        }

        /// <summary>
        /// Returns true if this condition has been met on the specified player and false otherwise.
        /// ���̏������w�肳�ꂽ�v���[���Ŗ�������Ă���ꍇ��true��Ԃ��A�����łȂ��ꍇ��false��Ԃ��܂��B
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>True if this condition has been met on the specified player; false otherwise.</returns>
        /// <returns>���̏������w�肳�ꂽ�v���[���Ŗ�������Ă���ꍇ��True��Ԃ��܂��B �����łȂ��ꍇ��false�ł��B</ returns>
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
