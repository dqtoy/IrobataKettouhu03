namespace CCGKit
{
    /// <summary>
    /// Game action to move cards from one zone to another.
    /// あるゾーンから別のゾーンへカードを移動するゲームアクション。
    /// /// </summary>
    public class MoveCardsAction : GameAction
    {
        /// <summary>
        /// The origin zone of this action.
        /// このアクションの起源ゾーン。
        /// </summary>
        [GameZoneField("Origin")]
        public int originZoneId;

        /// <summary>
        /// The destination zone of this action.
        /// このアクションの宛先ゾーン。
        /// </summary>
        [GameZoneField("Destination")]
        public int destinationZoneId;

        /// <summary>
        /// The number of cards to move.
        /// 移動するカードの数。
        /// </summary>
        [IntField("Num cards")]
        public int numCards;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MoveCardsAction() : base("Move cards")
        {
        }
        //【要確認】ドロー、マリガンで使用
        /// <summary>
        /// Resolves this action.
        /// この操作を解決します。
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The player on which to resolve this action.</param>
        public override void Resolve(GameState state, PlayerInfo player)
        {
            var fromZone = player.zones[originZoneId];
            var toZone = player.zones[destinationZoneId];
            // Do not move more cards than those available in the origin zone.
            // 原点ゾーンで使用可能なカードより多くのカードを移動しないでください。
            if (numCards > fromZone.cards.Count)
            {
                numCards = fromZone.cards.Count;
            }

            // Do not move more card than those allowed in the destination zone.
            // 宛先ゾーンで許可されているカードより多くのカードを移動しないでください。
            if (numCards > toZone.maxCards)
            {
                numCards = toZone.maxCards;
            }

            for (var i = 0; i < numCards; i++)
            {
                toZone.AddCard(fromZone.cards[i]);
            }
            fromZone.RemoveCards(numCards);
        }
    }
}
