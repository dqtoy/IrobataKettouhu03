using System;
using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class represents a runtime instance of a zone.
    /// ���̃N���X�́A�]�[���̎��s���C���X�^���X��\���܂��B
    /// </summary>
    public class RuntimeZone
    {
        /// <summary>
        /// The identifier of this zone.
        /// ���̃]�[���̎��ʎq�B
        /// </summary>
        public int zoneId;

        /// <summary>
        /// The instance identifier of this zone.
        /// ���̃]�[���̃C���X�^���X���ʎq�B
        /// </summary>
        public int instanceId;

        /// <summary>
        /// The name of this zone.
        /// ���̃]�[���̖��O�B
        /// </summary>
        public string name;

        /// <summary>
        /// The cards of this zone.
        /// ���̃]�[���̃J�[�h�B
        /// </summary>
        public List<RuntimeCard> cards = new List<RuntimeCard>();

        /// <summary>
        /// The number of cards of this zone.
        /// ���̃]�[���̃J�[�h���B
        /// </summary>
        protected int _numCards;

        /// <summary>
        /// The number of cards of this zone.
        /// ���̃]�[���̃J�[�h���B
        /// </summary>
        public int numCards
        {
            get
            {
                return _numCards;
            }
            set
            {
                _numCards = value;
                if (onZoneChanged != null)
                {
                    onZoneChanged(_numCards);
                }
            }
        }

        /// <summary>
        /// The maximum number of cards of this zone.
        /// ���̃]�[���̃J�[�h�̍ő吔�B
        /// </summary>
        public int maxCards;

        /// <summary>
        /// The callback that is called when this zone changes.
        /// ���̃]�[�����ύX���ꂽ�Ƃ��ɌĂяo�����R�[���o�b�N�ł��B
        /// </summary>
        public Action<int> onZoneChanged;

        /// <summary>
        /// The callback that is called when a card is added to this zone.
        /// ���̃]�[���ɃJ�[�h���ǉ����ꂽ�Ƃ��ɌĂяo�����R�[���o�b�N�B
        /// </summary>
        public Action<RuntimeCard> onCardAdded;

        /// <summary>
        /// The callback that is called when a card is removed from this zone.
        /// ���̃]�[������J�[�h���폜���ꂽ�Ƃ��ɌĂяo�����R�[���o�b�N�B
        /// </summary>
        public Action<RuntimeCard> onCardRemoved;

        /// <summary>
        /// Adds a card to this zone.
        /// ���̃]�[���ɃJ�[�h��ǉ����܂��B
        /// </summary>
        /// <param name="card">The card to add.</param>
        public void AddCard(RuntimeCard card)
        {
            if (cards.Count < maxCards && !cards.Contains(card))
            {
                cards.Add(card);
                _numCards += 1;
                if (onZoneChanged != null)
                {
                    onZoneChanged(numCards);
                }
                if (onCardAdded != null)
                {
                    onCardAdded(card);
                }
            }
        }

        /// <summary>
        /// Removes a card from this zone.
        /// ���̃]�[������J�[�h��1���폜���܂��B
        /// </summary>
        /// <param name="card">The card to remove.</param>
        public void RemoveCard(RuntimeCard card)
        {
            if (cards.Contains(card))
            {
                cards.Remove(card);
                _numCards -= 1;
                if (onZoneChanged != null)
                {
                    onZoneChanged(numCards);
                }
                if (onCardRemoved != null)
                {
                    onCardRemoved(card);
                }
            }
        }

        /// <summary>
        /// Removes a number of cards from this zone.
        /// ���̃]�[�����炢�����̃J�[�h���폜���܂��B
        /// </summary>
        /// <param name="amount">The number of cards to remove.</param>
        public void RemoveCards(int amount)
        {
            cards.RemoveRange(0, amount);
            _numCards -= amount;
            if (onZoneChanged != null)
            {
                onZoneChanged(numCards);
            }
        }
    }
}
