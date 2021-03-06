namespace CCGKit
{
    public class OnCardLeftZoneTrigger : OnCardMovedTrigger
    {
        [GameZoneField("Zone")]
        public int zoneId;

        public override bool IsTrue(GameState state, string zone)
        {
            //gameZonesのnameを検索(xに入るのはgameZones)
            var toZone = state.config.gameZones.Find(x => x.name == zone);
            return toZone.id == zoneId;
        }
    }
}