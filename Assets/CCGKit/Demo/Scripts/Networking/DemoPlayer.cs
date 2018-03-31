using CCGKit;

public class DemoPlayer : Player
{
    public override void OnStartTurn(StartTurnMessage msg)
    {
        base.OnStartTurn(msg);
        if (msg.isRecipientTheActivePlayer)
        {
            /*
            var handZone = Array.Find(msg.StaticGameZones, x => x.Name == "Hand");
            hand = new List<int>(handZone.Cards);
            */
        }
    }
}