using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// This class represents a card keyword. Keywords are enum-like stats.
    /// このクラスはカードのキーワードを表します。 キーワードは列挙型の統計です。
    /// </summary>
    public class Keyword : Resource
    {
        /// <summary>
        /// The current resource identifier.
        /// 現在のリソース識別子。
        /// </summary>
        public static int currentId;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Keyword() : base(currentId++)
        {
        }

        /// <summary>
        /// The name of this keyword.
        /// このキーワードの名前。
        /// </summary>
        public string name;

        /// <summary>
        /// The values of this keyword.
        /// このキーワードの値。
        /// </summary>
        public List<KeywordValue> values = new List<KeywordValue>();
    }

    /// <summary>
    /// This class represents a keyword value.
    /// このクラスはキーワード値を表します。
    /// </summary>
    public class KeywordValue
    {
        /// <summary>
        /// The value of this keyword.
        /// このキーワードの値。
        /// </summary>
        public string value;
    }
}
