namespace CCGKit
{
    /// <summary>
    /// This type represents end game conditions that result from a change
    /// in the number of cards in a game zone.
    /// このタイプは、ゲームゾーン内のカードの数の変化から生じる最終的なゲーム条件を表します。
    /// </summary>
    public class CardsInZoneEndGameCondition : EndGameCondition
    {
        /// <summary>
        /// The zone of this condition.
        /// この状態のゾーン
        /// </summary>
        [GameZoneField("Zone")]
        [Order(2)]
        public int zoneId;

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
        /// 状態を表す読み込み可能な文字列を返します。
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this condition.</returns>
        public override string GetReadableString(GameConfiguration config)
        {
            var zone = config.gameZones.Find(x => x.id == zoneId);
            if (zone != null)
            {
                return zone.name + " " + GetReadableConditionOperator(op) + " " + value;
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
            var numCards = player.zones[zoneId].cards.Count;
            switch (op)
            {
                case ConditionOperator.LessThan:
                    return numCards < value;

                case ConditionOperator.LessThanOrEqualTo:
                    return numCards <= value;

                case ConditionOperator.EqualTo:
                    return numCards == value;

                case ConditionOperator.GreaterThanOrEqualTo:
                    return numCards >= value;

                case ConditionOperator.GreaterThan:
                    return numCards > value;
            }
            return false;
        }
    }
}
