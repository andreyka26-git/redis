using System;
using Redis.Common.Abstractions;

namespace Redis.Tests.TestEntities
{
    public class BitHelperFake : IBitHelper
    {
        public uint BytesToUInt32(byte[] bytes)
        {
            //because uint32 has 4 bytes by 8 length each
            if (bytes.Length != 4)
                throw new Exception($"Cannot convert from bytes to uint32 because byte length is not equal to 4");

            var intHash = BitConverter.ToInt32(bytes, 0);
            //var intHash = bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3];
            return (uint)intHash;
        }
    }
}
