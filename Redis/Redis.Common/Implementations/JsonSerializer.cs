using System.Text;
using Newtonsoft.Json;
using Redis.Common.Abstractions;

namespace Redis.Common.Implementations
{
    public class JsonSerializer : IBinarySerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            var strObj = JsonConvert.SerializeObject(obj);
            var bytes = Encoding.UTF8.GetBytes(strObj);
            return bytes;
        }
    }
}
