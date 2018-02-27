namespace CCGKit
{
    public class OnPlayerStatDecreasedTrigger : OnPlayerStatChangedTrigger
    {
        public override bool IsTrue(Stat stat, int newValue, int oldValue)
        {
            return stat.statId == statId && newValue < oldValue;
        }
    }
}