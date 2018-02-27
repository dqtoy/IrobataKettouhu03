namespace CCGKit
{
    [CardTarget]
    public class RemoveKeywordEffect : CardEffect
    {
        [KeywordTypeField("Keyword")]
        [Order(3)]
        public int keywordTypeId;

        [KeywordValueField("Value")]
        [Order(4)]
        public int keywordValueId;

        /// <summary>
        /// カードからキーワードを削除する
        /// </summary>
        /// <param name="state"></param>
        /// <param name="card"></param>
        public override void Resolve(GameState state, RuntimeCard card)
        {
            card.RemoveKeyword(keywordTypeId, keywordValueId);
        }
    }
}