namespace CCGKit
{
    /// <summary>
    /// The base trigger class.
    /// 基本トリガークラス。
    /// </summary>
    public abstract class Trigger
    {
    }

    /// <summary>
    /// The base trigger class for triggers related to player stat changes.
    /// playerのスタッツ変更に関連するトリガーの基本トリガークラスです。
    /// </summary>
    public abstract class OnPlayerStatChangedTrigger : Trigger
    {
        /// <summary>
        /// The stat of this trigger.
        /// トリガーの固有ID
        /// </summary>
        [PlayerStatField("Stat")]
        public int statId;

        /// <summary>
        /// Returns true if this trigger is true and false otherwise.
        /// このトリガーがtrueの場合はtrueを返し、そうでない場合はfalseを返します。
        /// </summary>
        /// <param name="stat">The stat.</param>
        /// <param name="newValue">The new value of the stat.</param>
        /// <param name="oldValue">The old value of the stat.</param>
        /// <returns>True if this trigger is true; false otherwise.</returns>
        public abstract bool IsTrue(Stat stat, int newValue, int oldValue);
    }

    /// <summary>
    /// The base trigger class for triggers related to card stat changes.
    /// カードのスタッツ情報の変更に関連するトリガーの基本トリガークラス。
    /// </summary>
    public abstract class OnCardStatChangedTrigger : Trigger
    {
        /// <summary>
        /// The card type of this trigger.
        /// このトリガーのカードタイプ
        /// </summary>
        [CardTypeField("Card type")]
        public int cardTypeId;

        /// <summary>
        /// The stat of this trigger.
        /// このトリガーの固有ID
        /// </summary>
        [CardStatField("Stat")]
        public int statId;

        /// <summary>
        /// Returns true if this trigger is true and false otherwise.
        /// このトリガーがtrueの場合はtrueを返し、そうでない場合はfalseを返します。
        /// </summary>
        /// <param name="stat">The stat.</param>
        /// <param name="newValue">The new value of the stat.</param>
        /// <param name="oldValue">The old value of the stat.</param>
        /// <returns>True if this trigger is true; false otherwise.</returns>
        public abstract bool IsTrue(Stat stat, int newValue, int oldValue);
    }

    /// <summary>
    /// The base trigger class for triggers related to card movements.
    /// カードの動きに関連するトリガのベーストリガクラス。
    /// </summary>
    public abstract class OnCardMovedTrigger : Trigger
    {
        /// <summary>
        /// Returns true if this trigger is true and false otherwise.
        /// </summary>
        /// <param name="stat">The stat.</param>
        /// <param name="newValue">The new value of the stat.</param>
        /// <param name="oldValue">The old value of the stat.</param>
        /// <returns>True if this trigger is true; false otherwise.</returns>
        public abstract bool IsTrue(GameState state, string zone);
    }
}
