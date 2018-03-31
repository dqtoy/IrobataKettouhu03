using System;
using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class represents a runtime instance of a card.
    /// ���̃N���X�́A�J�[�h�̃����^�C���C���X�^���X(�v���O�����̎��s�ɕK�v�ȕ��i�̏W��)��\���܂��B
    /// </summary>
    public class RuntimeCard
    {
        /// <summary>
        /// The card identifier of this card.
        /// �J�[�h�̌ŗLID
        /// </summary>
        public int cardId;

        /// <summary>
        /// The instance identifier of this card.
        /// ���̃J�[�h�̃C���X�^���X���ʎq�B
        /// </summary>
        public int instanceId;

        /// <summary>
        /// The stats of this card, indexed by id.
        /// ���̃J�[�h�̃X�^�b�c��id�Ŏ�����܂��B
        /// </summary>
        public Dictionary<int, Stat> stats = new Dictionary<int, Stat>();

        /// <summary>
        /// The stats of this card, indexed by name.
        /// ���̃J�[�h�̃X�^�b�c�͖��O�Ŏ�����܂��B
        /// </summary>
        public Dictionary<string, Stat> namedStats = new Dictionary<string, Stat>();

        /// <summary>
        /// The keywords of this card.
        /// �J�[�h�̃L�[���[�h
        /// </summary>
        public List<RuntimeKeyword> keywords = new List<RuntimeKeyword>();

        /// <summary>
        /// The player that owns this card.
        /// ���̃J�[�h�����L���Ă���v���C���[�B
        /// </summary>
        public PlayerInfo ownerPlayer;

        /// <summary>
        /// The type of this card.
        /// �J�[�h�^�C�v
        /// </summary>
        public CardType cardType
        {
            get
            {
                var gameConfig = GameManager.Instance.config;
                var libraryCard = gameConfig.GetCard(cardId);
                return gameConfig.cardTypes.Find(x => x.id == libraryCard.cardTypeId);
            }
        }

        /// <summary>
        /// The callback that is called when a keyword is added to this card.
        /// ���̃J�[�h�ɃL�[���[�h���ǉ����ꂽ�Ƃ��ɌĂяo�����R�[���o�b�N�B
        /// </summary>
        public Action<RuntimeKeyword> onKeywordAdded;

        /// <summary>
        /// The callback that is called when a keyword is removed from this card.
        /// ���̃J�[�h����L�[���[�h���폜���ꂽ�Ƃ��ɌĂяo�����R�[���o�b�N�B
        /// </summary>
        public Action<RuntimeKeyword> onKeywordRemoved;

        /// <summary>
        /// Adds a keyword to this card.
        /// ���̃J�[�h�ɃL�[���[�h��ǉ�����
        /// </summary>
        /// <param name="keyword">The identifier of the keyword.</param>
        /// <param name="value">The value of the keyword.</param>
        public void AddKeyword(int keyword, int value)
        {
            var k = keywords.Find(x => x.keywordId == keyword && x.valueId == value);
            if (k == null)
            {
                k = new RuntimeKeyword();
                k.keywordId = keyword;
                k.valueId = value;
                keywords.Add(k);
                if (onKeywordAdded != null)
                {
                    onKeywordAdded(k);
                }
            }
        }

        /// <summary>
        /// Removes a keyword from this card.
        /// �J�[�h����L�[���[�h���폜����
        /// </summary>
        /// <param name="keyword">The identifier of this keyword.</param>
        /// <param name="value">The value of this keyword.</param>
        public void RemoveKeyword(int keyword, int value)
        {
            var k = keywords.Find(x => x.keywordId == keyword && x.valueId == value);
            if (k != null)
            {
                keywords.Remove(k);
                if (onKeywordRemoved != null)
                {
                    onKeywordRemoved(k);
                }
            }
        }

        /// <summary>
        /// Returns true if this card has the specified keyword and false otherwise.
        /// ���̃J�[�h�Ɏw�肳�ꂽ�L�[���[�h������ꍇ��true��Ԃ��A�����łȂ��ꍇ��false��Ԃ��܂��B
        /// </summary>
        /// <param name="name">The name of the keyword.</param>
        /// <returns>True if this card has the specified keyword; false otherwise.</returns>
        public bool HasKeyword(string name)
        {
            var gameConfig = GameManager.Instance.config;
            var keywordId = -1;
            var valueId = -1;
            foreach (var keyword in gameConfig.keywords)
            {
                var selectedValue = keyword.values.FindIndex(x => x.value == name);
                if (selectedValue != -1)
                {
                    keywordId = keyword.id;
                    valueId = selectedValue;
                    break;
                }
            }
            var k = keywords.Find(x => x.keywordId == keywordId && x.valueId == valueId);
            return k != null;
        }
    }
}
