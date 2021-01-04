namespace Redis.Child.Application
{
    public interface IPartition
    {
        void Add<T>(string key, int hashKey, T obj);
        void Add(string key, int hashKey, string obj);
        string Get(string key, int hashKey);
        T Get<T>(string key, int hashKey);
    }
}