namespace CCGKit
{
    /// <summary>
    /// Game action to move cards from one zone to another.
    /// ����]�[������ʂ̃]�[���փJ�[�h���ړ�����Q�[���A�N�V�����B
    /// /// </summary>
    public class MoveCardsAction : GameAction
    {
        /// <summary>
        /// The origin zone of this action.
        /// ���̃A�N�V�����̋N���]�[���B
        /// </summary>
        [GameZoneField("Origin")]
        public int originZoneId;

        /// <summary>
        /// The destination zone of this action.
        /// ���̃A�N�V�����̈���]�[���B
        /// </summary>
        [GameZoneField("Destination")]
        public int destinationZoneId;

        /// <summary>
        /// The number of cards to move.
        /// �ړ�����J�[�h�̐��B
        /// </summary>
        [IntField("Num cards")]
        public int numCards;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MoveCardsAction() : base("Move cards")
        {
        }
        //�y�v�m�F�z�h���[�A�}���K���Ŏg�p
        /// <summary>
        /// Resolves this action.
        /// ���̑�����������܂��B
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The player on which to resolve this action.</param>
        public override void Resolve(GameState state, PlayerInfo player)
        {
            var fromZone = player.zones[originZoneId];
            var toZone = player.zones[destinationZoneId];
            // Do not move more cards than those available in the origin zone.
            // ���_�]�[���Ŏg�p�\�ȃJ�[�h��葽���̃J�[�h���ړ����Ȃ��ł��������B
            if (numCards > fromZone.cards.Count)
            {
                numCards = fromZone.cards.Count;
            }

            // Do not move more card than those allowed in the destination zone.
            // ����]�[���ŋ�����Ă���J�[�h��葽���̃J�[�h���ړ����Ȃ��ł��������B
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
