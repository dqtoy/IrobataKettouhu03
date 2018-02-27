namespace CCGKit
{
    /// <summary>
    /// Game action to shuffle the cards in a zone.
    /// ゾーン内のカードをシャッフルするゲームアクション。
    /// </summary>
    public class ShuffleCardsAction : GameAction
    {
        /// <summary>
        /// The zone of this action.
        /// この行動の対象ゾーン。
        /// </summary>
        [GameZoneField("Zone")]
        public int zoneId;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ShuffleCardsAction() : base("Shuffle cards")
        {
        }

        /// <summary>
        /// Resolves this action.
        /// ゾーン内のカードをシャッフルする
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The player on which to resolve this action.</param>
        public override void Resolve(GameState state, PlayerInfo player)
        {
            var zone = player.zones[zoneId];
            zone.cards.Shuffle();
        }
    }
}
