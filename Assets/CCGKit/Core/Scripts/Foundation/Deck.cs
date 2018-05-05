using System;
using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// A single entry in a deck.
    /// デッキ内の一つの入り口
    /// </summary>
    [Serializable]
    public class DeckEntry
    {
        /// <summary>
        /// The unique identifier of the card.
        /// カードのユニークID
        /// </summary>
        public int id;

        /// <summary>
        /// The number of copies of the card.
        /// カードのコピー枚数
        /// </summary>
        public int amount;
    }

    /// <summary>
    /// A deck is a collection of cards that players use when entering a game.
    /// デッキとは、プレイヤーがゲームに参加する際に使用するカードの集まりです。
    /// </summary>
    [Serializable]
    public class Deck
    {
        /// <summary>
        /// The name of this deck.
        /// デッキ名
        /// </summary>
        public string name = "新しいデッキ";

        /// <summary>
        /// The entries of this deck.
        /// デッキの入り口
        /// デッキに含まれるカードの固有IDとその所持枚数
        /// </summary>
        public List<DeckEntry> cards = new List<DeckEntry>();


        /// <summary>
        /// Returns the number of cards in this deck.
        /// このデッキのカード数を返します。
        /// </summary>
        /// <returns>The number of cards in this deck.</returns>
        public int GetNumCards()
        {
            var total = 0;
            foreach (var card in cards)
            {
                total += card.amount;
            }
            return total;
        }

        /// <summary>
        /// Returns the number of cards of the specified type in this deck.
        /// このデッキ内の指定されたタイプのカードの数を返します。
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// ゲームの設定
        /// <param name="cardTypeId">The card type.</param>
        /// カードの属性(ミニオンかスペルか)
        /// <returns>The number of cards of the specified type in this deck.</returns>
        /// このデッキの指定されたタイプのカードの数。
        public int GetNumCards(GameConfiguration config, int cardTypeId)
        {
            var total = 0;
            foreach (var card in cards)
            {
                foreach (var set in config.cardSets)
                {
                    var libraryCard = set.cards.Find(x => x.id == card.id);
                    if (libraryCard != null && libraryCard.cardTypeId == cardTypeId)
                    {
                        total += card.amount;
                        break;
                    }
                }
            }
            return total;
        }

        /// <summary>
        /// Adds the specified card to this deck.
        /// 指定されたカードをこのデッキに追加します。
        /// </summary>
        /// <param name="card">The card to add to this deck.</param>
        /// このデッキに追加するカード。
        public void AddCard(Card card)
        {
            var existingCard = cards.Find(x => x.id == card.id);
            if (existingCard != null)
            {
                existingCard.amount += 1;
            }
            else
            {
                cards.Add(new DeckEntry { id = card.id, amount = 1 });
            }
        }

        /// <summary>
        /// Removes this card from this deck.
        /// このカードをこのデッキから取り外す。
        /// </summary>
        /// <param name="card">The card to remove from this deck.</param>
        public void RemoveCards(Card card)
        {
            var existingCard = cards.Find(x => x.id == card.id);
            if (existingCard != null)
            {
                cards.Remove(existingCard);
            }
        }
    }
}