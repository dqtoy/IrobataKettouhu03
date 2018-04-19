using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// A Token set is a named collection of Tokens. Their main purpose is to help organize
    /// a big collection of Tokens into smaller, more manageable sub-groups.
    /// カードセットは名前付きのカードの集合です。 彼らの主な目的は、大規模なカードのコレクションを、より小さく、より管理しやすいサブグループに編成することです。
    /// </summary>
    public class TokenSet : CardSet
    {
        /// <summary>
        /// The name of this Token set.
        /// このカードセットの名前。
        /// </summary>
        public string name;

        /// <summary>
        /// The Tokens of this Token set.
        /// このカードセットのカード。
        /// </summary>
        public List<Token> tokens = new List<Token>();
    }
}
