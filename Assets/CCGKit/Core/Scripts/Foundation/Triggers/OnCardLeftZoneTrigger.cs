namespace CCGKit
{
    public class OnCardLeftZoneTrigger : OnCardMovedTrigger
    {
        [GameZoneField("Zone")]
        public int zoneId;

        public override bool IsTrue(GameState state, string zone)
        {
            //gameZones‚Ìname‚ðŒŸõ(x‚É“ü‚é‚Ì‚ÍgameZones)
            var toZone = state.config.gameZones.Find(x => x.name == zone);
            return toZone.id == zoneId;
        }
    }
}