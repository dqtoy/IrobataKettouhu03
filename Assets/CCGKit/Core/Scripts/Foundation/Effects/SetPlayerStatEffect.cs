namespace CCGKit
{
    /// <summary>
    /// プレイヤーのスタッツを設定する
    /// </summary>
    [PlayerTarget]
    public class SetPlayerStatEffect : PlayerEffect
    {
        [PlayerStatField("Player stat")]
        [Order(1)]
        public int statId;

        [ValueField("Value")]
        [Order(2)]
        public Value value;

        [IntField("Duration")]
        [Order(3)]
        public int duration;

        public override void Resolve(GameState state, PlayerInfo player)
        {
            var stat = player.stats[statId];
            var newValue = value.GetValue(state, player);
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