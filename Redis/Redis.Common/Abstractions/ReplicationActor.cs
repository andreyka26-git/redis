namespace Redis.Common.Abstractions
{
    public class ReplicationActor
    {
        public static ReplicationActor Master = new ReplicationActor("Master");
        public static ReplicationActor Slave = new ReplicationActor("Slave");

        private ReplicationActor(string actorName)
        {
            ActorName = actorName;
        }

        public string ActorName { get; private set; }
    }
}
