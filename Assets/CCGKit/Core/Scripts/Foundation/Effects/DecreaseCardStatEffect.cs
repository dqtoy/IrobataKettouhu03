namespace CCGKit
{
    /// <summary>
    /// カードのダメージ計算
    /// </summary>
    [CardTarget]
    public class DecreaseCardStatEffect : CardStatEffect
    {
        [ValueField("Value")]
        [Order(4)]
        public Value value;

        [IntField("Duration")]
        [Order(5)]
        public int duration;

        public override void Resolve(GameState state, RuntimeCard card)
        {
            var modifier = new Modifier(-value.GetValue(state, card.ownerPlayer), duration);
            card.stats[statId].AddModifier(modifier);
        }
    }
}
