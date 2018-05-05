using System;
using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// デッキ内の一つの入り口
    /// </summary>
    [Serializable]
    public class TokenPoolEntry
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
    public class TokenPool
    {
        /// <summary>
        /// デッキの入り口
        /// デッキに含まれるカードの固有IDとその所持枚数
        /// </summary>
        public List<TokenPoolEntry> tokens = new List<TokenPoolEntry>();

        /// <summary>
        /// トークンのゲッターセッター
        /// </summary>
        public Token Token { get; private set; }

        /// <summary>
        /// Returns the number of cards of the specified type in this deck.
        /// このデッキ内の指定されたタイプのカードの数を返します。
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// ゲームの設定
        /// <param name="cardTypeId">The card type.</param>
        /// カードの属性
        /// <returns>The number of cards of the specified type in this deck.</returns>
        /// このデッキの指定されたタイプのカードの数。
/*
        public int GetNumCards(GameConfiguration config, int tokenTypeId)
        {
            var total = 0;
            foreach (var token in tokens)
            {
                foreach (var set in config.tokenSets)
                {
                    var libraryToken = set.tokens.Find(x => x.id == token.id);
                    if (libraryToken != null && libraryToken.tokenTypeId == tokenTypeId)
                    {
                        total += token.amount;
                        break;
                    }
                }
            }
            return total;
        }
*/
        /// <summary>
        /// Adds the specified card to this deck.
        /// 指定されたカードをこのデッキに追加します。
        /// </summary>
        /// <param name="card">The card to add to this deck.</param>
        /// このデッキに追加するカード。
        public void AddCard(Card token)
        {
            var existingToken = tokens.Find(x => x.id == token.id);
            if (existingToken != null)
            {
                existingToken.amount += 1;
            }
            else
            {
                tokens.Add(new TokenPoolEntry { id = token.id, amount = 1 });
            }
        }

        /// <summary>
        /// Removes this card from this deck.
        /// このカードをこのデッキから取り外す。
        /// </summary>
        /// <param name="card">The card to remove from this deck.</param>
        public void RemoveCards(Card token)
        {
            var existingToken = tokens.Find(x => x.id == token.id);
            if (existingToken != null)
            {
                tokens.Remove(existingToken);
            }
        }
    }
}