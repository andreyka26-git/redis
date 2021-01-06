using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Redis.Common.Abstractions;

namespace Redis.Common.Implementations
{
    public class BsonSerializer : IBinarySerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            var ms = new MemoryStream();
            using (var writer = new BsonWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }

            return ms.ToArray();
        }
    }
}
