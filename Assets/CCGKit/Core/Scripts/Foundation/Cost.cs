namespace CCGKit
{
    /// <summary>
    /// This card represents a cost in the game. Costs are used when playing cards and when
    /// activating activated abilities.
    /// このカードは、ゲームのコストを表します。 カードをプレイしたり、起動した能力をアクティブにするときにコストが使用されます。
    /// </summary>
    public class Cost
    {
        /// <summary>
        /// Returns a readable string representing this cost.
        /// このコストを表す文字列を返します。
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this cost.</returns>
        public virtual string GetReadableString(GameConfiguration config)
        {
            return "Card cost";
        }
    }
}
