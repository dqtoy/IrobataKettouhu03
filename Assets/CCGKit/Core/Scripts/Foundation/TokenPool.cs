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
        public Card Token { get; private set; }
        
        /// <summary>
        /// Adds the specified card to this deck.
        /// 指定されたカードをトークンプールに追加します。
        /// </summary>
        /// <param name="card">The card to add to this deck.</param>
        /// このデッキに追加するカード。
        public void AddToken(Card token)
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

    }
}