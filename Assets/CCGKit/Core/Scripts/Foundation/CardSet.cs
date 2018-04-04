using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// A card set is a named collection of cards. Their main purpose is to help organize
    /// a big collection of cards into smaller, more manageable sub-groups.
    /// カードセットは名前付きのカードの集合です。 彼らの主な目的は、大規模なカードのコレクションを、より小さく、より管理しやすいサブグループに編成することです。
    /// </summary>
    public class CardSet
    {
        /// <summary>
        /// カードセットのID。紅魔館とかの陣営のID
        /// </summary>
        public int Id;
        /// <summary>
        /// The name of this card set.
        /// このカードセットの名前。"紅魔館"や"白玉楼"等。
        /// </summary>
        public string name;

        /// <summary>
        /// The cards of this card set.
        /// このカードセットのカード。
        /// </summary>
        public List<Card> cards = new List<Card>();
    }
}
