using Redis.Common.HashGeneration;

namespace Redis.Tests.TestEntities
{
    internal class HashGeneratorFake : IHashGenerator
    {
        public int GenerateHash<T>(T obj)
        {
            return (int)((uint)obj.GetHashCode());
        }
    }
}
