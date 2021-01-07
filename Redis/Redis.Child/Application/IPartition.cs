using System.Collections.Generic;

namespace Redis.Child.Application
{
    public interface IPartition
    {
        List<(uint hashKey, LinkedList<Entry> entries)> GetEntries();
        void Add<T>(string key, uint hashKey, T obj);
        void Add(string key, uint hashKey, string obj);
        string Get(string key, uint hashKey);
        T Get<T>(string key, uint hashKey);
    }
}