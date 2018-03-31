namespace CCGKit
{
    /// <summary>
    /// The base trigger class.
    /// ��{�g���K�[�N���X�B
    /// </summary>
    public abstract class Trigger
    {
    }

    /// <summary>
    /// The base trigger class for triggers related to player stat changes.
    /// player�̃X�^�b�c�ύX�Ɋ֘A����g���K�[�̊�{�g���K�[�N���X�ł��B
    /// </summary>
    public abstract class OnPlayerStatChangedTrigger : Trigger
    {
        /// <summary>
        /// The stat of this trigger.
        /// �g���K�[�̌ŗLID
        /// </summary>
        [PlayerStatField("Stat")]
        public int statId;

        /// <summary>
        /// Returns true if this trigger is true and false otherwise.
        /// ���̃g���K�[��true�̏ꍇ��true��Ԃ��A�����łȂ��ꍇ��false��Ԃ��܂��B
        /// </summary>
        /// <param name="stat">The stat.</param>
        /// <param name="newValue">The new value of the stat.</param>
        /// <param name="oldValue">The old value of the stat.</param>
        /// <returns>True if this trigger is true; false otherwise.</returns>
        public abstract bool IsTrue(Stat stat, int newValue, int oldValue);
    }

    /// <summary>
    /// The base trigger class for triggers related to card stat changes.
    /// �J�[�h�̃X�^�b�c���̕ύX�Ɋ֘A����g���K�[�̊�{�g���K�[�N���X�B
    /// </summary>
    public abstract class OnCardStatChangedTrigger : Trigger
    {
        /// <summary>
        /// The card type of this trigger.
        /// ���̃g���K�[�̃J�[�h�^�C�v
        /// </summary>
        [CardTypeField("Card type")]
        public int cardTypeId;

        /// <summary>
        /// The stat of this trigger.
        /// ���̃g���K�[�̌ŗLID
        /// </summary>
        [CardStatField("Stat")]
        public int statId;

        /// <summary>
        /// Returns true if this trigger is true and false otherwise.
        /// ���̃g���K�[��true�̏ꍇ��true��Ԃ��A�����łȂ��ꍇ��false��Ԃ��܂��B
        /// </summary>
        /// <param name="stat">The stat.</param>
        /// <param name="newValue">The new value of the stat.</param>
        /// <param name="oldValue">The old value of the stat.</param>
        /// <returns>True if this trigger is true; false otherwise.</returns>
        public abstract bool IsTrue(Stat stat, int newValue, int oldValue);
    }

    /// <summary>
    /// The base trigger class for triggers related to card movements.
    /// �J�[�h�̓����Ɋ֘A����g���K�̃x�[�X�g���K�N���X�B
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
