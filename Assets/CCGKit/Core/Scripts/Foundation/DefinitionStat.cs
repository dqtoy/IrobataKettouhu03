namespace CCGKit
{
    /// <summary>
    /// The base stat class used in player and card definitions.
    /// �v���[���ƃJ�[�h�̒�`�Ŏg�p������{stat�N���X�B
    /// </summary>
    public class DefinitionStat : Resource
    {
        /// <summary>
        /// The name of this stat.
        /// </summary>
        public string name;

        /// <summary>
        /// The base value of this stat.
        /// </summary>
        public int baseValue;

        /// <summary>
        /// The original value of this stat.
        /// </summary>
        public int originalValue;

        /// <summary>
        /// The minimum value of this stat.
        /// </summary>
        public int minValue;

        /// <summary>
        /// The maximum value of this stat.
        /// </summary>
        public int maxValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The identifier of this resource.</param>
        public DefinitionStat(int id) : base(id)
        {
        }
    }

    /// <summary>
    /// The stat of a player definition.
    /// </summary>
    public class PlayerStat : DefinitionStat
    {
        /// <summary>
        /// The current resource identifier.
        /// �����̃v���C���[��ID�ŊǗ�����
        /// </summary>
        public static int currentId;

        /// <summary>
        /// Constructor.
        /// �Ăяo����邽�т�ID��1�����₷
        /// </summary>
        /// �h���N���X������N���X�̋��ʏ������Ăяo���Ƃ��́ubase.�֐����v�ŌĂяo�����Ƃ��ł���
        public PlayerStat() : base(currentId++)
        {
        }
    }

    /// <summary>
    /// The stat of a card definition.
    /// </summary>
    public class CardStat : DefinitionStat
    {
        /// <summary>
        /// The current resource identifier.
        /// </summary>
        public static int currentId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// �Ăяo����邽�т�ID��1�����₷
        /// �h���N���X������N���X�̋��ʏ������Ăяo���Ƃ��́ubase.�֐����v�ŌĂяo�����Ƃ��ł���
        public CardStat() : base(currentId++)
        {
        }
    }
}
