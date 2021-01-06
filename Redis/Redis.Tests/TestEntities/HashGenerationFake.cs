using Redis.Common.Abstractions;

namespace Redis.Tests.TestEntities
{
    internal class HashGeneratorFake : IHashGenerator
    {
        public uint GenerateHash<T>(T obj)
        {
            return ((uint)obj.GetHashCode());
        }
    }
}
