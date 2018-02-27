namespace CCGKit
{
    /// <summary>
    /// This class represents a condition for a card to be destroyed.
    /// このクラスは、破壊されるカードの条件を表します。
    /// </summary>
    public class DestroyCardCondition
    {
        /// <summary>
        /// Returns a readable string representing this condition.
        /// この条件を表す読み込み可能な文字列を返します。
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this condition.</returns>
        /// <returns>この条件を表す文字列。</ returns>
        public virtual string GetReadableString(GameConfiguration config)
        {
            return "Destroy card condition";
        }

        /// <summary>
        /// Returns a readable string representing the specified condition operator.
        /// 指定した条件演算子を表す読み込み可能な文字列を返します。
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
