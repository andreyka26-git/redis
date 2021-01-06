namespace Redis.Common.Abstractions
{
    public interface IBinarySerializer
    {
        byte[] Serialize<T>(T obj);
    }
}