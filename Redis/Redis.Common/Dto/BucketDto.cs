using System.Collections.Generic;

namespace Redis.Common.Dto
{
    public class BucketDto
    {
        public BucketDto(uint bucketIndex, IEnumerable<EntryDto> entries)
        {
            BucketIndex = bucketIndex;
            Entries = entries;
        }

        public uint BucketIndex { get; set; }
        public IEnumerable<EntryDto> Entries { get; set; }
    }
}
