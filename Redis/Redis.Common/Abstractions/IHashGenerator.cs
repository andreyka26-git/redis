namespace Redis.Common.Abstractions
{
    public interface IHashGenerator
    {
        uint GenerateHash<T>(T obj);
    }
}