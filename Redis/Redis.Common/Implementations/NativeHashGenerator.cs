using Redis.Common.Abstractions;

namespace Redis.Common.Implementations
{
    public class NativeHashGenerator : IHashGenerator
    {
        public uint GenerateHash<T>(T obj)
        {
            return (uint) obj.GetHashCode();
        }
    }
}
