using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class represents a card keyword. Keywords are enum-like stats.
    /// ���̃N���X�̓J�[�h�̃L�[���[�h��\���܂��B �L�[���[�h�͗񋓌^�̓��v�ł��B
    /// </summary>
    public class Keyword : Resource
    {
        /// <summary>
        /// The current resource identifier.
        /// ���݂̃��\�[�X���ʎq�B
        /// </summary>
        public static int currentId;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Keyword() : base(currentId++)
        {
        }

        /// <summary>
        /// The name of this keyword.
        /// ���̃L�[���[�h�̖��O�B
        /// </summary>
        public string name;

        /// <summary>
        /// The values of this keyword.
        /// ���̃L�[���[�h�̒l�B
        /// </summary>
        public List<KeywordValue> values = new List<KeywordValue>();
    }

    /// <summary>
    /// This class represents a keyword value.
    /// ���̃N���X�̓L�[���[�h�l��\���܂��B
    /// </summary>
    public class KeywordValue
    {
        /// <summary>
        /// The value of this keyword.
        /// ���̃L�[���[�h�̒l�B
        /// </summary>
        public string value;
    }
}
