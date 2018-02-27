namespace CCGKit
{
    public class OnCardStatReachedValueTrigger : OnCardStatChangedTrigger
    {
        [IntField("Value")]
        public int value;

        public override bool IsTrue(Stat stat, int newValue, int oldValue)
        {
            return stat.statId == statId && newValue == value;
        }
    }
}