// Copyright (C) 2016-2017 David Pol. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// This class represents a condition in which a card stat is checked against a certain value.
    /// このクラスは、カードのstatが特定の値と照合される条件を表します。
    /// </summary>
    public class CardStatCondition : CardCondition
    {
        /// <summary>
        /// The type of the card.
        /// カードのタイプ
        /// </summary>
        [CardTypeField("Card type")]
        [Order(2)]
        public int cardTypeId;

        /// <summary>
        /// The stat of the card.
        /// カードのスタッツ
        /// </summary>
        [CardStatField("Stat")]
        [Order(3)]
        public int statId;

        /// <summary>
        /// The operator of this condition.
        /// この条件の演算子。
        /// </summary>
        [EnumField("Operator", width = 150)]
        [Order(4)]
        public ConditionOperator op;

        /// <summary>
        /// The value of this condition.
        /// この条件の値
        /// </summary>
        [IntField("Value")]
        [Order(5)]
        public int value;

        /// <summary>
        /// Returns a readable string representing this condition.
        /// この条件を表す読み込み可能な文字列を返します。
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this condition.</returns>
        public override string GetReadableString(GameConfiguration config)
        {
            var cardType = config.cardTypes.Find(x => x.id == cardTypeId);
            if (statId < cardType.stats.Count)
            {
                var stat = cardType.stats[statId];
                return stat.name + " " + GetReadableConditionOperator(op) + " " + value;
            }
            return null;
        }

        /// <summary>
        /// Returns true if this condition has been met on the specified card and false otherwise.
        /// 指定されたカードでこの条件が満たされている場合はtrueを返し、そうでない場合はfalseを返します。
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if this condition has been met on the specified card; false otherwise.</returns>
        public override bool IsTrue(RuntimeCard card)
        {
            var stat = card.stats[statId];
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
