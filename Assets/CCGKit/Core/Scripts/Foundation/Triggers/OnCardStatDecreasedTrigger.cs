namespace CCGKit
{
    public class OnCardStatDecreasedTrigger : OnCardStatChangedTrigger
    {
        public override bool IsTrue(Stat stat, int newValue, int oldValue)
        {
            return stat.statId == statId && newValue < oldValue;
        }
    }
}