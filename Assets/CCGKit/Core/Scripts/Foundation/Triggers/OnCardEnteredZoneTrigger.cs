namespace CCGKit
{
    public class OnCardEnteredZoneTrigger : OnCardMovedTrigger
    {
        [GameZoneField("Zone")]
        public int zoneId;

        public override bool IsTrue(GameState state, string zone)
        {
            var toZone = state.config.gameZones.Find(x => x.name == zone);
            return toZone.id == zoneId;
        }
    }
}