namespace CCGKit
{
    /// <summary>
    /// The available zone owners.
    /// �g�p�\�ȃ]�[���̏��L�ҁB
    /// </summary>
    public enum ZoneOwner
    {
        Player,
        Shared
    }

    /// <summary>
    /// The available zone types.
    /// ���p�\�ȃ]�[���̎�ށB
    /// </summary>
    public enum ZoneType
    {
        Static,
        Dynamic
    }

    /// <summary>
    /// The available zone owner visibilities.
    /// �g�p�\�ȃ]�[�����L�҂̉����B
    /// </summary>
    public enum ZoneOwnerVisibility
    {
        Visible,
        Hidden,
    }

    /// <summary>
    /// The available zone opponent visibilities.
    /// �g�p�\�ȃ]�[���̑���̎��F���B
    /// </summary>
    public enum ZoneOpponentVisibility
    {
        Visible,
        Hidden
    }

    /// <summary>
    /// This class represents a game zone type available in a game.
    /// ���̃N���X�́A�Q�[���Ŏg�p�\�ȃQ�[���]�[���^�C�v��\���܂��B
    /// </summary>
    public class GameZoneType : Resource
    {
        /// <summary>
        /// The current resource identifier.
        /// ���݂̃��\�[�X���ʎq�B
        /// </summary>
        public static int currentId;

        /// <summary>
        /// The name of this zone.
        /// ���̃]�[���̖��O�B
        /// </summary>
        public string name;

        /// <summary>
        /// The owner of this zone.
        /// ���̃]�[���̏��L�ҁB
        /// </summary>
        public ZoneOwner owner;

        /// <summary>
        /// The type of this zone.
        /// ���̃]�[���̃^�C�v�B
        /// </summary>
        public ZoneType type;

        /// <summary>
        /// The visibility of this zone for the owner player.
        /// ���L�҃v���[���[�̂��߂̂��̃]�[���̉����B
        /// </summary>
        public ZoneOwnerVisibility ownerVisibility;

        /// <summary>
        /// The visibility of this zone for the opponent player.
        /// ����v���[���[�̂��߂̂��̃]�[���̉����B
        /// </summary>
        public ZoneOpponentVisibility opponentVisibility;

        /// <summary>
        /// True if this zone has a maximum size.
        /// ���̃]�[�����ő�T�C�Y�����ꍇ��True�ł��B
        /// </summary>
        public bool hasMaxSize;

        /// <summary>
        /// The maximum size of this number.
        /// ���̃]�[�����ŏ��T�C�Y�����ꍇ��True�ł��B
        /// </summary>
        public int maxSize;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameZoneType() : base(currentId++)
        {
        }
    }
}
