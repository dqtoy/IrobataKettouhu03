using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// A card set is a named collection of cards. Their main purpose is to help organize
    /// a big collection of cards into smaller, more manageable sub-groups.
    /// �J�[�h�Z�b�g�͖��O�t���̃J�[�h�̏W���ł��B �ނ�̎�ȖړI�́A��K�͂ȃJ�[�h�̃R���N�V�������A��菬�����A���Ǘ����₷���T�u�O���[�v�ɕҐ����邱�Ƃł��B
    /// </summary>
    public class CardSet
    {
        /// <summary>
        /// �J�[�h�Z�b�g��ID�B�g���قƂ��̐w�c��ID
        /// </summary>
        public int Id;
        /// <summary>
        /// The name of this card set.
        /// ���̃J�[�h�Z�b�g�̖��O�B"�g����"��"���ʘO"���B
        /// </summary>
        public string name;

        /// <summary>
        /// The cards of this card set.
        /// ���̃J�[�h�Z�b�g�̃J�[�h�B
        /// </summary>
        public List<Card> cards = new List<Card>();
    }
}
