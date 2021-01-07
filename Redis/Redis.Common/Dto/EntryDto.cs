namespace Redis.Common.Dto
{
    public class EntryDto
    {
        public EntryDto(uint hashkey, string key, string value)
        {
            HashKey = hashkey;
            Key = key;
            Value = value;
        }

        public uint HashKey { get;  set; }
        public string Key { get;  set; }
        public string Value { get;  set; }
    }
}
