namespace Redis.Common.HashGeneration
{
    public class HashGenerator : IHashGenerator
    {
        //Implement some platform independent hash generation like MurmurHash
        public int GenerateHash<T>(T obj)
        {
            return (int)((uint) obj.GetHashCode());
        }
    }
}
