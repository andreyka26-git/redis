namespace Redis.Child.Application
{
    public class Entry
    {
        public Entry(uint hashCode, string key, string value)
        {
            HashCode = hashCode;
            Key = key;
            Value = value;
        }

        public uint HashCode { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
    }
}
