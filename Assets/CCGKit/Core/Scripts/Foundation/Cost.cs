namespace CCGKit
{
    /// <summary>
    /// This card represents a cost in the game. Costs are used when playing cards and when
    /// activating activated abilities.
    /// ���̃J�[�h�́A�Q�[���̃R�X�g��\���܂��B �J�[�h���v���C������A�N�������\�͂��A�N�e�B�u�ɂ���Ƃ��ɃR�X�g���g�p����܂��B
    /// </summary>
    public class Cost
    {
        /// <summary>
        /// Returns a readable string representing this cost.
        /// ���̃R�X�g��\���������Ԃ��܂��B
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this cost.</returns>
        public virtual string GetReadableString(GameConfiguration config)
        {
            return "Card cost";
        }
    }
}
