using System;
using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class represents a runtime instance of a Token.
    /// このクラスは、カードのランタイムインスタンス(プログラムの実行に必要な部品の集合)を表します。
    /// </summary>
    public class RuntimeToken
    {
        /// <summary>
        /// The Token identifier of this Token.
        /// カードの固有ID
        /// </summary>
        public int TokenId;

        /// <summary>
        /// The instance identifier of this Token.
        /// このカードのインスタンス識別子。
        /// </summary>
        public int instanceId;

        /// <summary>
        /// The stats of this Token, indexed by id.
        /// このカードのスタッツはidで示されます。
        /// </summary>
        public Dictionary<int, Stat> stats = new Dictionary<int, Stat>();

        /// <summary>
        /// The stats of this Token, indexed by name.
        /// このカードのスタッツは名前で示されます。
        /// </summary>
        public Dictionary<string, Stat> namedStats = new Dictionary<string, Stat>();

        /// <summary>
        /// The keywords of this Token.
        /// カードのキーワード
        /// </summary>
        public List<RuntimeKeyword> keywords = new List<RuntimeKeyword>();

        /// <summary>
        /// The player that owns this Token.
        /// このカードを所有しているプレイヤー。
        /// </summary>
        public PlayerInfo ownerPlayer;

        /// <summary>
        /// The type of this Token.
        /// カードタイプ
        /// </summary>
        public TokenType TokenType
        {
            get
            {
                var gameConfig = GameManager.Instance.config;
                var libraryToken = gameConfig.GetToken(TokenId);
                return gameConfig.tokenTypes.Find(x => x.id == libraryToken.tokenTypeId);
            }
        }

        /// <summary>
        /// The callback that is called when a keyword is added to this Token.
        /// このカードにキーワードが追加されたときに呼び出されるコールバック。
        /// </summary>
        public Action<RuntimeKeyword> onKeywordAdded;

        /// <summary>
        /// The callback that is called when a keyword is removed from this Token.
        /// このカードからキーワードが削除されたときに呼び出されるコールバック。
        /// </summary>
        public Action<RuntimeKeyword> onKeywordRemoved;

        /// <summary>
        /// Adds a keyword to this Token.
        /// このカードにキーワードを追加する
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
        /// Removes a keyword from this Token.
        /// カードからキーワードを削除する
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
        /// Returns true if this Token has the specified keyword and false otherwise.
        /// このカードに指定されたキーワードがある場合はtrueを返し、そうでない場合はfalseを返します。
        /// </summary>
        /// <param name="name">The name of the keyword.</param>
        /// <returns>True if this Token has the specified keyword; false otherwise.</returns>
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
