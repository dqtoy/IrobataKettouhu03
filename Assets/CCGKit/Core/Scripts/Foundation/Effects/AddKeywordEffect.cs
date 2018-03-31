namespace CCGKit
{
    [CardTarget]
    public class AddKeywordEffect : CardEffect
    {
        [KeywordTypeField("Keyword")]
        [Order(3)]
        public int keywordTypeId;

        [KeywordValueField("Value")]
        [Order(4)]
        public int keywordValueId;

        public override void Resolve(GameState state, RuntimeCard card)
        {
            card.AddKeyword(keywordTypeId, keywordValueId);
        }
    }
}