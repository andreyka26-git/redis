using System.Collections.Generic;

namespace Redis.Common.Dto
{
    public class ChildEntriesDto
    {
        public ChildEntriesDto(string url, List<BucketDto> entries)
        {
            Url = url;
            Entries = entries;
        }

        public string Url { get; set; }

        public List<BucketDto> Entries { get; set; }
    }
}
