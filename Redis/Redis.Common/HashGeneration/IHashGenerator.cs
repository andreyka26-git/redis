namespace Redis.Common.HashGeneration
{
    public interface IHashGenerator
    {
        int GenerateHash<T>(T obj);
    }
}