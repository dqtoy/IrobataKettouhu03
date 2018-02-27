namespace CCGKit
{
    /// <summary>
    /// カードのスタッツを設定する
    /// </summary>
    [CardTarget]
    public class SetCardStatEffect : CardStatEffect
    {
        [ValueField("Value")]
        [Order(4)]
        public Value value;

        [IntField("Duration")]
        [Order(5)]
        public int duration;

        public override void Resolve(GameState state, RuntimeCard card)
        {
            var stat = card.stats[statId];
            var newValue = value.GetValue(state, card.ownerPlayer);
            var diff = 0;
            if (stat.effectiveValue >= newValue)
            {
                diff = -(stat.effectiveValue - newValue);
            }
            else
            {
                diff = newValue - stat.effectiveValue;
            }
            var modifier = new Modifier(diff, duration);
            stat.AddModifier(modifier);
        }
    }
}