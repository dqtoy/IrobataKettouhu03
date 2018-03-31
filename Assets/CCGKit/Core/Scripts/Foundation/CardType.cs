using UnityEngine.Assertions;

using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class represents a single card type in the game.
    /// ���̃N���X�́A�Q�[�����̒P��̃J�[�h�^�C�v��\���܂��B
    /// </summary>
    public class CardType : Resource
    {
        /// <summary>
        /// The current resource identifier.
        /// ���݂̃��\�[�X���ʎq�B
        /// </summary>
        public static int currentId;

        /// <summary>
        /// The name of this card type.
        /// ���̃J�[�h�^�C�v�̖��O�B
        /// </summary>
        public string name;

        /// <summary>
        /// The properties of this card type.
        /// ���̃J�[�h�^�C�v�̃v���p�e�B�B
        /// </summary>
        public List<Property> properties = new List<Property>();

        /// <summary>
        /// The stats of this card type.
        /// ���̃J�[�h�^�C�v�̃X�^�b�c
        /// </summary>
        public List<DefinitionStat> stats = new List<DefinitionStat>();

        /// <summary>
        /// The destroy conditions of this card type.
        /// ���̃J�[�h�^�C�v�̔j������B
        /// </summary>
        public List<DestroyCardCondition> destroyConditions = new List<DestroyCardCondition>();

        /// <summary>
        /// True if this card should move to another zone after triggering its effect
        /// ���̃J�[�h�����ʂ𔭓�������ɕʂ̃]�[���Ɉړ�����K�v������ꍇ��True
        /// (useful for spell-like cards); false otherwise.
        /// �i�X�y���̂悤�ȃJ�[�h�ɕ֗��ł��j�B �����łȂ����false�B
        /// </summary>
        public bool moveAfterTriggeringEffect;

        /// <summary>
        /// The zone to which this card should move after triggering its effect.
        /// ���̃J�[�h�����ʂ𔭓�������Ɉړ�����]�[���B
        /// </summary>
        [GameZoneField("Zone")]
        public int zoneId;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CardType() : base(currentId++)
        {
        }

        /// <summary>
        /// Returns the value of the integer property with the specified name.
        /// �w�肳�ꂽ���O���������v���p�e�B�̒l��Ԃ��܂��B
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public int GetIntProperty(string name)
        {
            var property = properties.Find(x => x.name == name && x is IntProperty);
            Assert.IsNotNull(property);
            return (property as IntProperty).value;
        }

        /// <summary>
        /// Returns the value of the string property with the specified name.
        /// �w�肵�����O�̕�����v���p�e�B�̒l��Ԃ��܂��B
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public string GetStringProperty(string name)
        {
            var property = properties.Find(x => x.name == name && x is StringProperty);
            Assert.IsNotNull(property);
            return (property as StringProperty).value;
        }
    }
}
