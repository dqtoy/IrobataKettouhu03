using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class is the in-game entry point to the game configuration managed from within
    /// the CCG Kit menu option in Unity.
    /// ���̃N���X�́AUnity��CCG Kit���j���[�I�v�V��������Ǘ�����Ă���Q�[���ݒ�ւ̃Q�[�����G���g���|�C���g�ł��B
    /// </summary>
    public sealed class GameManager
    {
        /// <summary>
        /// The configuration of this game.
        /// ���̃Q�[���̍\���B
        /// </summary>
        public GameConfiguration config = new GameConfiguration();

        /// <summary>
        /// The default deck of this game.
        /// �Q�[���̃f�t�H���g�f�b�L
        /// </summary>
        public Deck defaultDeck;

        /// <summary>
        /// The player decks of this game.
        /// �v���C���[�����ɍ\�z���Ă���f�b�L
        /// </summary>
        public List<Deck> playerDecks = new List<Deck>();

        /// <summary>
        /// �Q�[�����Ƀg�[�N������������ێQ�Ƃ���g�[�N���̃v�[��
        /// �q���p�⃉���_���X�y������D�ɉ�����ۂɂ��g��
        /// </summary>
        public List<TokenPool> allPlayerTokens = new List<TokenPool>();

        /// <summary>
        /// The player's nickname of this game.
        /// �v���C���[��
        /// </summary>
        public string playerName;

        /// <summary>
        /// True if the player is logged in; false otherwise (used in Master Server Kit
        /// integration).
        /// �v���[���[�����O�C�����Ă���ꍇ��True�ł��B ����ȊO�̏ꍇ��false�ł��iMaster Server Kit�̓����Ŏg�p����܂��j�B
        /// </summary>
        public bool isPlayerLoggedIn;

        /// <summary>
        /// Static instance.
        /// �ÓI�C���X�^���X
        /// </summary>
        private static readonly GameManager instance = new GameManager();

        /// <summary>
        /// Constructor.
        /// �R���X�g���N�^
        /// </summary>
        private GameManager()
        {
            config.LoadGameConfigurationAtRuntime();
        }

        /// <summary>
        /// Static instance.
        /// �ÓI�C���X�^���X
        /// </summary>
        public static GameManager Instance
        {
            get { return instance; }
        }
    }
}