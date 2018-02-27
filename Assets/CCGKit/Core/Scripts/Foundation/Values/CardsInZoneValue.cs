namespace CCGKit
{
    /// <summary>
    /// A value that is equal to the number of cards in a zone.
    /// ゾーン内のカードの数に等しい値。
    /// </summary>
    public class CardsInZoneValue : Value
    {
        /// <summary>
        /// The zone of this value.
        /// この値のゾーン。
        /// </summary>
        [GameZoneField("Zone")]
        public int zoneId;

        /// <summary>
        /// Returns the integer value of this value.
        /// この値の整数値を返します。
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The state of the player.</param>
        /// <returns>The integer value of this value.</returns>
        public override int GetValue(GameState state, PlayerInfo player)
        {
            var zone = player.zones[zoneId];
            return zone.cards.Count;
        }
    }
}
