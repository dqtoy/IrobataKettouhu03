// Copyright (C) 2016-2017 David Pol. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// This class represents a condition in which a card stat is checked to determine if
    /// it contains a certain keyword.
    /// このクラスは、カード統計が特定のキーワードを含むかどうかを判断するためにチェックされる条件を表します。
    /// </summary>
    public class CardWithKeywordCondition : CardCondition
    {
        /// <summary>
        /// The keyword of this condition.
        /// この条件のキーワード
        /// </summary>
        [KeywordTypeField("Keyword")]
        [Order(2)]
        public int keywordId;

        /// <summary>
        /// The value of this condition.
        /// この条件の値
        /// </summary>
        [KeywordValueField("Value")]
        [Order(3)]
        public int valueId;

        /// <summary>
        /// Returns a readable string representing this condition.
        /// この条件を表す読み込み可能な文字列を返します。
        /// </summary>
        /// <param name="config">The game's configuration.</param>
        /// <returns>A readable string that represents this condition.</returns>
        public override string GetReadableString(GameConfiguration config)
        {
            var keyword = config.keywords.Find(x => x.id == keywordId);
            if (keyword != null)
            {
                return "With " + keyword.values[valueId].value;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Returns true if this condition has been met on the specified card and false otherwise.
        /// 指定されたカードでこの条件が満たされている場合はtrueを返し、そうでない場合はfalseを返します。
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if this condition has been met on the specified card; false otherwise.</returns>
        /// <returns>この条件が指定されたカードで満たされていれば真。 そうでない場合はfalseです。</ returns>
        public override bool IsTrue(RuntimeCard card)
        {
            return card.keywords.Find(x => x.keywordId == keywordId && x.valueId == valueId) != null;
        }
    }
}
