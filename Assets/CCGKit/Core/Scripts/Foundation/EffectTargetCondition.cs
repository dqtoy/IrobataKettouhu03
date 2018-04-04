namespace CCGKit
{
    /// <summary>
    /// The base class for conditions.
    /// 条件の基本クラス。
    /// </summary>
    public abstract class Condition
    {
        /// <summary>
        /// Returns a readable string representing this condition.
        /// この条件を表す読み込み可能な文字列を返します。
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this condition.</returns>
        public abstract string GetReadableString(GameConfiguration config);

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

    /// <summary>
    /// The base class for player conditions.
    /// プレーヤ条件の基本クラス。
    /// </summary>
    public abstract class PlayerCondition : Condition
    {
        /// <summary>
        /// Returns true if this condition has been met on the specified player and false otherwise.
        /// この条件が指定されたプレーヤで満たされている場合はtrueを返し、そうでない場合はfalseを返します。
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>True if this condition has been met on the specified player; false otherwise.</returns>
        public abstract bool IsTrue(PlayerInfo player);
    }

    /// <summary>
    /// The base class for card conditions.
    /// カード条件の基本クラス。
    /// </summary>
    public abstract class CardCondition : Condition
    {
        /// <summary>
        /// Returns true if this condition has been met on the specified card and false otherwise.
        /// 指定されたカードでこの条件が満たされている場合はtrueを返し、そうでない場合はfalseを返します。
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if this condition has been met on the specified card; false otherwise.</returns>
        /// <returns>この条件が指定されたカードで満たされていれば真。 そうでない場合はfalseです。</ returns>
        public abstract bool IsTrue(RuntimeCard card);
    }
    /// <summary>
    /// The base class for card conditions.
    /// ヒロパのコンディションの基本クラス。CardConditionクラスのパクリ
    /// </summary>
    public abstract class HeroPowerCondition : Condition
    {
        /// <summary>
        /// Returns true if this condition has been met on the specified card and false otherwise.
        /// 指定されたカードでこの条件が満たされている場合はtrueを返し、そうでない場合はfalseを返します。
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if this condition has been met on the specified card; false otherwise.</returns>
        /// <returns>この条件が指定されたカードで満たされていれば真。 そうでない場合はfalseです。</ returns>
        public abstract bool IsTrue(RuntimeHeroPower HeroPower);
    }
    /// <summary>
    /// The base class for card conditions.
    /// カード条件の基本クラス。
    /// </summary>
    public abstract class TokenCondition : Condition
    {
        /// <summary>
        /// Returns true if this condition has been met on the specified card and false otherwise.
        /// 指定されたカードでこの条件が満たされている場合はtrueを返し、そうでない場合はfalseを返します。
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if this condition has been met on the specified card; false otherwise.</returns>
        /// <returns>この条件が指定されたカードで満たされていれば真。 そうでない場合はfalseです。</ returns>
        public abstract bool IsTrue(RuntimeToken token);
    }

}
