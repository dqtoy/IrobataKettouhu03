namespace CCGKit
{
    public class OnPlayerStatReachedValueTrigger : OnPlayerStatChangedTrigger
    {
        [IntField("Value")]
        public int value;

        public override bool IsTrue(Stat stat, int newValue, int oldValue)
        {
            return stat.statId == statId && newValue == value;
        }
    }
}