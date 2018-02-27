namespace CCGKit
{
    public class OnCardStatIncreasedTrigger : OnCardStatChangedTrigger
    {
        public override bool IsTrue(Stat stat, int newValue, int oldValue)
        {
            return stat.statId == statId && newValue > oldValue;
        }
    }
}