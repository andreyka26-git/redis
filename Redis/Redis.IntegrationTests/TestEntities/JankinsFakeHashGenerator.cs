using System;
using Redis.Common.Abstractions;

namespace Redis.IntegrationTests.TestEntities
{
    public class JankinsFakeHashGenerator : IHashGenerator
    {
        public uint GenerateHash<T>(T obj)
        {
            var hash = Convert.ToInt32(obj);
            return (uint)hash % 107;
        }
    }
}
