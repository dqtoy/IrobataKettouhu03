using System.Collections.Generic;

using UnityEngine.Assertions;

namespace CCGKit
{
    /// <summary>
    /// This class represents a single card in the game.
    /// ���̃N���X�́A�Q�[������1���̃J�[�h��\���܂��B
    /// </summary>
    public class Card : Resource
    {
        /// <summary>
        /// �J�[�h�̏����w�c(�ǉ�)
        /// </summary>
        public static int groupId { get; set; }

        /// <summary>
        /// The current resource identifier.
        /// ���݂̃��\�[�X���ʎq(�J�[�h�̃��j�[�NID)
        /// </summary>
//        public static int currentId;
        public static int currentId { get; set; }

        /// <summary>
        /// The type of this card.
        /// ���̃J�[�h�̃^�C�v�B
        /// </summary>
//        public int cardTypeId;
        public int cardTypeId { get; set; }

        /// <summary>
        /// The name of this card.
        /// ���̃J�[�h�̖��O�B
        /// </summary>
//        public string name;
        public string name { get; set; }

        /// <summary>
        /// The costs of this card.
        /// ���̃J�[�h�̃R�X�g�B
        /// </summary>
        public List<Cost> costs = new List<Cost>();

        /// <summary>
        /// The properties of this card.
        /// ���̃J�[�h�̓����B
        /// </summary>
        public List<Property> properties = new List<Property>();

        /// <summary>
        /// The stats of this card.
        /// ���̃J�[�h�̃X�^�b�c���B
        /// </summary>
        public List<Stat> stats = new List<Stat>();

        /// <summary>
        /// The keywords of this card.
        /// ���̃J�[�h�̃L�[���[�h�B
        /// </summary>
        public List<RuntimeKeyword> keywords = new List<RuntimeKeyword>();

        /// <summary>
        /// The abilities of this card.
        /// ���̃J�[�h�̔\�́B
        /// </summary>
        public List<Ability> abilities = new List<Ability>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public Card() : base(currentId++)
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
