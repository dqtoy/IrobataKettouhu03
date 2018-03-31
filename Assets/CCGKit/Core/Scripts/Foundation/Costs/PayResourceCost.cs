namespace CCGKit
{
    /// <summary>
    /// A cost that is payed via a player stat.
    /// �v���[���[�̃X�^�b�c������Ďx�������p�B
    /// </summary>
    public class PayResourceCost : Cost
    {
        /// <summary>
        /// The stat of this cost.
        /// ���̃R�X�g�̃X�^�b�c�B
        /// </summary>
        [PlayerStatField("Player stat")]
        public int statId;

        /// <summary>
        /// The value of this cost.
        /// ���̃R�X�g�̃X�^�b�c�B
        /// </summary>
        [IntField("Cost")]
        public int value;

        /// <summary>
        /// Returns a readable string representing this cost.
        /// ���̃R�X�g��\���������Ԃ��܂��B
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
