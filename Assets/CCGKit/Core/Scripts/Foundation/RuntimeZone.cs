using System;
using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class represents a runtime instance of a zone.
    /// このクラスは、ゾーンの実行時インスタンスを表します。
    /// </summary>
    public class RuntimeZone
    {
        /// <summary>
        /// The identifier of this zone.
        /// このゾーンの識別子。
        /// </summary>
        public int zoneId;

        /// <summary>
        /// The instance identifier of this zone.
        /// このゾーンのインスタンス識別子。
        /// </summary>
        public int instanceId;

        /// <summary>
        /// The name of this zone.
        /// このゾーンの名前。
        /// </summary>
        public string name;

        /// <summary>
        /// The cards of this zone.
        /// このゾーンのカード。
        /// </summary>
        public List<RuntimeCard> cards = new List<RuntimeCard>();

        /// <summary>
        /// The number of cards of this zone.
        /// このゾーンのカード数。
        /// </summary>
        protected int _numCards;

        /// <summary>
        /// The number of cards of this zone.
        /// このゾーンのカード数。
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
        /// このゾーンのカードの最大数。
        /// </summary>
        public int maxCards;

        /// <summary>
        /// The callback that is called when this zone changes.
        /// このゾーンが変更されたときに呼び出されるコールバックです。
        /// </summary>
        public Action<int> onZoneChanged;

        /// <summary>
        /// The callback that is called when a card is added to this zone.
        /// このゾーンにカードが追加されたときに呼び出されるコールバック。
        /// </summary>
        public Action<RuntimeCard> onCardAdded;

        /// <summary>
        /// The callback that is called when a card is removed from this zone.
        /// このゾーンからカードが削除されたときに呼び出されるコールバック。
        /// </summary>
        public Action<RuntimeCard> onCardRemoved;

        /// <summary>
        /// Adds a card to this zone.
        /// このゾーンにカードを追加します。
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
        /// このゾーンからカードを1枚削除します。
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
        /// このゾーンからいくつかのカードを削除します。
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
