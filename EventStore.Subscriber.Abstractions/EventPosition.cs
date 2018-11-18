namespace EventStore.Subscriber.Abstractions
{
    public class EventPosition
    {
        public EventPosition(long commitPosition, long preparePosition)
        {
            CommitPosition = commitPosition;
            PreparePosition = preparePosition;
        }

        public int Id { get; private set; }
        public long CommitPosition { get; private set; }
        public long PreparePosition { get; private set; }

        public void WithNewPosition(long commit, long prepare)
        {
            CommitPosition = commit;
            PreparePosition = prepare;
        }
    }
}
