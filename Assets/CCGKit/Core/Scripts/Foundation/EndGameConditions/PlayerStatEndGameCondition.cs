namespace CCGKit
{
    /// <summary>
    /// This type represents end game conditions that result from a change
    /// in the value of a player's attribute.
    /// このタイプは、プレイヤーの属性の値の変化から生じる最終的なゲーム条件を表します。
    /// </summary>
    public class PlayerStatEndGameCondition : EndGameCondition
    {
        /// <summary>
        /// The stat of this condition.
        /// この状態のスタッツ
        /// </summary>
        [PlayerStatField("Stat")]
        [Order(2)]
        public int statId;

        /// <summary>
        /// The operator of this condition.
        /// この状態の演算子
        /// </summary>
        [EnumField("Operator", width = 150)]
        [Order(3)]
        public ConditionOperator op;

        /// <summary>
        /// The value of this condition.
        /// この状態の値
        /// </summary>
        [IntField("Value")]
        [Order(4)]
        public int value;

        /// <summary>
        /// Returns a readable string representing this condition.
        /// この状態を表す読み込み可能な文字列を返します。
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
        /// この状態が指定されたプレーヤで満たされている場合はtrueを返し、そうでない場合はfalseを返します。
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
