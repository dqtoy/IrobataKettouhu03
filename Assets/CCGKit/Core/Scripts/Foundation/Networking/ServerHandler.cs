namespace CCGKit
{
    /// <summary>
    /// Base type intended to be subclassed from in order to provide management of a specific subset
    /// of the server's functionality. This separation of concerns helps to avoid having a gigantic
    /// server class and makes the code more maintainable and reusable.
    /// �T�[�o�̋@�\�̓���̃T�u�Z�b�g�̊Ǘ���񋟂��邽�߂ɁA�T�u�N���X������邱�Ƃ��Ӑ}�����x�[�X�^�C�v�B 
    /// ���̌��O�̕����́A����ȃT�[�o�[�N���X������A�R�[�h����胁���e�i���X���₷���A�ė��p�I�ɂ��܂��B
    /// </summary>
    public class ServerHandler
    {
        /// <summary>
        /// Convenient access to the server itself.
        /// �T�[�o�[���̂ɊȒP�ɃA�N�Z�X�ł��܂��B
        /// </summary>
        protected Server server;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server">Reference to the server.</param>
        public ServerHandler(Server server)
        {
            this.server = server;
        }

        /// <summary>
        /// This method is where subclasses should register to receive the network messages they are
        /// interested in.
        /// ���̃��\�b�h�́A�T�u�N���X���A�����̂���l�b�g���[�N���b�Z�[�W����M���邽�߂ɓo�^����K�v������܂��B
        /// </summary>
        public virtual void RegisterNetworkHandlers()
        {
        }

        /// <summary>
        /// This method is where subclasses should unregister to stop receiving the network messages they are
        /// interested in.
        /// ���̃��\�b�h�́A�T�u�N���X���o�^���������āA�֐S�̂���l�b�g���[�N���b�Z�[�W�̎�M���~����K�v������܂��B
        /// </summary>
        public virtual void UnregisterNetworkHandlers()
        {
        }

        /// <summary>
        /// This method provides a convenient entry point for subclasses to perform any turn-start-specific
        /// initialization logic.
        /// ���̃��\�b�h�́A�^�[���E�X�^�[�g�ŗL�̏��������W�b�N�����s���邽�߂̃T�u�N���X�ɕ֗��ȃG���g���E�|�C���g��񋟂��܂��B
        /// </summary>
        public virtual void OnStartTurn()
        {
        }

        /// <summary>
        /// This method provides a convenient entry point for subclasses to perform any turn-end-specific
        /// cleanup logic.
        /// ���̃��\�b�h�́A�T�u�N���X���^�[���G���h�ŗL�̃N���[���A�b�v���W�b�N�����s���邽�߂֗̕��ȃG���g���|�C���g��񋟂��܂��B
        /// </summary>
        public virtual void OnEndTurn()
        {
        }
    }
}
