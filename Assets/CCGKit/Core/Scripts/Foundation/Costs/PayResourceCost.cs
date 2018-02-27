namespace CCGKit
{
    /// <summary>
    /// A cost that is payed via a player stat.
    /// プレーヤーのスタッツ情報を介して支払われる費用。
    /// </summary>
    public class PayResourceCost : Cost
    {
        /// <summary>
        /// The stat of this cost.
        /// このコストのスタッツ。
        /// </summary>
        [PlayerStatField("Player stat")]
        public int statId;

        /// <summary>
        /// The value of this cost.
        /// このコストのスタッツ。
        /// </summary>
        [IntField("Cost")]
        public int value;

        /// <summary>
        /// Returns a readable string representing this cost.
        /// このコストを表す文字列を返します。
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this cost.</returns>
        public override string GetReadableString(GameConfiguration config)
        {
            var stat = config.playerStats.Find(x => x.id == statId);
            if (stat != null)
            {
                return "Pay " + value + " " + stat.name.ToLower();
            }
            return null;
        }
    }
}
