namespace CCGKit
{
    /// <summary>
    /// Game action to set a player stat.
    /// �v���[���[�̏�Ԃ�ݒ肷�邽�߂̃Q�[���A�N�V�����B
    /// </summary>
    public class SetPlayerStatAction : GameAction
    {
        /// <summary>
        /// The stat of this action.
        /// ���̍s���̃X�^�b�c
        /// </summary>
        [PlayerStatField("Stat")]
        public int statId;

        /// <summary>
        /// The value of this action.
        /// �A�N�V�����̒l
        /// </summary>
        [ValueField("Value")]
        public Value value;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SetPlayerStatAction() : base("Set player stat")
        {
        }

        /// <summary>
        /// Resolves this action.
        /// �A�N�V�����̉���
        /// </summary>
        /// <param name="state">The state of the game.</param>
        /// <param name="player">The player on which to resolve this action.</param>
        public override void Resolve(GameState state, PlayerInfo player)
        {
            player.stats[statId].baseValue = value.GetValue(state, player);
        }
    }
}
