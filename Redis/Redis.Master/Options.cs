using System.Collections.Generic;

namespace Redis.Master
{
    public class MasterOptions
    {
        public List<string> Children { get; set; }
        public int PartitionItemsCount { get; set; }
    }
}
