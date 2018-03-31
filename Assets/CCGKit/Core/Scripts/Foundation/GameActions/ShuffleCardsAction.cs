namespace CCGKit
{
    /// <summary>
    /// Game action to shuffle the cards in a zone.
    /// �]�[�����̃J�[�h���V���b�t������Q�[���A�N�V�����B
    /// </summary>
    public class ShuffleCardsAction : GameAction
    {
        /// <summary>
        /// The zone of this action.
        /// ���̍s���̑Ώۃ]�[���B
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
        /// �]�[�����̃J�[�h���V���b�t������
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
