using System.Data.HashFunction.Jenkins;
using System.Text;
using Redis.Common.Abstractions;

namespace Redis.Common.Implementations
{
    public class JenkinsHashGenerator : IHashGenerator
    {
        private readonly IJenkinsOneAtATime _jenkinsOneAtATime = JenkinsOneAtATimeFactory.Instance.Create();

        private readonly IBinarySerializer _serializer;
        private readonly IBitHelper _bitHelper;

        public JenkinsHashGenerator(IBinarySerializer serializer,
            IBitHelper bitHelper)
        {
            _serializer = serializer;
            _bitHelper = bitHelper;
        }

        public uint GenerateHash<T>(T obj)
        {
            var bytes = _serializer.Serialize(obj);

            var hash = _jenkinsOneAtATime.ComputeHash(bytes);//byte array should have 4 items (8 bytes each) and we map them to uint
            var hashBytes = hash.Hash;
            var intHash = _bitHelper.BytesToUInt32(hashBytes);
            return intHash;
        }
    }
}
