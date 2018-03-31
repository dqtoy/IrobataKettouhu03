namespace CCGKit
{
    /// <summary>
    /// プレイヤーのダメージ計算
    /// </summary>
    [PlayerTarget]
    public class DecreasePlayerStatEffect : PlayerEffect
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
            var modifier = new Modifier(-value.GetValue(state, player), duration);
            player.stats[statId].AddModifier(modifier);
        }
    }
}