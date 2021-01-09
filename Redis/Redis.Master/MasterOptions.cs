using System.Collections.Generic;

namespace Redis.Master
{
    public class MasterOptions
    {
        public string ReplicationActor { get; set; }
        public List<string> Slaves { get; set; }
        public List<string> Children { get; set; }
    }
}
