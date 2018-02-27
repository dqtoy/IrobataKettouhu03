namespace CCGKit
{
    /// <summary>
    /// カードのスタッツをリセットする
    /// </summary>
    [CardTarget]
    public class ResetCardStatEffect : CardStatEffect
    {
        public override void Resolve(GameState state, RuntimeCard card)
        {
            var stat = card.stats[statId];
            stat.modifiers.Clear();
            stat.baseValue = stat.originalValue;
        }
    }
}