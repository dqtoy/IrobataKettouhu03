namespace CCGKit
{
    /// <summary>
    /// プレイヤーのスタッツをリセットする
    /// </summary>
    [PlayerTarget]
    public class ResetPlayerStatEffect : PlayerEffect
    {
        [PlayerStatField("Player stat")]
        [Order(1)]
        public int statId;

        public override void Resolve(GameState state, PlayerInfo player)
        {
            var stat = player.stats[statId];
            stat.modifiers.Clear();
            stat.baseValue = stat.originalValue;
        }
    }
}